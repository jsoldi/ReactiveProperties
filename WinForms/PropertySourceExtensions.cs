using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties.WinForms
{
    public static class PropertySourceExtensions
    {
        /// <summary>
        /// Invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="propertyName">The name to be passed passed in the <see cref="PropertyChangedEventArgs"/> argument.</param>
        /// <param name="onPropertyChanged">An action that takes a <see cref="PropertyChangedEventArgs"/> argument.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable NotifyChangesAs<T>(this IPropertySource<T> source, string propertyName, Action<PropertyChangedEventArgs> onPropertyChanged)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (onPropertyChanged == null) throw new ArgumentNullException("onPropertyChanged");

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            return source.RawSubscribe(() => onPropertyChanged(eventArgs));
        }

        /// <summary>
        /// Invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The property source to subscribe to.</param>
        /// <param name="memberAccessExpression">A member access expression from which the name to be passed passed in the <see cref="PropertyChangedEventArgs"/> argument will be taken.</param>
        /// <param name="onPropertyChanged">An action that takes a <see cref="PropertyChangedEventArgs"/> argument.</param>
        /// <returns>A disposable that must be disposed to end the subscription and stop receiving notifications.</returns>
        public static IDisposable NotifyChangesAs<T>(this IPropertySource<T> source, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> onPropertyChanged)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (onPropertyChanged == null) throw new ArgumentNullException("onPropertyChanged");

            var eventArgs = new PropertyChangedEventArgs(Utils.PropertyHelper.GetMemberName(memberAccessExpression));
            return source.RawSubscribe(() => onPropertyChanged(eventArgs));
        }
    }
}
