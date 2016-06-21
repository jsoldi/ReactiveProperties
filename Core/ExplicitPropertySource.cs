using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// A class that implements the <see cref="IPropertySource{T}"/> interface using explicitly given values.
    /// </summary>
    /// <typeparam name="T">The type of the property source.</typeparam>
    internal class ExplicitPropertySource<T> : IPropertySource<T>
    {
        private readonly Func<T> _GetValue;
        private readonly Func<Action, IDisposable> _RawSubscribe;

        public T Value
        {
            get { return _GetValue(); }
        }

        internal ExplicitPropertySource(Func<Action, IDisposable> rawSubscribe, Func<T> getValue)
        {
            _RawSubscribe = rawSubscribe;
            _GetValue = getValue;
        }

        public IDisposable RawSubscribe(Action rawObserver)
        {
            return _RawSubscribe(rawObserver);
        }
    }
}
