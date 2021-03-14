using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Graph
{
    class Graph_Constructing_Parameters
    {
        public int number_of_vertices { get; set; }
        public int number_of_edges { get; set; }
        public Graph_Constructing_Parameters(int vertices, int edges)
        {
            this.number_of_vertices = vertices;
            this.number_of_edges = number_of_edges;
        }
    }
}
