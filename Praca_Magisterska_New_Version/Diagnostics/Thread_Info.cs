using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class Thread_Info
    {
        public int id { get; set; }
        public ProcessThread system_thread { get; set; }
        public List<Task_info> thread_subtasks { get; set; }
        public long real_time { get; set; }
        public double process_time { get; set; }
        public long thread_memory { get; set; }
        public Thread_Info()
        {
            this.id = AppDomain.GetCurrentThreadId();
            this.thread_subtasks = new List<Task_info>();
            this.real_time = 0;
            this.process_time = 0;
            this.thread_memory = 0;

            this.system_thread = Current_Process.GetCurrentProcessThread();
        }

        public void UpdateSubtask(int task_id, Stopwatch watch, double process_time)
        {
            this.thread_subtasks.Add(new Task_info(task_id, watch, process_time));
        }
        public void ThreadUpdate(Stopwatch local_watch, double process_time)
        {
            this.real_time = local_watch.ElapsedMilliseconds;
            this.process_time = process_time;
            this.thread_memory = GC.GetAllocatedBytesForCurrentThread();
        }

    }
}
