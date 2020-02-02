using System;

namespace Ture
{
    public abstract class Expr
    {
        public interface IVisitor<R>
        {
            public R VisitBinaryExpr(Binary expr);
            public R VisitGroupingExpr(Grouping expr);
            public R VisitLiteralExpr(Literal expr);
            public R VisitUnaryExpr(Unary expr);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);
    }

    public class Binary : Expr
    {
        public Expr Left;
        public Token Oper;
        public Expr Right;

        public Binary(Expr left, Token oper, Expr right)
        {
            Left = left;
            Oper = oper;
            Right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression;

        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public Object Value;

        public Literal(Object value)
        {
            Value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Oper;
        public Expr Right;

        public Unary(Token oper, Expr right)
        {
            Oper = oper;
            Right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }


}
