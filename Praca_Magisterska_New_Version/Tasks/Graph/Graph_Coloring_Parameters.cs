using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Graph
{
    class Graph_Coloring_Parameters
    {
        public int method_first { get; set; }
        public int method_second { get; set; }
        public int method_third { get; set; }
        public Graph_Coloring_Parameters(int first, int second, int third)
        {
            this.method_first = first;
            this.method_second = second;
            this.method_third = third;
        }
    }
}
