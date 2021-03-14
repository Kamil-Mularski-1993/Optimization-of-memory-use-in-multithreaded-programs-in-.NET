using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree
{
    class Comparision_List: IComparer<Single_record>
    {
        public int index { get; set; }
        public Comparision_List(int index)
        {
            this.index = index;
        }
        public int Compare(Single_record record_1, Single_record record_2)
        {
            return record_1.record[index].CompareTo(record_2.record[index]);
        }
    }
}
