using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// Represents a property that can be observed and has a read and write value.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    public interface IProperty<T> : IPropertySource<T>
    {
        new T Value { get; set; }
    }
}
