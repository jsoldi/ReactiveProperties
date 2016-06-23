using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// Represents a property that can be observed and has a readonly value.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    public interface IPropertySource<out T>
    {
        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Starts notifying the given observer when the property value changes, and returns a disposable that must be called when the subscription is not needed anymore.
        /// </summary>
        /// <param name="rawObserver">An action that must be invoked whenever the property value changes.</param>
        /// <returns>A disposable that must be called when the subscription is not needed anymore.</returns>
        /// <remarks>
        /// Whether to notify the observer when <c>RawSubscribe</c> is called is up to the implementer, because 
        /// this behavior is overridden in the <see cref="PropertySource.Subscribe"/> extension method and can be changed 
        /// with the <see cref="PropertySource.Lazy"/> and <see cref="PropertySource.Eager"/> extension methods.
        /// </remarks>
        IDisposable RawSubscribe(Action rawObserver);
    }
}
