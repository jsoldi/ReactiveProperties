using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        /// <summary>
        /// Creates an immutable property source with the given value. This is the return operator of the property source monad.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="value">The value of the property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Return<T>(T value)
        {
            return PropertySource.Create(observer => Disposable.Empty, () => value);
        }

        /// <summary>
        /// Given a property source, creates a property source that notifies only when the original property source's value changes according to the provided equality comparer.
        /// Observers given to this property's <c>RawSubscribe</c> method are notified during subscription.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <param name="comparer">The comparer that determines whether the property source value has changed.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Distinct<T>(this IPropertySource<T> source, IEqualityComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (comparer == null) throw new ArgumentNullException("comparer");

            return PropertySource.Create(
                action =>
                {
                    T cachedValue = default(T);

                    Action<T> sendAndCache = value =>
                    {
                        action();
                        cachedValue = value;
                    };

                    Action sendIfChanged = () =>
                    {
                        var value = source.Value;

                        if (!comparer.Equals(value, cachedValue))
                            sendAndCache(value);
                    };

                    sendAndCache(source.Value);

                    return source.Lazy().RawSubscribe(() =>
                    {
                        sendIfChanged();
                    });
                },
                () => source.Value
            );
        }

        /// <summary>
        /// Given a property source, creates a property source that notifies only when the original property source's value changes.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Distinct<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Distinct(EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Given a property source, creates a property source that notifies when <c>RawSubscribe</c> is called.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Eager<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return PropertySource.Create(
                action =>
                {
                    action();
                    return source.Lazy().RawSubscribe(action);
                },
                () => source.Value
            );
        }

        /// <summary>
        /// Given a property source, creates a property source that does not notify when <c>RawSubscribe</c> is called.
        /// This property source will still notify when the <see cref="PropertySource.Subscribe"/> extension method is called.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Lazy<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return PropertySource.Create(
                action =>
                {
                    Action send = () => { };
                    var subscription = source.RawSubscribe(() => send());
                    send = action;
                    return subscription;
                },
                () => source.Value
            );
        }

        /// <summary>
        /// Given a property source and a selector that returns a property source, calls the selector passing the source's value, 
        /// at subscription time and every time the original source changes, and creates a property source whose value is the value 
        /// of the property source returned by the selector. This is the bind operator of the property source monad.
        /// </summary>
        /// <typeparam name="TSource">The type of the original property soruce.</typeparam>
        /// <typeparam name="TResult">The type of the created property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <param name="selector">A function that receives the value of the original source at subscription time and every time it changes, and returns a property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<TResult> SelectMany<TSource, TResult>(this IPropertySource<TSource> source, Func<TSource, IPropertySource<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return PropertySource.Create(
                observer =>
                {
                    IDisposable rightSubscription = Disposable.Empty;

                    Func<TSource, IPropertySource<TResult>> reattachRight = leftValue =>
                    {
                        rightSubscription.Dispose();
                        var rightSource = selector(leftValue);
                        rightSubscription = rightSource.Lazy().RawSubscribe(observer);
                        return rightSource;
                    };

                    IDisposable leftSubscription = source.Lazy().RawSubscribe(() =>
                    {
                        reattachRight(source.Value);
                        observer();
                    });

                    reattachRight(source.Value);

                    return Disposable.Create(() =>
                    {
                        leftSubscription.Dispose();
                        rightSubscription.Dispose();
                    });
                },
                () => selector(source.Value).Value
            );
        }

        /// <summary>
        /// Given a property source, a property source selector and a result selector, calls the property source selector passing the source's value, and
        /// the result selector passing the source's value and the value of the property source selector's result, at subscription time and every time the 
        /// original source changes, and creates a property whose value is the value returned by the result selector.
        /// </summary>
        /// <typeparam name="TSource">The type of the original property soruce.</typeparam>
        /// <typeparam name="TCollection">The type of the property source returned by <see cref="propertySourceSelector"/>.</typeparam>
        /// <typeparam name="TResult">The type of the created property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <param name="propertySourceSelector">A function that receives the value of the original source at subscription time and every time it changes, and returns a property source.</param>
        /// <param name="resultSelector">A selector that will receive each value from the original property source and the property source returned by <see cref="propertySourceSelector"/> and returns a new value.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<TResult> SelectMany<TSource, TCollection, TResult>(
            this IPropertySource<TSource> source, 
            Func<TSource, IPropertySource<TCollection>> propertySourceSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            return source.SelectMany(tSource =>
                propertySourceSelector(tSource).Select(tCollection => resultSelector(tSource, tCollection))
            );
        }

        /// <summary>
        /// Creates a new property source given an original property source and a selector that maps each value taken by the original source property into a new value.
        /// </summary>
        /// <typeparam name="TSource">The type of the original source.</typeparam>
        /// <typeparam name="TResult">The type of the created property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <param name="selector">The selector that maps the original source values into new values.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<TResult> Select<TSource, TResult>(this IPropertySource<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return source.SelectMany(value => PropertySource.Return(selector(value)));
        }

        /// <summary>
        /// Casts the values taken by a property source to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to case the original source values to.</typeparam>
        /// <param name="source">The orignal property source.</param>
        /// <returns>A property source whose value is the result of casting the original property source.</returns>
        public static IPropertySource<T> Cast<T>(this IPropertySource<object> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Select(obj => (T)obj);
        }

        /// <summary>
        /// Casts the values taken by a property source to the specified type, or set the resulting property source's value to null if casting isn't possible.
        /// </summary>
        /// <typeparam name="T">The type to case the original source values to.</typeparam>
        /// <param name="source">The orignal property source.</param>
        /// <returns>A property source whose value is the result of casting the original property source, or null if casting isn't possible.</returns>
        public static IPropertySource<T> As<T>(this IPropertySource<object> source)
            where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Select(obj => obj as T);
        }

        /// <summary>
        /// Merges two property sources into a single one using a selector that takes values from both sources and returns another value.
        /// </summary>
        /// <typeparam name="TLeft">The type of the left property source.</typeparam>
        /// <typeparam name="TRight">The type fo the right property source.</typeparam>
        /// <typeparam name="TResult">The type of the selector's result and the created property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="selector">A function that is called every time the left or right property sources change and returns another value.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<TResult> Merge<TLeft, TRight, TResult>(this IPropertySource<TLeft> left, IPropertySource<TRight> right, Func<TLeft, TRight, TResult> selector)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (selector == null) throw new ArgumentNullException("selector");

            return left.SelectMany(leftValue =>
                right.Select(rightValue => selector(leftValue, rightValue))
            );
        }

        /// <summary>
        /// Merges three property sources into a single one using a selector that takes values from all three sources and returns another value.
        /// </summary>
        /// <typeparam name="TLeft">The type of the left property source.</typeparam>
        /// <typeparam name="TMiddle">The type fo the middle property source.</typeparam>
        /// <typeparam name="TRight">The type fo the right property source.</typeparam>
        /// <typeparam name="TResult">The type of the selector's result and the created property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="middle">The middle property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="selector">A function that is called every time the left, middle or right property sources change and returns another value.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<TResult> Merge<TLeft, TMiddle, TRight, TResult>(this IPropertySource<TLeft> left, IPropertySource<TMiddle> middle, IPropertySource<TRight> right, Func<TLeft, TMiddle, TRight, TResult> selector)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (middle == null) throw new ArgumentNullException("middle");
            if (right == null) throw new ArgumentNullException("right");
            if (selector == null) throw new ArgumentNullException("selector");

            return left.SelectMany(leftValue =>
                middle.SelectMany(middleValue =>
                    right.Select(rightValue => selector(leftValue, middleValue, rightValue))
                )
            );
        }

        /// <summary>
        /// Creates a property source whose value is the result of applying the "and" logical operator to the left and right property sources.
        /// </summary>
        /// <param name="left">The left property source.</param>
        /// <param name="right">The right property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<bool> And(this IPropertySource<bool> left, IPropertySource<bool> right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            return left.Merge(right, (l, r) => l && r).Distinct();
        }

        /// <summary>
        /// Creates a property source whose value is the result of applying the "or" logical operator to the left and right property sources.
        /// </summary>
        /// <param name="left">The left property source.</param>
        /// <param name="right">The right property source.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<bool> Or(this IPropertySource<bool> left, IPropertySource<bool> right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            return left.Merge(right, (l, r) => l || r).Distinct();
        }
    }
}
