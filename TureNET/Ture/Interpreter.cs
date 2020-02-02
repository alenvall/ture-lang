using static Ture.TokenType;

namespace Ture
{
    public class Interpreter : Expr.IVisitor<object>
    {
        public string Interpret(Expr expression)
        {
            try
            {
                object value = Evaluate(expression);
                return value.ToString();
            }
            catch (RuntimeError error)
            {
                Ture.RuntimeError(error);
            }

            return null;
        }

        public Interpreter() { }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitUnaryExpr(Unary expr)
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

        public object VisitBinaryExpr(Binary expr)
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
