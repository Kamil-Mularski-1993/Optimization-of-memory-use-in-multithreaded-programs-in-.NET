using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Graph
{
    class Subgraph
    {
        public Vertex representative { get; set; }
        public List<Vertex> list_of_vertices { get; set; }
        public Subgraph(Vertex vertex)
        {
            this.representative = vertex;
            this.list_of_vertices = new List<Vertex>();
            list_of_vertices.Add(vertex);
        }
    }
}
