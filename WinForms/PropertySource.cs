using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        /// <summary>
        /// Creates a property source from an instance, an instance type and a property member info. 
        /// The property will use either the [X]Changed event, where [X] is the name of the property, 
        /// or the <c>PropertyChanged</c> event if the class implements <see cref="INotifyPropertyChanged"/>, 
        /// to notify subscribers when the property changes. 
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="instance">An instance of the declaring class.</param>
        /// <param name="instanceType">The type of the declaring class.</param>
        /// <param name="memberInfo">A <see cref="PropertyInfo"/> that represents the class property.</param>
        /// <returns>The created property.</returns>
        internal static IPropertySource<T> FromProperty<T>(object instance, Type instanceType, PropertyInfo memberInfo)
        {
            var eventInfo = instanceType.GetEvent(memberInfo.Name + "Changed", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

            Func<Action, IDisposable> subscribe;

            if (eventInfo != null && eventInfo.EventHandlerType == typeof(EventHandler))
            {
                subscribe = observer =>
                {
                    EventHandler handler = (s, e) => observer();
                    eventInfo.AddEventHandler(instance, handler);
                    return Disposable.Create(() => eventInfo.RemoveEventHandler(instance, handler));
                };
            }
            else
            {
                var notifyPropertyChanged = instance as INotifyPropertyChanged;

                if (notifyPropertyChanged == null)
                    throw new Exception("This member cannot be observed.");

                subscribe = observer =>
                {
                    PropertyChangedEventHandler handler = (s, e) =>
                    {
                        if (e.PropertyName == memberInfo.Name)
                            observer();
                    };

                    notifyPropertyChanged.PropertyChanged += handler;
                    return Disposable.Create(() => notifyPropertyChanged.PropertyChanged -= handler);
                };
            }

            return Create(subscribe, () => (T)memberInfo.GetValue(instance));
        }

        /// <summary>
        /// Creates a property from an instance and a property name. 
        /// The property will use either the [X]Changed event, where [X] is the name of the property, 
        /// or the <c>PropertyChanged</c> event if the class implements <see cref="INotifyPropertyChanged"/>, 
        /// to notify subscribers when the property changes. 
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="instance">An instance of the declaring class.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> FromProperty<T>(object instance, string propertyName)
        {


            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = instance.GetType();
            return FromProperty<T>(instance, type, type.GetProperty(propertyName));
        }

        /// <summary>
        /// Creates a property source from a member access expression. 
        /// The property will use either the [X]Changed event, where [X] is the name of the property, 
        /// or the <c>PropertyChanged</c> event if the class implements <see cref="INotifyPropertyChanged"/>, 
        /// to notify subscribers when the property changes. 
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="memberAccessExpression">An member access expression from which the declaring class instance and the property name will be extracted.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> FromProperty<T>(Expression<Func<T>> memberAccessExpression)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");

            var propertyInfo = Utils.PropertyHelper.GetMemberAccessInfo(memberAccessExpression);
            return FromProperty<T>(propertyInfo.Instance, propertyInfo.MemberName);
        }
    }
}
