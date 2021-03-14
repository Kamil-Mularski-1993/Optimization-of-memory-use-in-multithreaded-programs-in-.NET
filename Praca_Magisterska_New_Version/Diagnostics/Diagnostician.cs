using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class Diagnostician
    {
        public List<Checkpoint> list_of_checkpoints { get; set; }
        public int number_of_threads { get; set; }
        public List<Thread_Info> list_of_threads { get; set; }
        public Stopwatch global_watch { get; set; }
        public double pt_start_time {get; set;}
        public ProcessThread pt { get; set; }
        private object blockade { get; set; }
        public int[] gc_generation { get; set; }

        public Diagnostician()
        {
            this.list_of_checkpoints = new List<Checkpoint>();
            this.list_of_threads = new List<Thread_Info>();
            this.global_watch = new Stopwatch();
            blockade = new object();
            this.number_of_threads = 0;
            this.pt = Current_Process.GetCurrentProcessThread();
            this.pt_start_time = 0;
            this.gc_generation = new int[3];
        }

        public void Start()
        {
            this.global_watch.Start();
            this.gc_generation[0] = GC.CollectionCount(0);
            this.gc_generation[1] = GC.CollectionCount(1);
            this.gc_generation[2] = GC.CollectionCount(2);
        }
        public void Stop()
        {
            this.global_watch.Stop();
            this.gc_generation[0] = GC.CollectionCount(0) - gc_generation[0];
            this.gc_generation[1] = GC.CollectionCount(1) - gc_generation[1];
            this.gc_generation[2] = GC.CollectionCount(2) - gc_generation[2];
        }
        public void Checkpoint(string note)
        {
            this.list_of_checkpoints.Add(new Checkpoint(this.global_watch, pt.UserProcessorTime.TotalMilliseconds - pt_start_time, note));
        }

        public void AddThread()
        {
            lock (blockade)
            {
                this.list_of_threads.Add(new Thread_Info());
            }
        }

        public void Normalize()
        {
            foreach(Thread_Info thread in this.list_of_threads)
            {
                for(int i = 1; i < thread.thread_subtasks.Count(); i++)
                {
                    for(int j = 0; j < i; j++)
                    {
                        thread.thread_subtasks[i].subtask_memory -= thread.thread_subtasks[j].subtask_memory;
                    }
                }
            }
        }

        public double AdditionProcessorTime()
        {
            double sum = 0;
            foreach(Thread_Info thread in this.list_of_threads)
            {
                sum += thread.process_time;
            }

            return sum;
        }

        public void Save(int number_of_threads, Options options)
        {
            
            string path = "../../../Diagnostyka";     

            if(!Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            path = Path.Combine(path, options.number_of_issue.ToString() + "_" + number_of_threads.ToString() + "_" + options.allocation.ToString() + "_" + options.garbage_collector.ToString() + ".txt");

            using (var sw = File.CreateText(path))
            {
                sw.WriteLine("USTAWIENIA:\n  Rodzaj zadania: {0}\n  Ilość statyczna zadań: {1}\n  Bieżąca ilość wątków: {2}\n  Alokowanie pamięci: {3}\n  Czy GC włączony: {4}\n\n\n", options.number_of_issue, options.number_of_tasks, number_of_threads, options.allocation, options.garbage_collector);
                sw.WriteLine("Calkowity czas trwania programu dla zadanego przypadku (ms): {0}", this.global_watch.ElapsedMilliseconds);
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("Czas działania silnika wielowątkowego (ms):");
                sw.WriteLine("Czas rzeczywisty: {0}   |   Czas procesora: {1}   |   Stosunek:   {2}", this.list_of_checkpoints[3].real_time - this.list_of_checkpoints[2].real_time, this.AdditionProcessorTime(), this.AdditionProcessorTime() / (this.list_of_checkpoints[3].real_time - this.list_of_checkpoints[2].real_time));
                sw.WriteLine();
                sw.WriteLine("Poszczególne generacje: {0} / {1} / {2}", this.gc_generation[0], this.gc_generation[1], this.gc_generation[2]);
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("Poszczegolne punkty kontrolne:");
                foreach(Checkpoint p in this.list_of_checkpoints)
                {
                    sw.WriteLine("  {0} - Czas: {1} (ms)  |  Pamiec:  {2} (bajt)", p.note, p.real_time, p.memory);
                }
                sw.WriteLine("Czas pracy samego silnika: {0} (ms)", this.list_of_checkpoints[3].real_time - this.list_of_checkpoints[2].real_time);
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("Poszczegolne watki:");
                foreach (Thread_Info w in this.list_of_threads)
                {
                    sw.WriteLine("  Watek nr {0} - Czas R: {1} (ms)  | Czas P: {2} (ms) | Pamiec:  {3} (bajt)", w.id, w.real_time, w.process_time, w.thread_memory);
                    foreach(Task_info pz in w.thread_subtasks)
                    {
                        sw.WriteLine("          Podzadanie nr {0} - Czas: {1} (ms)  |  Pamiec:  {2} (bajt)", pz.id, pz.real_time, pz.subtask_memory);
                    }
                }
            }
        }

    }
}
