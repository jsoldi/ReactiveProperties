using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public struct ChangeInfo<T>
    {
        public readonly T Old;
        public readonly T New;

        internal ChangeInfo(T oldValue, T newValue)
        {
            Old = oldValue;
            New = newValue;
        }
    }
}
