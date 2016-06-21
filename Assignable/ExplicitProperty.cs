using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// A class that implements the <see cref="IProperty{T}"/> interface using explicitly given values.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    internal class ExplicitProperty<T> : IProperty<T>
    {
        private readonly IPropertySource<T> _PropertySource;
        private readonly Action<T> _Setter;

        public T Value
        {
            get { return _PropertySource.Value; }
            set { _Setter(value); }
        }

        public ExplicitProperty(IPropertySource<T> propertySource, Action<T> setter)
        {
            _PropertySource = propertySource;
            _Setter = setter;
        }

        public IDisposable RawSubscribe(Action rawObserver)
        {
            return _PropertySource.RawSubscribe(rawObserver);
        }
    }
}
