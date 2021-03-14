using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class Task_info
    {
        public int id { get; set; }
        public long real_time { get; set; }
        public double process_time { get; set; }
        public long subtask_memory { get; set; }
        public Task_info(int id, Stopwatch watch, double process_time)
        {
            this.id = id;
            this.real_time = watch.ElapsedMilliseconds;
            this.subtask_memory = GC.GetAllocatedBytesForCurrentThread();
            this.process_time = process_time;
        }
    }
}
