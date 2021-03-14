using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class Engine_Info
    {
        public int number_of_threads { get; set; }
        public long real_time { get; set; }
        public double process_time { get; set; }
        public Engine_Info(int number_of_threads, long real_time, double process_time)
        {
            this.number_of_threads = number_of_threads;
            this.real_time = real_time;
            this.process_time = process_time;
        }
    }
}
