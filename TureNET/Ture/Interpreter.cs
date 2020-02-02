using System.Collections.Generic;
using Ture.Core;
using Ture.Core.Native;
using Ture.Models;

using static Ture.Models.TokenType;

namespace Ture
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        public readonly Environment Globals = new Environment();
        private Environment environment;

        public string Interpret(ICollection<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Ture.RuntimeError(error);
            }

            return null;
        }

        public Interpreter()
        {
            environment = Globals;

            var clock = new Clock();
            Globals.Define(clock.GetType().Name.ToLower(), clock);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Oper.Type == TokenType.OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type)
            {
                case MINUS:
                    CheckNumberOperand(expr.Oper, right);
                    return -(double)right;
                case EXCLAMATION:
                    return !IsTruthy(right);
            }

            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left - (double)right;

                case SLASH:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left / (double)right;

                case STAR:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left * (double)right;

                case PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.Oper, "Operands must be two numbers or two strings.");

                case GREATER:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left > (double)right;

                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left >= (double)right;

                case LESS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left < (double)right;

                case LESS_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return (double)left <= (double)right;

                case EQUAL_EQUAL:
                    return IsEqual(left, right);

                case EXCLAMATION_EQUAL:
                    return !IsEqual(left, right);


                default:
                    break;
            }

            return null;
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.Callee);

            IList<object> arguments = new List<object>();

            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }


            if (!(callee is ICallable))
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes");
            }

            ICallable function = (ICallable)callee;

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, "Expected " + function.Arity() + " arguments but got " + arguments.Count);
            }

            return function.Call(this, arguments);
        }

        private void CheckNumberOperand(Token oper, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeError(oper, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token oper, object left, object right)
        {
            if (left is double && right is double)
            {
                return;
            }

            throw new RuntimeError(oper, "Operands must be numbers.");
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void ExecuteBlock(ICollection<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;

            try
            {
                this.environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
            return null;
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);

            return null;
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            Function function = new Function(stmt);
            environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }

            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.Expr);
            Ture.Print(value.ToString());

            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;

            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            throw new Return(value);
        }

        public object VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;

            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            environment.Define(stmt.Name.Lexeme, value);

            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }

            return null;
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evaluate(expr.Value);

            environment.Assign(expr.Name, value);
            return value;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool)
            {
                return (bool)obj;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

    }
}
