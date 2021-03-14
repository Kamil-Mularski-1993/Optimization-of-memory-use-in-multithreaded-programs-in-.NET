using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures
{
    class Single_record
    {
        public List<string> record { get; set; }
        public string category { get; set; }
        public Single_record()
        {
            this.record = new List<string>();
            this.category = null;
        }

        public Single_record(string [,] table, int row)
        {
            this.record = new List<string>();

            for(int i = 0; i < table.GetLength(1) - 1; i++)
            {
                this.record.Add(table[row, i]);
            }

            this.category = table[row, table.GetLength(1) - 1];
        }
    }
}
