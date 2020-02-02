using System;
using System.Collections.Generic;

namespace Ture.Models
{
    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            public R VisitBlockStmt(Block stmt);
            public R VisitExpressionStmt(Expression stmt);
            public R VisitFunctionStmt(Function stmt);
            public R VisitIfStmt(If stmt);
            public R VisitPrintStmt(Print stmt);
            public R VisitReturnStmt(Return stmt);
            public R VisitVarStmt(Var stmt);
            public R VisitWhileStmt(While stmt);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);

        public class Block : Stmt
        {
            public ICollection<Stmt> Statements;

            public Block(ICollection<Stmt> statements)
            {
                Statements = statements;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        public class Expression : Stmt
        {
            public Expr Expr;

            public Expression(Expr expr)
            {
                Expr = expr;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Function : Stmt
        {
            public Token Name;
            public IList<Token> Parameters;
            public ICollection<Stmt> Body;

            public Function(Token name, IList<Token> parameters, ICollection<Stmt> body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        public class If : Stmt
        {
            public Expr Condition;
            public Stmt ThenBranch;
            public Stmt ElseBranch;

            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        public class Print : Stmt
        {
            public Expr Expr;

            public Print(Expr expr)
            {
                Expr = expr;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        public class Return : Stmt
        {
            public Token Keyword;
            public Expr Value;

            public Return(Token keyword, Expr value)
            {
                Keyword = keyword;
                Value = value;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        public class Var : Stmt
        {
            public Token Name;
            public Expr Initializer;

            public Var(Token name, Expr initializer)
            {
                Name = name;
                Initializer = initializer;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        public class While : Stmt
        {
            public Expr Condition;
            public Stmt Body;

            public While(Expr condition, Stmt body)
            {
                Condition = condition;
                Body = body;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }

    }
}
