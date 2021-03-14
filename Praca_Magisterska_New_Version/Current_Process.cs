using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Optymalizacja_wykorzystania_pamieci
{
    class Current_Process
    {
        public static ProcessThread GetCurrentProcessThread()
        {
            Int32 id = AppDomain.GetCurrentThreadId();
            ProcessThreadCollection pts = Process.GetCurrentProcess().Threads;
            ProcessThread pt;
            for (int i = pts.Count - 1; i >= 0; i--)
            {
                pt = pts[i];
                if (pt.Id == id)
                    return pt;
            }
            return null;
        }
    }
}
