using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp7
{
    public class LongPressEffect : RoutingEffect
    {
        // Событие длительного нажатия
        public event EventHandler LongPressed;

        public LongPressEffect() : base("MauiApp7.LongPressEffect")
        {
        }

        public void HandleLongPress(View view)
        {
            LongPressed?.Invoke(view, EventArgs.Empty);
        }
    }
}
