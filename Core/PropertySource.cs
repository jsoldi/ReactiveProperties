using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        /// <summary>
        /// Creates a property source given a subscribe function and a property value getter.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="rawSubscribe">A function that takes an action to be invoked whenever the property changes and returns a disposable that must cause the property to stop notifying whenever is disposed.</param>
        /// <param name="getValue">Gets the current value of the property.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Create<T>(Func<Action, IDisposable> rawSubscribe, Func<T> getValue)
        {
            if (rawSubscribe == null) throw new ArgumentNullException("rawSubscribe");
            if (getValue == null) throw new ArgumentNullException("getValue");

            return new ExplicitPropertySource<T>(rawSubscribe, getValue);
        }

        /// <summary>
        /// Creates a property source from a .NET property that has an associated empty event that fires when the property changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="getValue">A function that gets the .NET property value.</param>
        /// <param name="addHandler">An action that adds the given event handler to the .NET property changed event.</param>
        /// <param name="removeHandler">An action that removes the given event handler from the .NET property changed event.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Create<T>(Func<T> getValue, Action<EventHandler> addHandler, Action<EventHandler> removeHandler)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (addHandler == null) throw new ArgumentNullException("addHandler");
            if (removeHandler == null) throw new ArgumentNullException("removeHandler");
            
            return Create(getValue, notify => (s, e) => notify(), addHandler, removeHandler);
        }

        /// <summary>
        /// Creates a property source from a .NET property that has an associated event that fires when the property changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
        /// <param name="getValue">A function that gets the .NET property value.</param>
        /// <param name="createHandler">A function that takes an action and returns an event handler that invokes the given action.</param>
        /// <param name="addEventHandler">An action that adds the given event handler to the .NET property changed event.</param>
        /// <param name="removeEventHandler">An action that removes the given event handler from the .NET property changed event.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Create<T, TEventHandler>(Func<T> getValue, Func<Action, TEventHandler> createHandler, Action<TEventHandler> addEventHandler, Action<TEventHandler> removeEventHandler)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (createHandler == null) throw new ArgumentNullException("createHandler");
            if (addEventHandler == null) throw new ArgumentNullException("addEventHandler");
            if (removeEventHandler == null) throw new ArgumentNullException("removeEventHandler");

            return Create(
                observer =>
                {
                    TEventHandler eventHandler = createHandler(observer);
                    addEventHandler(eventHandler);
                    return Disposable.Create(() => removeEventHandler(eventHandler));
                },
                getValue
            );
        }

        /// <summary>
        /// Creates a property source from an existing property that has an associated event that is invoked when the property changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="getValue">A function that gets the existing property value.</param>
        /// <param name="addHandler">An action that adds the given event handler to the action that fires when the existing property changes.</param>
        /// <param name="removeHandler">An action that removes the given event handler from the action that fires when the existing property changes.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Create<T>(Func<T> getValue, Action<Action> addHandler, Action<Action> removeHandler)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (addHandler == null) throw new ArgumentNullException("addHandler");
            if (removeHandler == null) throw new ArgumentNullException("removeHandler");

            return Create(
                observer =>
                {
                    addHandler(observer);
                    return Disposable.Create(() => removeHandler(observer));
                },
                getValue
            );
        }

        /// <summary>
        /// Creates a property source given a function that returns the current property value and outputs an action that must be invoked whenever the property changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="getValue">A function that gets the current value of the property.</param>
        /// <param name="notify">An action that must be invoked whenever the value of the property changes.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Create<T>(Func<T> getValue, out Action notify)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");

            Action notifyEvent = null;

            notify = () =>
            {
                Action evt = notifyEvent;
                if (evt != null)
                    evt();
            };

            return Create(
                observer =>
                {
                    notifyEvent += observer;
                    return Disposable.Create(() => notifyEvent -= observer);
                },
                getValue
            );
        }
    }
}
