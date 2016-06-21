using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactiveProperties
{
    public static partial class PropertySource
    {
        /// <summary>
        /// Creates a property source that notifies using the given control's <c>Invoke</c> method.
        /// </summary>
        /// <typeparam name="T">The type of the property source.</typeparam>
        /// <param name="source">The original property source.</param>
        /// <param name="control">The control to synchronize to.</param>
        /// <returns>The created property source.</returns>
        public static IPropertySource<T> Synchronize<T>(this IPropertySource<T> source, Control control)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (control == null) throw new ArgumentNullException("control");

            Action<Action> safeAction = action =>
            {
                if (control.InvokeRequired)
                    control.Invoke(action);
                else
                    action();
            };

            Func<Func<T>, T> safeFunction = function =>
            {
                T result = default(T);
                Action action = () => result = function();

                if (control.InvokeRequired)
                    control.Invoke(action);
                else
                    action();

                return result;
            };

            return PropertySource.Create(
                observer => source.RawSubscribe(() => safeAction(observer)),
                () => safeFunction(() => source.Value)
            );
        }
    }
}
