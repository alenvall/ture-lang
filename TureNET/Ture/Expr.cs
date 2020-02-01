using System;

namespace Ture
{
    public abstract class Expr { }

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
    }

    public class Grouping : Expr
    {
        public Expr Expression;

        public Grouping(Expr expression)
        {
            Expression = expression;
        }
    }

    public class Literal : Expr
    {
        public Object Value;

        public Literal(Object value)
        {
            Value = value;
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
    }

}
