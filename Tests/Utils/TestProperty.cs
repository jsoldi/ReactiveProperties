using ReactiveProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Utils
{
    internal class TestPropertySource<T> : IPropertySource<T>
    {
        public Action Notify;
        public T Value { get; set; }

        public TestPropertySource(T value)
        {
            Value = value;
        }

        public IDisposable RawSubscribe(Action rawObserver)
        {
            Notify += rawObserver;
            return Disposable.Create(() => Notify -= rawObserver);
        }

        public void SetAndNotify(T value)
        {
            Value = value;
            Notify();
        }
    }
}
