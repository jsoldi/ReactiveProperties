using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public static partial class Property
    {
        /// <summary>
        /// Given a property and a selector that returns a property, calls the selector passing the original property value, 
        /// at subscription time and every time the original property changes, and creates a property whose value gets and sets 
        /// the value of the property source returned by the selector. 
        /// </summary>
        /// <typeparam name="TSource">The type of the original property.</typeparam>
        /// <typeparam name="TResult">The type of the created property.</typeparam>
        /// <param name="property">The original property.</param>
        /// <param name="selector">A function that receives the value of the original property at subscription time and every time it changes, and returns a property.</param>
        /// <returns>The created property.</returns>
        public static IProperty<TResult> SelectMany<TSource, TResult>(this IProperty<TSource> property, Func<TSource, IProperty<TResult>> selector)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (selector == null) throw new ArgumentNullException("selector");

            return Property.Create(PropertySource.SelectMany(property, selector), res => selector(property.Value).Value = res);
        }

        /// <summary>
        /// Creates a new property given an original property and two selectors that map between the original and created property values.
        /// </summary>
        /// <typeparam name="TSource">The type of the original property.</typeparam>
        /// <typeparam name="TResult">The type of the created property.</typeparam>
        /// <param name="property">The original property.</param>
        /// <param name="selector">A selector that maps from the original property values to the created property values.</param>
        /// <param name="backSelector">A selector used when the created property is assigned to, that maps from the created property values to the created property values.</param>
        /// <returns>The created property.</returns>
        public static IProperty<TResult> Select<TSource, TResult>(this IProperty<TSource> property, Func<TSource, TResult> selector, Func<TResult, TSource> backSelector)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (selector == null) throw new ArgumentNullException("selector");
            if (backSelector == null) throw new ArgumentNullException("backSelector");

            return Property.Create(
                PropertySource.SelectMany(property, val => PropertySource.Return(selector(val))),
                res => property.Value = backSelector(res)
            );
        }

        /// <summary>
        /// Subscribes to the source property and assigns the value to the target property every time it changes.
        /// </summary>
        /// <typeparam name="T">The type of the source and target properties.</typeparam>
        /// <param name="source">The type of the source property.</param>
        /// <param name="target">The type of the target property.</param>
        /// <returns>A disposable that must be disposed to end the subscription and release the binding.</returns>
        public static IDisposable BindTo<T>(this IPropertySource<T> source, IProperty<T> target)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            return source.Subscribe(val => target.Value = val);
        }

        /// <summary>
        /// Binds two properties to assign their value to each other anytime one of them change.
        /// </summary>
        /// <typeparam name="T">The type of the first and second properties.</typeparam>
        /// <param name="first">The first property.</param>
        /// <param name="second">The second property.</param>
        /// <returns>A disposable that must be disposed to end the subscription and release the binding.</returns>
        /// <remarks>The first property will be bound to the second before binding the second to the first.</remarks>
        public static IDisposable TwoWayBind<T>(this IProperty<T> first, IProperty<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            var a = first.BindTo(second);
            var b = second.BindTo(first);

            return Disposable.Create(() =>
            {
                a.Dispose();
                b.Dispose();
            });
        }
    }
}
