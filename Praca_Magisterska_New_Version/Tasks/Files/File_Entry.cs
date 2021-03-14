using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Files
{
    class File_Entry
    {
        public string file_name { get; set; }
        public List<int> line_numbers { get; set; }

        public File_Entry(string file_name, int line_number)
        {
            this.file_name = file_name;
            this.line_numbers = new List<int>();
            this.line_numbers.Add(line_number);
        }
    }
}
