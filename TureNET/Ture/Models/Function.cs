using System.Collections.Generic;
using Ture.Core;

namespace Ture.Models
{
    public class Function : ICallable
    {
        private readonly Stmt.Function declaration;

        public Function(Stmt.Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, IList<object> arguments)
        {
            Environment environment = new Environment(interpreter.Globals);

            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.value;
            }

            return null;
        }

        public override string ToString()
        {
            return "<fn " + declaration.Name.Lexeme + ">";
        }
    }
}
