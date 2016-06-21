using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Disposables;

namespace ReactiveProperties
{
    public static partial class Property
    {
        /// <summary>
        /// Creates a property given a property source and a value setter.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="source">The property source.</param>
        /// <param name="setValue">The value setter.</param>
        /// <returns>The created property.</returns>
        public static IProperty<T> Create<T>(IPropertySource<T> source, Action<T> setValue)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (setValue == null) throw new ArgumentNullException("setValue");

            return new ExplicitProperty<T>(source, setValue);
        }

        /// <summary>
        /// Creates a property given an initial value and an action that processes a <see cref="SettingData{T}"/> to customize the property value setting operation.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="setValue">An action that receives a <see cref="SettingData{T}"/> responsable for setting the property value.</param>
        /// <returns>The created property.</returns>
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

        /// <summary>
        /// Creates a property given an initial value and notifies the given actions before and after the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the created property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="beforeChange">An action that is invoked before the property value changes.</param>
        /// <param name="afterChange">An action that is invoked after the property of the value changes.</param>
        /// <param name="comparer">A comparer that determines whether the value of the property is changing.</param>
        /// <returns>The created property.</returns>
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

        /// <summary>
        /// Creates a property given an initial value and notifies the given actions before and after the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the created property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="beforeChange">An action that is invoked before the property value changes.</param>
        /// <param name="afterChange">An action that is invoked after the property of the value changes.</param>
        /// <returns>The created property.</returns>
        public static IProperty<T> FromValue<T>(T value, Action<T> beforeChange, Action<T> afterChange)
        {
            if (beforeChange == null) throw new ArgumentNullException("beforeChange");
            if (afterChange == null) throw new ArgumentNullException("afterChange");

            return FromValue(value, beforeChange, afterChange, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Creates a property given an initial value and a comparer. 
        /// </summary>
        /// <typeparam name="T">The type of the created property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <param name="comparer">A comparer that determines whether the value being set is equal to the current property value.</param>
        /// <returns>The created property.</returns>
        /// <remarks>If the comparer determines that the value being set is the same as the current value, the new value is ignored.</remarks>
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

        /// <summary>
        /// Creates a property with an initial value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The initial value of the property.</param>
        /// <returns>The created property.</returns>
        public static IProperty<T> FromValue<T>(T value = default(T))
        {
            return FromValue(value, EqualityComparer<T>.Default);
        }
    }
}
