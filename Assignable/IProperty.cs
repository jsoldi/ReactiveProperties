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
        /// <summary>
        /// Gets or sets the value of the property. The implementer is responsible for notifying the observer whenever the value changes.
        /// </summary>
        new T Value { get; set; }
    }
}
