using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties.Utils
{
    /// <summary>
    /// A utility class that takes a list of disposables and dispose them all when its disposed.
    /// </summary>
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

        /// <summary>
        /// Adds a single disposable.
        /// </summary>
        /// <param name="obj">The disposable to add.</param>
        public void Add(IDisposable obj)
        {
            _Set.Add(obj);
        }

        /// <summary>
        /// Adds a list of disposables.
        /// </summary>
        /// <param name="objs">A list of disposables.</param>
        public void AddRange(IEnumerable<IDisposable> objs)
        {
            foreach (var item in objs)
                _Set.Add(item);
        }

        /// <summary>
        /// Adds a list of disposables.
        /// </summary>
        /// <param name="objs">A list of disposables.</param>
        public void AddRange(params IDisposable[] objs)
        {
            AddRange((IEnumerable<IDisposable>)objs);
        }

        /// <summary>
        /// Removes the given disposable without disposing it.
        /// </summary>
        /// <param name="obj">The disposable to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
        public bool Remove(IDisposable obj)
        {
            return _Set.Remove(obj);
        }

        /// <summary>
        /// Removes the given disposable and disposes it.
        /// </summary>
        /// <param name="obj">The disposable to remove and dispose.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
        public void RemoveAndDispose(IDisposable obj)
        {
            _Set.Remove(obj);
            obj.Dispose();
        }

        /// <summary>
        /// Removes and disposes all members.
        /// </summary>
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

        /// <summary>
        /// Disposes all the members.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
