using System;
using System.Collections.Generic;

namespace Ture.Core.Native
{
    public class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, IList<object> arguments)
        {
            return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

}
