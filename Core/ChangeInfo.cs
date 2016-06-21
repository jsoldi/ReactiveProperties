using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// Holds the old a new values when subcribed to a property source using any of the <c>PropertySource.SubscribeToChanges</c> methods.
    /// </summary>
    /// <typeparam name="T">The type of the property source.</typeparam>
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
