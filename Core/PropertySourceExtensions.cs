using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static IObservable<T> ToObservable<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Observable.Create<T>(
                observer => source.RawSubscribe(() => observer.OnNext(source.Value))
            );
        }

        public static IDisposable Subscribe<T>(this IPropertySource<T> source, Action<T> observer, IEqualityComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");
            if (comparer == null) throw new ArgumentNullException("comparer");

            return source.Eager().Distinct(comparer).RawSubscribe(() => observer(source.Value));
        }

        public static IDisposable Subscribe<T>(this IPropertySource<T> source, Action<T> observer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");

            return source.Eager().Distinct().RawSubscribe(() => observer(source.Value));
        }

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

        public static IDisposable SubscribeToChanges<T>(this IPropertySource<T> source, Action<ChangeInfo<T>> observer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (observer == null) throw new ArgumentNullException("observer");

            return source.SubscribeToChanges(observer, EqualityComparer<T>.Default);
        }

        public static IDisposable NotifyChangesAs<T>(this IPropertySource<T> source, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> onPropertyChanged)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (onPropertyChanged == null) throw new ArgumentNullException("onPropertyChanged");

            var eventArgs = new PropertyChangedEventArgs(Utils.PropertyHelper.GetMemberName(memberAccessExpression));
            return source.RawSubscribe(() => onPropertyChanged(eventArgs));
        }

        public static IDisposable MergeSubscribe<L, R>(this IPropertySource<L> left, IPropertySource<R> right, Action<L, R> action)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");

            var source = left.Merge(right, (l, r) => new { l, r });

            return source.Eager().RawSubscribe(() =>
            {
                var tuple = source.Value;
                action(tuple.l, tuple.r);
            });
        }

        public static IDisposable MergeSubscribe<L, M, R>(this IPropertySource<L> left, IPropertySource<M> middle, IPropertySource<R> right, Action<L, M, R> action)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (middle == null) throw new ArgumentNullException("middle");
            if (right == null) throw new ArgumentNullException("right");
            if (action == null) throw new ArgumentNullException("action");

            var source = left.Merge(middle, right, (l, m, r) => new { l, m, r });

            return source.Eager().RawSubscribe(() =>
            {
                var tuple = source.Value;
                action(tuple.l, tuple.m, tuple.r);
            });
        }
    }
}
