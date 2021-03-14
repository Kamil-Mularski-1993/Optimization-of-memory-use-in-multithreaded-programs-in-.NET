using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class Checkpoint
    {
        public long real_time { get; set; }
        public double process_time { get; set; }
        public long memory { get; set; }
        public string note { get; set; }
        public Checkpoint(Stopwatch watch, double pt_time, string note)
        {
            this.real_time = watch.ElapsedMilliseconds;
            this.memory = GC.GetTotalMemory(false);
            this.note = note;
            this.process_time = pt_time;
        }
    }
}
