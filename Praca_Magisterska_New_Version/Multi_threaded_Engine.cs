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
    class Multi_threaded_Engine
    {
        private object blockade = new object();
        private int number_of_tasks { get; set; }
        private int number_of_threads { get; set; }
        private List<Thread> list_of_threads {get; set;}
        private Options options { get; set; }
        private int completed_threads { get; set; }

        public Multi_threaded_Engine(int threads, Options options)
        {
            this.number_of_threads = threads;
            this.number_of_tasks = 0;
            this.list_of_threads = new List<Thread>();
            this.options = options;
            this.completed_threads = 0;
        }

        public void Run(Queue<TaskInterface> list_of_tasks, Diagnostician diag)
        {
            diag.Checkpoint("Rozpoczeczie dzialania silnika wielowątkowego.");
            number_of_tasks = list_of_tasks.Count();

            for (int i = 0; i < number_of_threads; i++)
            {                
                var thread = new Thread(() =>
                {
                    //Kod zarządzany przez 1 wątek systemowy
                    Thread.BeginThreadAffinity();

                    //Dodanie bieżącego wątku do listy diagnostycznej
                    diag.AddThread();
                    
                    //Utworzenie lokalnych zegarów pomiarowych
                    Stopwatch thread_local_timer = new Stopwatch();
                    ProcessThread pt = Current_Process.GetCurrentProcessThread();
                    
                    //Resetowanie zegarów
                    thread_local_timer.Start();
                    double process_timer = pt.UserProcessorTime.TotalMilliseconds;

                    //Pętla z poborem i wykonywaniem zadań
                    while (number_of_tasks > 0)
                    {
                        //TaskInterface task = new Engine_Task<TaskInterface, TaskInterface>();
                        TaskInterface task = new Engine_Task<TaskInterface, TaskInterface>(-1, null, null, null);

                        bool if_receive = false;
                        lock (blockade)
                        {
                            if (number_of_tasks > 0)
                            {
                                number_of_tasks--;
                                task = list_of_tasks.Dequeue();
                                if_receive = true;
                            }
                        }
                        Console.WriteLine("Watek {0} rozpoczal prace | Pozostalo {1} zadan", AppDomain.GetCurrentThreadId(), number_of_tasks);
                        if (if_receive)
                            task.Execute(diag);
                        task = null;

                        //Uruchamianie Garbage Collectora jesli ustawiono
                        if (options.garbage_collector)
                            GC.Collect();
                    }

                    //Zapis pomiarów czasu
                    thread_local_timer.Stop();
                    process_timer = pt.UserProcessorTime.TotalMilliseconds - process_timer;
                    diag.list_of_threads.Find(x => x.id == 
                        AppDomain.GetCurrentThreadId()).ThreadUpdate(thread_local_timer, process_timer);

                    lock (blockade) completed_threads++;
                    Thread.EndThreadAffinity();
                });
                thread.Start();

                list_of_threads.Add(thread);
            }

            foreach (var thread in list_of_threads) thread.Join();
              
            diag.Checkpoint("Zakonczenie dzialania silnika wielowątkowego");
        }
    }
}
