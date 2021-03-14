using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Optymalizacja_wykorzystania_pamieci.Diagnostics
{
    class General_Diagnostician
    {
        public List<Diagnostician> list_of_diagnosticians { get; set; }
        public List<Engine_Info> list_of_engines_info { get; set; }
        public General_Diagnostician()
        {
            this.list_of_diagnosticians = new List<Diagnostician>();
            this.list_of_engines_info = new List<Engine_Info>();
        }

        public void Save_statistics()
        {
            string main_folder = "../../../Diagnostyka";
            DateTime date = DateTime.Now;

            main_folder = System.IO.Path.Combine(main_folder, date.ToString("MM/dd/yyyy HH"));

            string path = System.IO.Path.Combine(main_folder, "Statistics.txt");


            using (var sw = File.CreateText(path))
            {
                sw.WriteLine("STATYSTYKI: \n\n");
                foreach (Diagnostician diag in this.list_of_diagnosticians)
                {
                    sw.WriteLine("Kolekcje GC dla próby z {0} wątkami:  {1} / {2} / {3}", diag.number_of_threads, diag.gc_generation[0], diag.gc_generation[1], diag.gc_generation[2]);
                }

                sw.WriteLine("\n\nPOSZCZEGOLNE CZASY:\n\n");


                foreach (Engine_Info e_info in this.list_of_engines_info)
                {
                    sw.WriteLine("Watki: {0}   |   Czas rzecz: {1} ms |  Czas proc: {2} ms  ", e_info.number_of_threads, e_info.real_time, e_info.process_time);
                }

                sw.WriteLine("\n\nSTOSUNEK DO CZASU 1 WATKU:");

                
                foreach (Engine_Info e_info in this.list_of_engines_info)
                {
                    sw.WriteLine("Watki: {0}   |   Czas rzecz: {1}  |  Czas proc: {2} ", e_info.number_of_threads, e_info.real_time / (double)list_of_engines_info[0].real_time, e_info.process_time / list_of_engines_info[0].process_time);
                }
            }
        }
    }
}
