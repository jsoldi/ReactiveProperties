using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    public class SettingData<T>
    {
        public readonly T CurrentValue;
        public readonly T DesiredValue;
        public Action<T> SetAndNotify;

        internal SettingData(T currentValue, T desiredValue, Action<T> setAndNotify)
        {
            CurrentValue = currentValue;
            DesiredValue = desiredValue;
            SetAndNotify = setAndNotify;
        }
    }
}
