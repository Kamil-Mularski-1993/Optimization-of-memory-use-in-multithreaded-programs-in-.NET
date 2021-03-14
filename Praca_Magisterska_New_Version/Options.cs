using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Optymalizacja_wykorzystania_pamieci
{
    class Options
    {
        public List<int> number_of_threads { get; set; }
        public int number_of_tasks { get; set; }
        public int number_of_issue { get; set; }
        public bool allocation { get; set; }
        public bool garbage_collector { get; set; }
        public bool show_result { get; set; }
        public Options()
        {
            this.number_of_issue = 5;         // 1 - pliki, 2-kolorowanie grafu, 3 - tworzenie grafu, 4 - tablica liczb, 5 - drzewa decyzyjne

           //this.number_of_threads = 1;          // gdy opcja lawinowa jest wybrana - maksymalna ilość uruchomionych wątków 
            this.number_of_threads = new List<int>();
            //this.number_of_threads.Add(1);

            this.number_of_tasks = 1;        // ilość podzadań do zrealizowania przez silnik - nie każde zadanie je implementuje (np. kolorowanie, obsługa plików)

            this.allocation = false;          // czy alokować pamięć przez wątki w silniku (wybrane zadania: tablica liczb)

            this.garbage_collector = false;
            this.show_result = false;
        }

        public Options LoadOptions(string path)
        {
            string[] datas = new string[6];
            bool read_records = false;
            int i = 0;

            Options op = new Options();

            try
            {
                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (read_records)
                        {
                            string[] tmp = line.Split('=');
                            datas[i] = tmp[1];
                            i++;
                        }

                        if (line.IndexOf("OPTIONS", StringComparison.CurrentCultureIgnoreCase) >= 0) read_records = true;
                    }
                }

                if (Int32.Parse(datas[0]) > 0 && Int32.Parse(datas[0]) < 7)
                    op.number_of_issue = Int32.Parse(datas[0]);
                else
                    op.number_of_issue = 1;

                string[] tmp_2 = datas[1].Split(',');
                foreach(string s in tmp_2)
                {
                   // if(Int32.Parse(s) > 1 )
                    op.number_of_threads.Add(Int32.Parse(s));
                }

                if (Int32.Parse(datas[2]) > 0)
                    op.number_of_tasks = Int32.Parse(datas[2]);
                else
                    op.number_of_tasks = 1;

                op.allocation = Convert.ToBoolean(datas[3]);
                op.garbage_collector = Convert.ToBoolean(datas[4]);
                op.show_result = Convert.ToBoolean(datas[5]);

                return op;
            }
            catch
            {
                return this;
            }
        }

        public void ShowOptions()
        {
            Console.WriteLine("\nOPCJE:\n");
            Console.WriteLine("Numer zagadnienia: {0}", this.number_of_issue);
            Console.Write("Ilosc uruchamianych watkow: ");
            foreach (int i in this.number_of_threads)
                Console.Write("{0}, ", i);
            Console.WriteLine("\nIlosc rozpatryanych zadan: {0}", this.number_of_tasks);
            Console.WriteLine("Dodatkowa alokacja: {0}", this.allocation);
            Console.WriteLine("Reczne zwalnianie pamieci: {0}", this.garbage_collector);
            Console.WriteLine("Wypisywanie na ekran: {0}\n", this.show_result);
        }
    }
}
