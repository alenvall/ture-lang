using System;

namespace Ture
{
    class Token
    {
        readonly TokenType type;
        readonly string lexeme;
        readonly Object literal;
        readonly int lineNumber;

        public Token(TokenType type, string lexeme, Object literal, int lineNumber)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.lineNumber = lineNumber;
        }

        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}
