using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;

namespace Optymalizacja_wykorzystania_pamieci
{
    class Engine_Task<TObject, TParameters>: TaskInterface
    {
        public int id { get; set; }
        public long lead_time { get; set; }
        public TParameters parameters { get; set; }
        public TObject problem_object { get; set; }
        TypeOfTask<TObject, TParameters> method { get; set; }

        public Engine_Task(int id, TObject task_object, 
            TypeOfTask<TObject, TParameters> method, TParameters task_parameters)
        {
            this.id = id;
            this.lead_time = 0;
            this.problem_object = task_object;
            this.method = method;
            this.parameters = task_parameters;
        }

        public void Execute(Diagnostician diag)
        {
            Stopwatch task_timer = new Stopwatch();
            ProcessThread pt_task = Current_Process.GetCurrentProcessThread();
            task_timer.Start();
            double process_timer = pt_task.UserProcessorTime.TotalMilliseconds;
            this.method(this.problem_object, parameters);
            task_timer.Stop();
            process_timer = pt_task.UserProcessorTime.TotalMilliseconds - process_timer;
            diag.list_of_threads.Find(x => x.id == 
                AppDomain.GetCurrentThreadId()).UpdateSubtask(this.id, task_timer, process_timer);

            task_timer = null;
        }
    }
}
