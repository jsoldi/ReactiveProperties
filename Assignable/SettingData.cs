using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    /// <summary>
    /// Encapsulates the information needed to customize a property value setting operation.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    public struct SettingData<T>
    {
        private readonly T _CurrentValue;
        private readonly T _DesiredValue;
        private Action<T> _SetAndNotify;

        /// <summary>
        /// The current value of the property.
        /// </summary>
        public T CurrentValue 
        { 
            get { return _CurrentValue; } 
        }

        /// <summary>
        /// The value being assigned to the property.
        /// </summary>
        public T DesiredValue 
        { 
            get { return _DesiredValue; } 
        }

        /// <summary>
        /// Sets the given value to the property and notifies subscribers, if any. Should normally take <see cref="DesiredValue"/>.
        /// </summary>
        public Action<T> SetAndNotify 
        { 
            get { return _SetAndNotify; } 
        }

        internal SettingData(T currentValue, T desiredValue, Action<T> setAndNotify)
        {
            _CurrentValue = currentValue;
            _DesiredValue = desiredValue;
            _SetAndNotify = setAndNotify;
        }
    }
}
