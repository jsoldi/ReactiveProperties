using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public static partial class Property
    {
        public static IProperty<T> FromValue<T>(T value, string name, Action<PropertyChangedEventArgs> notifyChanges, IEqualityComparer<T> comparer)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var args = new PropertyChangedEventArgs(name);
            return FromValue(value, _ => { }, _ => notifyChanges(args), comparer);
        }

        public static IProperty<T> FromValue<T>(T value, string name, Action<PropertyChangedEventArgs> notifyChanges)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");

            return FromValue(value, name, notifyChanges, EqualityComparer<T>.Default);
        }

        public static IProperty<T> FromValue<T>(T value, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> notifyChanges, IEqualityComparer<T> comparer)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var name = Utils.PropertyHelper.GetMemberName(memberAccessExpression);
            return FromValue(value, name, notifyChanges, comparer);
        }

        public static IProperty<T> FromValue<T>(T value, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> notifyChanges)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");

            return FromValue(value, memberAccessExpression, notifyChanges, EqualityComparer<T>.Default);
        }
    }
}
