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
        public static IProperty<TResult> SelectMany<TSource, TResult>(this IProperty<TSource> property, Func<TSource, IProperty<TResult>> selector)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (selector == null) throw new ArgumentNullException("selector");

            return Property.Create(PropertySource.SelectMany(property, selector), res => selector(property.Value).Value = res);
        }

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

        public static IDisposable BindTo<T>(this IPropertySource<T> source, IProperty<T> target)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            return source.Subscribe(val => target.Value = val);
        }

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
