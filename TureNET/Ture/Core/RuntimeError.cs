using System;
using Ture.Models;

namespace Ture.Core
{
    public class RuntimeError : Exception
    {
        public readonly Token Token;

        public RuntimeError(Token token, string message) : base(message)
        {
            this.Token = token;
        }
    }
}
