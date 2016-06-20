using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties.Utils
{
    public class DisposableSet : IDisposable
    {
        private readonly HashSet<IDisposable> _Set = new HashSet<IDisposable>();
        private bool _Disposed;

        public DisposableSet() { }

        public DisposableSet(IEnumerable<IDisposable> disposables)
        {
            AddRange(disposables);
        }

        public DisposableSet(params IDisposable[] disposables)
            : this(disposables.AsEnumerable()) { }

        public void Add(IDisposable obj)
        {
            _Set.Add(obj);
        }

        public void AddRange(IEnumerable<IDisposable> objs)
        {
            foreach (var item in objs)
                _Set.Add(item);
        }

        public void AddRange(params IDisposable[] objs)
        {
            AddRange((IEnumerable<IDisposable>)objs);
        }

        public bool Remove(IDisposable obj)
        {
            return _Set.Remove(obj);
        }

        public void RemoveAndDispose(IDisposable obj)
        {
            _Set.Remove(obj);
            obj.Dispose();
        }

        public void RemoveAndDisposeAll()
        {
            foreach (var item in _Set)
                item.Dispose();

            _Set.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    foreach (var item in _Set)
                        item.Dispose();
                }
            }
            _Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class DisposableMap : IDisposable
    {
        private readonly Dictionary<object, IDisposable> Disposables = new Dictionary<object, IDisposable>();
        private bool _Disposed;

        public void Add(object key, IDisposable value)
        {
            Disposables.Add(key, value);
        }

        public void TryRemoveAndDispose(object key)
        {
            if (Disposables.ContainsKey(key))
                RemoveAndDispose(key);
        }

        public void RemoveAndDispose(object key)
        {
            Disposables[key].Dispose();
            Disposables.Remove(key);
        }

        public void RemoveAndDisposeAll()
        {
            foreach (var key in Disposables)
                Disposables[key].Dispose();

            Disposables.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    foreach (var disposable in Disposables.Values)
                        disposable.Dispose();
                }
            }
            _Disposed = true;
        }
    }
}
