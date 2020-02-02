using System;
using System.Collections.Generic;
using System.Text;

namespace Ture.Core
{
    public class Return : Exception
    {
        public readonly object value;

        public Return(object value) : base(null, null)
        {
            this.value = value;
        }
    }
}
