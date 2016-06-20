using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public interface IPropertySource<out T>
    {
        T Value { get; }
        IDisposable RawSubscribe(Action rawObserver);
    }
}
