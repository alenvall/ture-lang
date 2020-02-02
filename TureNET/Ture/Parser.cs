using System;
using System.Collections.Generic;

using static Ture.TokenType;

namespace Ture
{
    public class Parser
    {
        private class ParseError : Exception { }

        private readonly IList<Token> tokens;
        private int current = 0;

        public Parser(IList<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            Expr expr;
            try
            {
                expr = Expression();
            }
            catch (ParseError)
            {
                return null;
            }

            return expr;
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

            while (Match(SLASH, STAR))
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
                Consume(RIGHT_PAREN, "Expected \")\" after expression");

                return new Grouping(expr);
            }

            throw Error(Peek(), "Expected expression");
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

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
                return Previous();
            }
            else
            {
                return Peek();
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

        private ParseError Error(Token token, string message)
        {
            Ture.Error(token, message);

            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case CLASS:
                    case FUNCTION:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}