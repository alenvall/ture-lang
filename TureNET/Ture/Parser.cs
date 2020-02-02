using System;
using System.Collections.Generic;
using Ture.Core;
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
                if (Match(FUNCTION))
                {
                    return Function("function");
                }

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
            if (Match(FOR))
            {
                return ForStatement();
            }

            if (Match(IF))
            {
                return IfStatement();
            }

            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(RETURN))
            {
                return ReturnStatement();
            }

            if (Match(WHILE))
            {
                return WhileStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }

            return ExpressionStatement();
        }


        private Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expected \"(\" after \"for\".");

            Stmt initializer;

            if (Match(SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expr condition = null;

            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }
            Consume(SEMICOLON, "Expected \";\" after loop condition");

            Expr increment = null;

            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(RIGHT_PAREN, "Expected \")\" after clauses");

            Stmt body = Statement();

            if (increment != null)
            {
                body = new Stmt.Block(new List<Stmt>() { body, new Stmt.Expression(increment) });
            }

            if (condition == null)
            {
                condition = new Expr.Literal(true);
            }

            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new List<Stmt>() { initializer, body });
            }

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expected \"(\" after \"if\".");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expected \")\" after condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(SEMICOLON, "Expected \";\" after value.");
            return new Stmt.Print(value);
        }

        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Expr value = null;
            if (!Check(SEMICOLON))
            {
                value = Expression();
            }

            Consume(SEMICOLON, "Expect \";\" after return value");
            return new Stmt.Return(keyword, value);
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(IDENTIFIER, "Expected variable name");

            Expr initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(SEMICOLON, "Expected \";\" after variable declaration");

            return new Stmt.Var(name, initializer);
        }

        private Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expected \"(\" after \"while\".");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expected \")\" after loop condition");
            Stmt body = Statement();

            return new Stmt.While(condition, body);
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expected \";\" after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function Function(string kind)
        {
            Token name = Consume(IDENTIFIER, "Expected \"" + kind + " name");

            Consume(LEFT_PAREN, "Expected \"(\" after " + name + " name");

            IList<Token> parameters = new List<Token>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count > Constants.MAX_COUNT_ARGUMENTS)
                    {
                        Error(Peek(), "Cannot have more than 255 parameters");
                    }

                    parameters.Add(Consume(IDENTIFIER, "Expected paramter name"));
                } while (Match(COMMA));
            }

            Consume(RIGHT_PAREN, "Expected \")\" after parameters");
            Consume(LEFT_BRACE, "Expected \"{\" before " + kind + " body");

            ICollection<Stmt> body = Block();

            return new Stmt.Function(name, parameters, body);
        }

        private ICollection<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(RIGHT_BRACE, "Expected \"}\" after block");
            return statements;
        }

        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target");
            }

            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();

            while (Match(OR))
            {
                Token oper = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, oper, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality();

            while (Match(AND))
            {
                Token oper = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, oper, right);
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

            return Call();
        }

        private Expr FinishCall(Expr callee)
        {
            ICollection<Expr> arguments = new List<Expr>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= Constants.MAX_COUNT_ARGUMENTS)
                    {
                        Error(Peek(), "Cannot have more than 255 arguments");
                    }
                    arguments.Add(Expression());
                } while (Match(COMMA));
            }

            Token paren = Consume(RIGHT_PAREN, "Expected \")\" after arguments");

            return new Expr.Call(callee, paren, arguments);
        }

        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
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