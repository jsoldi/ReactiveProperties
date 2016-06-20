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
        public static IPropertySource<T> Return<T>(T value)
        {
            return PropertySource.Create(observer => Disposable.Empty, () => value);
        }

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

                    Action observer = null;

                    observer = () =>
                    {
                        observer = sendIfChanged;
                        sendAndCache(source.Value);
                    };

                    return source.RawSubscribe(() =>
                    {
                        observer();
                    });
                },
                () => source.Value
            );
        }

        public static IPropertySource<T> Distinct<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Distinct(EqualityComparer<T>.Default);
        }

        public static IPropertySource<T> Eager<T>(this IPropertySource<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return PropertySource.Create(
                action =>
                {
                    action();
                    return source.RawSubscribe(action);
                },
                () => source.Value
            );
        }

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

        public static IPropertySource<TResult> Select<TSource, TResult>(this IPropertySource<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return source.SelectMany(value => PropertySource.Return(selector(value)));
        }

        public static IPropertySource<T> Cast<T>(this IPropertySource<object> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Select(obj => (T)obj);
        }

        public static IPropertySource<T> As<T>(this IPropertySource<object> source)
            where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Select(obj => obj as T);
        }

        public static IPropertySource<TResult> Merge<TLeft, TRight, TResult>(this IPropertySource<TLeft> left, IPropertySource<TRight> right, Func<TLeft, TRight, TResult> selector)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (selector == null) throw new ArgumentNullException("selector");

            return left.SelectMany(leftValue =>
                right.Select(rightValue => selector(leftValue, rightValue))
            );
        }

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

        public static IPropertySource<bool> And(this IPropertySource<bool> left, IPropertySource<bool> right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            return left.Merge(right, (l, r) => l && r).Distinct();
        }

        public static IPropertySource<bool> Or(this IPropertySource<bool> left, IPropertySource<bool> right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            return left.Merge(right, (l, r) => l || r).Distinct();
        }
    }
}
