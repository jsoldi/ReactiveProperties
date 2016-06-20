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

        public static IPropertySource<T> FromProperty<T>(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = instance.GetType();
            return FromProperty<T>(instance, type, type.GetProperty(propertyName));
        }

        public static IPropertySource<T> FromProperty<T>(Expression<Func<T>> memberAccessExpression)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");

            var propertyInfo = Utils.PropertyHelper.GetMemberAccessInfo(memberAccessExpression);
            return FromProperty<T>(propertyInfo.Instance, propertyInfo.MemberName);
        }
    }
}
