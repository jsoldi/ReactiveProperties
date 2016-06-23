using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        /// <summary>
        /// Creates an <see cref="IObservable{T}"/> from a property source.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source.</param>
        /// <returns>The created observable.</returns>
        /// <remarks>The created observable never calls OnCompleted.</remarks>
        public static IObservable<T> ToObservable<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Observable.Create<T>(
                observer => source.RawSubscribe(() => observer.OnNext(source.Value))
            );
        }

        /// <summary>
        /// Subscribes to the property source given a comparer that determines whether the value has changed or not. 
        /// The observer is notified at subscription time, and subsequently after every value change.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="observer">An action that will be receive the current value at subscription time and after every value change.</param>
        /// <param name="comparer">A compared that is used to determine whether the value has changed.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable Subscribe<T>(this IPropertySource<T> source, Action<T> observer, IEqualityComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");
            if (comparer == null) throw new ArgumentNullException("comparer");

            return source.Distinct(comparer).RawSubscribe(() => observer(source.Value));
        }

        /// <summary>
        /// Subscribes to the property source. The observer is first notified at subscription time, and subsequently after every value change.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="observer">An action that will be receive the current value at subscription time and after every value change.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable Subscribe<T>(this IPropertySource<T> source, Action<T> observer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");

            return source.Distinct().RawSubscribe(() => observer(source.Value));
        }

        /// <summary>
        /// Subscribes to the property source given an action that receives a <see cref="ChangeInfo{T}"/> cotaining the old a new values, and comparer that determines whether the value has changed or not. 
        /// The observer is invoked at subscription time with <c>default{T}</c> as the old value, and then subsequently invoked after every change.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="observer">An action that will be receive a <see cref="ChangeInfo{T}"/> containing the new and old values at subscription time and after every value change.</param>
        /// <param name="comparer">A comparer that is used to determine whether the value has changed.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable SubscribeToChanges<T>(this IPropertySource<T> source, Action<ChangeInfo<T>> observer, IEqualityComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var oldValue = default(T);

            return source.Subscribe(value =>
            {
                observer(new ChangeInfo<T>(oldValue, value));
                oldValue = value;
            },
            comparer);
        }

        /// <summary>
        /// Subscribes to the property source given an action that receives a <see cref="ChangeInfo{T}"/> cotaining the old a new values.
        /// The observer is invoked at subscription time with <c>default{T}</c> as the old value, and then subsequently invoked after every change.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="observer">An action that will be receive a <see cref="ChangeInfo{T}"/> containing the new and old values at subscription time and after every value change.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable SubscribeToChanges<T>(this IPropertySource<T> source, Action<ChangeInfo<T>> observer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");

            return source.SubscribeToChanges(observer, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Merges two property sources and notifies whenever any of the property values changes using a comparer for each property.
        /// </summary>
        /// <typeparam name="L">The type of the left property source.</typeparam>
        /// <typeparam name="R">The type of the right property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="action">The action to be invoked when any of the properties change.</param>
        /// <param name="leftComparer">A comparer that is used to determine whether the left value has changed.</param>
        /// <param name="rightComparer">A comparer that is used to determine whether the right value has changed.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable MergeSubscribe<L, R>(this IPropertySource<L> left, IPropertySource<R> right, Action<L, R> action, IEqualityComparer<L> leftComparer, IEqualityComparer<R> rightComparer)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");
            if (leftComparer == null) throw new ArgumentNullException("leftComparer");
            if (rightComparer == null) throw new ArgumentNullException("rightComparer");

            var source = left.Merge(right, (l, r) => new MergedPropertyValue<L, R>(l, r));
            return source.Subscribe(merged => action(merged.Left, merged.Right), new MergedPropertyValueComparer<L, R>(leftComparer, rightComparer));
        }

        /// <summary>
        /// Merges two property sources and notifies whenever any of the property values changes.
        /// </summary>
        /// <typeparam name="L">The type of the left property source.</typeparam>
        /// <typeparam name="R">The type of the right property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="action">The action to be invoked when any of the properties change.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable MergeSubscribe<L, R>(this IPropertySource<L> left, IPropertySource<R> right, Action<L, R> action)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");

            var source = left.Merge(right, (l, r) => new MergedPropertyValue<L, R>(l, r));
            return source.Subscribe(merged => action(merged.Left, merged.Right), new MergedPropertyValueComparer<L, R>(EqualityComparer<L>.Default, EqualityComparer<R>.Default));
        }

        /// <summary>
        /// Merges three property sources and notifies whenever any of the property values changes using a comparer for each property.
        /// </summary>
        /// <typeparam name="L">The type of the left property source.</typeparam>
        /// <typeparam name="M">The type of the middle property source.</typeparam>
        /// <typeparam name="R">The type of the right property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="middle">The middle property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="action">The action to be invoked when any of the properties change.</param>
        /// <param name="leftComparer">A comparer that is used to determine whether the left value has changed.</param>
        /// <param name="middleComparer">A comparer that is used to determine whether the middle value has changed.</param>
        /// <param name="rightComparer">A comparer that is used to determine whether the right value has changed.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable MergeSubscribe<L, M, R>(this IPropertySource<L> left, IPropertySource<M> middle, IPropertySource<R> right, Action<L, M, R> action, IEqualityComparer<L> leftComparer, IEqualityComparer<M> middleComparer, IEqualityComparer<R> rightComparer)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (middle == null) throw new ArgumentNullException("middle");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");
            if (leftComparer == null) throw new ArgumentNullException("leftComparer");
            if (middleComparer == null) throw new ArgumentNullException("middleComparer");
            if (rightComparer == null) throw new ArgumentNullException("rightComparer");

            var source = left.Merge(middle, right, (l, m, r) => new MergedPropertyValue<L, M, R>(l, m, r));
            return source.Subscribe(merged => action(merged.Left, merged.Middle, merged.Right), new MergedPropertyValueComparer<L, M, R>(leftComparer, middleComparer, rightComparer));
        }

        /// <summary>
        /// Merges two property sources and notifies whenever any of the property values changes.
        /// </summary>
        /// <typeparam name="L">The type of the left property source.</typeparam>
        /// <typeparam name="M">The type of the middle property source.</typeparam>
        /// <typeparam name="R">The type of the right property source.</typeparam>
        /// <param name="left">The left property source.</param>
        /// <param name="middle">The middle property source.</param>
        /// <param name="right">The right property source.</param>
        /// <param name="action">The action to be invoked when any of the properties change.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable MergeSubscribe<L, M, R>(this IPropertySource<L> left, IPropertySource<M> middle, IPropertySource<R> right, Action<L, M, R> action)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (middle == null) throw new ArgumentNullException("middle");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");

            var source = left.Merge(middle, right, (l, m, r) => new MergedPropertyValue<L, M, R>(l, m, r));
            return source.Subscribe(merged => action(merged.Left, merged.Middle, merged.Right), new MergedPropertyValueComparer<L, M, R>(EqualityComparer<L>.Default, EqualityComparer<M>.Default, EqualityComparer<R>.Default));
        }
    }
}
