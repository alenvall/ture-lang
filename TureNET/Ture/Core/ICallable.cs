using System.Collections.Generic;

namespace Ture.Core
{
    public interface ICallable
    {
        public int Arity();
        public object Call(Interpreter interpreter, IList<object> arguments);
    }
}
