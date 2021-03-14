using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Optymalizacja_wykorzystania_pamieci.Tasks;
using Optymalizacja_wykorzystania_pamieci.Tasks.Simple_Allocation;
using Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree;
using Optymalizacja_wykorzystania_pamieci.Tasks.Graph;
using Optymalizacja_wykorzystania_pamieci.Tasks.Array_of_Numbers;
using Optymalizacja_wykorzystania_pamieci.Tasks.Files;
using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;

namespace Optymalizacja_wykorzystania_pamieci
{
    class Tasks_Administrator
    {
        public static void Run(Options options)
        {
            General_Diagnostician general_diag = new General_Diagnostician();
            
            int current_number_of_threads = 0;

            foreach(int nr in options.number_of_threads)
            {
                general_diag.list_of_diagnosticians.Add(new Diagnostician());
            }

            Thread.BeginThreadAffinity();

            foreach (Diagnostician diag in general_diag.list_of_diagnosticians)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                

                Console.WriteLine("\nBiezaca proba dla silnika z liczba {0} watkow: \n", options.number_of_threads[current_number_of_threads]);
                Multi_threaded_Engine engine = new Multi_threaded_Engine(options.number_of_threads[current_number_of_threads], options);

                

                diag.pt = Current_Process.GetCurrentProcessThread();
                diag.pt_start_time = diag.pt.UserProcessorTime.Milliseconds;
                diag.Start();
                diag.Checkpoint("Start");
                switch (options.number_of_issue)
                {
                    case 6://-----------------------------------------------------------Zadanie z prostą alokacją------------------------------------------------------------------------------
                        Simple_Array_Allocation simple_example = new Simple_Array_Allocation(options.allocation);

                        diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");
                        engine.Run(simple_example.PrepareForSimpleAllocation(options.number_of_tasks), diag);
                        diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");
                        diag.Stop();

                        simple_example = null;
                        break;

                    case 1://-----------------------------------------------------------Zadanie z tablicą------------------------------------------------------------------------------
                        try
                        {
                            Array_Of_Numbers array = new Array_Of_Numbers(100000000, options.allocation);
                            diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");
                            engine.Run(array.PrepareForSort(options.number_of_tasks, diag), diag);
                            Console.WriteLine("Finalizacja");
                            array.Finalization();

                            diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");
                            diag.Stop();
                            if (options.show_result)
                                array.ShowResults();

                            array = null;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Nie udalo sie zaalowokac wystarczajaco duzo pamieci dla proby z {0} watkami.", options.number_of_threads[current_number_of_threads]);
                        }

                        break;

                    case 2: //------------------------------------------------------------Zadanie z drzewami decyzyjnymi------------------------------------------
                        Decision_Trees_Collection collection_of_decision_trees = new Decision_Trees_Collection("../../../data", options.number_of_tasks);
                        diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");
                        engine.Run(collection_of_decision_trees.PrepareForTreeCreation(options), diag);

                        diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");

                        diag.Stop();

                        if (options.show_result)
                            collection_of_decision_trees.CheckTrees();

                        collection_of_decision_trees = null;
                        break;

                    case 3://----------------------------------------------------------Zadanie z grafem (Tworzenie)--------------------------------------------------------------------------
                        Graph graph_for_constructing = new Graph(10, options.show_result);
                        
                        Queue<TaskInterface> queue = graph_for_constructing.GraphConstructing(192000, 19200000, options.number_of_tasks, diag);
                        diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");

                        engine.Run(queue, diag);

                        graph_for_constructing.Finalization();

                        diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");
                        diag.Stop();
                        graph_for_constructing = null;
                        break;


                    case 4: //----------------------------------------------------------Zadanie z grafem (Kolorowanie)-------------------------------------------------------------------------
                        Graph graph_for_coloring = new Graph(1920, 38400, options.show_result);

                        diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");

                        engine.Run(graph_for_coloring.Coloring(options, diag), diag);

                        diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");
                        diag.Stop();
                        graph_for_coloring = null;
                        break;

                    case 5://--------------------------------------------------------------Zadanie z plikami---------------------------------------------------------------------

                        Files.AlignFilesToTasks(options.number_of_tasks, "../../../Pliki Tekstowe");

                        Files file = new Files("../../../Pliki tekstowe", "magisterska");

                        diag.Checkpoint("Rozpoczecie rozwiazywania zadanego problemu");
                        engine.Run(file.FileSearch(diag), diag);
                        diag.Checkpoint("Zakonczenie rozwiazywania zadanego problemu");
                        diag.Stop();

                        if (options.show_result)
                            file.ShowResults();

                        file = null;
                        break;
                }

                general_diag.list_of_engines_info.Add(new Engine_Info(options.number_of_threads[current_number_of_threads], diag.list_of_checkpoints[3].real_time - diag.list_of_checkpoints[2].real_time, diag.AdditionProcessorTime()));
                diag.Normalize();
                diag.Save(options.number_of_threads[current_number_of_threads], options);
                Console.WriteLine("\nZakonczenie proby z {0} watkiem. \n\n", options.number_of_threads[current_number_of_threads]);
                diag.number_of_threads = options.number_of_threads[current_number_of_threads];
                current_number_of_threads++;
            }
            Thread.EndThreadAffinity();
            Console.WriteLine("KONIEC");
            /*
            foreach(Diagnostician diag in general_diag.list_of_diagnosticians)
            {
                Console.WriteLine("Próba z {0} watkami: {1}", diag.number_of_threads, diag.list_of_checkpoints[3].real_time - diag.list_of_checkpoints[2].real_time);
            }
            
            foreach(Engine_Info e_info in general_diag.list_of_engines_info)
            {
                Console.WriteLine("{0} | {1}  |  {2}", e_info.number_of_threads, e_info.real_time, e_info.process_time);
            }
            */
            //general_diag.Save_statistics();

            //Console.ReadKey();
        }
    }
}
