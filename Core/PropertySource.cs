using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        public static IPropertySource<T> Create<T>(Func<Action, IDisposable> rawSubscribe, Func<T> getValue)
        {
            if (rawSubscribe == null) throw new ArgumentNullException("rawSubscribe");
            if (getValue == null) throw new ArgumentNullException("getValue");

            return new ExplicitPropertySource<T>(rawSubscribe, getValue);
        }

        public static IPropertySource<T> Create<T>(Func<T> getValue, Action<EventHandler> addHandler, Action<EventHandler> removeHandler)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (addHandler == null) throw new ArgumentNullException("addHandler");
            if (removeHandler == null) throw new ArgumentNullException("removeHandler");

            return Create(getValue, notify => (s, e) => notify(), addHandler, removeHandler);
        }

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
