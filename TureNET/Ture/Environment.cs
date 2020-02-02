using System;
using System.Collections.Generic;
using System.Text;
using Ture.Core;
using Ture.Models;

namespace Ture
{
    public class Environment
    {
        private readonly IDictionary<string, object> values = new Dictionary<string, object>();
        private readonly Environment enclosing;

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (enclosing != null)
            {
                return enclosing.Get(name);
            }

            throw new RuntimeError(name, "Undefined variable \"" + name.Lexeme + "\".");
        }

        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable \"" + name.Lexeme + "\".");
        }
    }
}
