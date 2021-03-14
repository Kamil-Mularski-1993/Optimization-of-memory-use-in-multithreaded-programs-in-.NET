using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Optymalizacja_wykorzystania_pamieci
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Options options = new Options();

            options = options.LoadOptions("../../../Opcje.txt");
            options.ShowOptions();

            Tasks_Administrator.Run(options);
            */

            Options options = new Options();

            string path = "../../../OPCJE";

            Directory.GetFiles(path);

            List<string> list_of_files = Directory.GetFiles(path).ToList<string>();

            if (list_of_files.Count > 0)
            {
                options = options.LoadOptions(list_of_files[0]);
                options.ShowOptions();

                File.Delete(list_of_files[0]);

                Tasks_Administrator.Run(options);

                if(list_of_files.Count() > 1)
                    Process.Start("RUN.bat");
            }

        }
    }
}
