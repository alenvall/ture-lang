﻿using System;
using System.Collections.Generic;
using Ture.Models;

using static Ture.Models.TokenType;

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

        public ICollection<Stmt> Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(VAR))
                {
                    return VarDeclaration();
                }
                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt Statement()
        {
            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }

            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(SEMICOLON, "Expected \";\" after value.");
            return new Stmt.Print(value);
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(IDENTIFIER, "Expected variable name.");

            Expr initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expected \";\" after variabled declaration");

            return new Stmt.Var(name, initializer);
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expected \";\" after expression.");
            return new Stmt.Expression(expr);
        }

        private ICollection<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(RIGHT_BRACE, "Expected \"}\" after block.");
            return statements;
        }

        private Expr Assignment()
        {
            Expr expr = Equality();

            if (Match(EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparsion();

            while (Match(EXCLAMATION_EQUAL, EQUAL_EQUAL))
            {
                Token oper = Previous();
                Expr right = Comparsion();
                expr = new Expr.Binary(expr, oper, right);
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
                expr = new Expr.Binary(expr, oper, right);
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
                expr = new Expr.Binary(expr, oper, right);
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
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(EXCLAMATION, MINUS))
            {
                Token oper = Previous();
                Expr right = Unary();
                return new Expr.Unary(oper, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Expr.Literal(false);
            }

            if (Match(TRUE))
            {
                return new Expr.Literal(true);
            }

            if (Match(NULL))
            {
                return new Expr.Literal(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(RIGHT_PAREN, "Expected \")\" after expression");

                return new Expr.Grouping(expr);
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