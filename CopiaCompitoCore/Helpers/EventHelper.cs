using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentCore
{
    public static class EventHelper
    {
        public static void Fire(this EventHandler ev, object sender = null)
        {
            if (ev == null)
                return;
            ev(sender, EventArgs.Empty);
        }

        public static void Fire<T>(this EventHandler<T> ev, object sender = null, T e = null) where T : EventArgs
        {
            if (ev == null)
                return;
            if (e == null)
                e = EventArgs.Empty as T;
            ev(sender, e);
        }

        
    }
}
