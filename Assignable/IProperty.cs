using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public interface IProperty<T> : IPropertySource<T>
    {
        new T Value { get; set; }
    }
}
