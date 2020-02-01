using System.Collections.Generic;

using static Ture.TokenType;

namespace Ture
{
    public class Parser
    {
        private readonly IList<Token> tokens;
        private int current = 0;

        public Parser(IList<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparsion();

            while (Match(EXCLAMATION_EQUAL, EQUAL_EQUAL))
            {
                Token oper = Previous();
                Expr right = Comparsion();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Comparsion()
        {
            Expr expr = Addition();

            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                Token oper = Previous();
                Expr right = Addition();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            Expr expr = Multiplication();

            while (Match(MINUS, PLUS))
            {
                Token oper = Previous();
                Expr right = Multiplication();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            Expr expr = Unary();

            while(Match(SLASH, STAR))
            {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(EXCLAMATION, MINUS))
            {
                Token oper = Previous();
                Expr right = Unary();
                return new Unary(oper, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Literal(false);
            }
            if (Match(TRUE))
            {
                return new Literal(true);
            }
            if (Match(NULL))
            {
                return new Literal(null);
            }
            if (Match(NUMBER, STRING))
            {
                return new Literal(Previous().Literal);
            }
            if (Match(LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(RIGHT_PAREN, "Expect \")\" after expreession.");

                return new Grouping(expr);
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
                return Previous();
            }
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }
    }
}