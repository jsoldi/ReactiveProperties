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
        /// <summary>
        /// Creates a property from an instance and a property name. 
        /// The property will use either the [X]Changed event, where [X] is the name of the property, 
        /// or the <c>PropertyChanged</c> event if the class implements <see cref="INotifyPropertyChanged"/>, 
        /// to notify subscribers when the property changes. Setting the value of the property will 
        /// set the value of the instance's property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="instance">An instance of the declaring class.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The created property.</returns>
        public static IProperty<T> FromProperty<T>(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = instance.GetType();
            var memberInfo = type.GetProperty(propertyName);
            return Property.Create(PropertySource.FromProperty<T>(instance, type, memberInfo), val => memberInfo.SetValue(instance, val));
        }

        /// <summary>
        /// Creates a property from a member access expression.
        /// The property will use either the [X]Changed event, where [X] is the name of the property, 
        /// or the <c>PropertyChanged</c> event if the class implements <see cref="INotifyPropertyChanged"/>, 
        /// to notify subscribers when the property changes. Setting the value of the property will 
        /// set the value of the instance's property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="memberAccessExpression">An member access expression from which the declaring class instance and the property name will be extracted.</param>
        /// <returns>The created property.</returns>
        public static IProperty<T> FromProperty<T>(Expression<Func<T>> memberAccessExpression)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");

            var info = Utils.PropertyHelper.GetMemberAccessInfo(memberAccessExpression);
            return FromProperty<T>(info.Instance, info.MemberName);
        }

        /// <summary>
        /// Creates a property that invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="name">The name of the property to be passed to the <c>OnPropertyChanged</c> method in the <see cref="PropertyChangedEventArgs"/> argument.</param>
        /// <param name="notifyChanges">An <c>OnPropertyChanged</c> method.</param>
        /// <param name="comparer">A comparer that determines whether the property value has changed.</param>
        /// <returns>The created property value.</returns>
        public static IProperty<T> FromValue<T>(T value, string name, Action<PropertyChangedEventArgs> notifyChanges, IEqualityComparer<T> comparer)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var args = new PropertyChangedEventArgs(name);
            return FromValue(value, _ => { }, _ => notifyChanges(args), comparer);
        }

        /// <summary>
        /// Creates a property that invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="name">The name of the property to be passed to the <c>OnPropertyChanged</c> method in the <see cref="PropertyChangedEventArgs"/> argument.</param>
        /// <param name="notifyChanges">An <c>OnPropertyChanged</c> method.</param>
        /// <returns>The created property value.</returns>
        public static IProperty<T> FromValue<T>(T value, string name, Action<PropertyChangedEventArgs> notifyChanges)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");

            return FromValue(value, name, notifyChanges, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Creates a property that invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="memberAccessExpression">An member access expression from which the declaring class instance and the property name will be extracted.</param>
        /// <param name="notifyChanges">An <c>OnPropertyChanged</c> method.</param>
        /// <param name="comparer">A comparer that determines whether the property value has changed.</param>
        /// <returns>The created property value.</returns>
        public static IProperty<T> FromValue<T>(T value, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> notifyChanges, IEqualityComparer<T> comparer)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var name = Utils.PropertyHelper.GetMemberName(memberAccessExpression);
            return FromValue(value, name, notifyChanges, comparer);
        }

        /// <summary>
        /// Creates a property that invokes <c>OnPropertyChanged</c> when the property value changes, for classes that implement <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="memberAccessExpression">An member access expression from which the declaring class instance and the property name will be extracted.</param>
        /// <param name="notifyChanges">An <c>OnPropertyChanged</c> method.</param>
        /// <returns>The created property value.</returns>
        public static IProperty<T> FromValue<T>(T value, Expression<Func<T>> memberAccessExpression, Action<PropertyChangedEventArgs> notifyChanges)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");
            if (notifyChanges == null) throw new ArgumentNullException("notifyChanges");

            return FromValue(value, memberAccessExpression, notifyChanges, EqualityComparer<T>.Default);
        }
    }
}
