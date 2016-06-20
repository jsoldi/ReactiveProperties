using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Disposables;

namespace ReactiveProperties
{
    public static partial class Property
    {
        public static IProperty<T> Create<T>(IPropertySource<T> source, Action<T> setValue)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (setValue == null) throw new ArgumentNullException("setValue");

            return new ExplicitProperty<T>(source, setValue);
        }

        public static IProperty<T> FromValue<T>(T value, Action<SettingData<T>> setValue)
        {
            if (setValue == null) throw new ArgumentNullException("setValue");

            Action notify;

            return Create<T>(
                PropertySource.Create(() => value, out notify),
                desiredValue =>
                {
                    setValue(new SettingData<T>(value, desiredValue, actualValue =>
                    {
                        value = actualValue;
                        notify();
                    }));
                }
            );
        }

        public static IProperty<T> FromValue<T>(T value, Action<T> beforeChange, Action<T> afterChange, IEqualityComparer<T> comparer)
        {
            if (beforeChange == null) throw new ArgumentNullException("beforeChange");
            if (afterChange == null) throw new ArgumentNullException("afterChange");
            if (comparer == null) throw new ArgumentNullException("comparer");

            Action notify;

            return Create<T>(
                PropertySource.Create(() => value, out notify),
                newValue =>
                {
                    if (!comparer.Equals(value, newValue))
                    {
                        beforeChange(value);
                        value = newValue;
                        afterChange(value);
                        notify();
                    }
                }
            );
        }

        public static IProperty<T> FromValue<T>(T value, Action<T> beforeChange, Action<T> afterChange)
        {
            if (beforeChange == null) throw new ArgumentNullException("beforeChange");
            if (afterChange == null) throw new ArgumentNullException("afterChange");

            return FromValue(value, beforeChange, afterChange, EqualityComparer<T>.Default);
        }

        public static IProperty<T> FromValue<T>(T value, IEqualityComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            Action notify;

            return Create(
                PropertySource.Create(() => value, out notify),
                newValue =>
                {
                    if (!comparer.Equals(value, newValue))
                    {
                        value = newValue;
                        notify();
                    }
                }
            );
        }

        public static IProperty<T> FromValue<T>(T value = default(T))
        {
            return FromValue(value, EqualityComparer<T>.Default);
        }

        public static IProperty<T> FromProperty<T>(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var type = instance.GetType();
            var memberInfo = type.GetProperty(propertyName);
            return Property.Create(PropertySource.FromProperty<T>(instance, type, memberInfo), val => memberInfo.SetValue(instance, val));
        }

        public static IProperty<T> FromProperty<T>(Expression<Func<T>> memberAccessExpression)
        {
            if (memberAccessExpression == null) throw new ArgumentNullException("memberAccessExpression");

            var info = Utils.PropertyHelper.GetMemberAccessInfo(memberAccessExpression);
            return FromProperty<T>(info.Instance, info.MemberName);
        }
    }
}
