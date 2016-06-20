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
