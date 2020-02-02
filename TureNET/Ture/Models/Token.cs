using System;

namespace Ture.Models
{
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly Object Literal;
        public readonly int LineNumber;

        public Token(TokenType type, string lexeme, Object literal, int lineNumber)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.LineNumber = lineNumber;
        }

        public override string ToString()
        {
            return Type + " " + Lexeme + " " + Literal;
        }
    }
}
