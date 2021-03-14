using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Graph
{
    class Vertex
    {
        public int id { get; set; }
        public int color { get; set; }
        public int saturation { get; set; }
        public int intensity { get; set; }
        public int degree { get; set; }
        public List<Vertex> related { get; set; }
        public List<Vertex> unrelated { get; set; }
        public bool[] pallete_of_colors { get; set; }

        public Vertex(int id, Graph graph)
        {
            this.id = id;
            this.color = -1;
            this.saturation = 0;
            this.intensity = 0;
            this.degree = 0;
            this.related = new List<Vertex>();
            this.unrelated = new List<Vertex>();

            if (graph != null) { 
                this.pallete_of_colors = new bool[graph.number_of_vertices];
                for (int i = 0; i < pallete_of_colors.Length; i++)
                    this.pallete_of_colors[i] = false;


                foreach (Vertex p in graph.list_of_vertices)
                {
                    p.unrelated.Add(this);
                    this.unrelated.Add(p);
                }
            }

        }

        public void ColorIn()
        {
            this.color = Array.FindIndex(this.pallete_of_colors, used);

            foreach (Vertex s in this.related)
            {
                s.saturation++;
                if (s.pallete_of_colors[color] == false)
                {
                    s.pallete_of_colors[color] = true;
                    s.intensity++;
                }
            }
        }
        private static bool used(bool x)
        {
            return !x;
        }

        public void AddConnection(Vertex vertex)
        {
            this.related.Add(vertex);
            vertex.related.Add(this);
            this.unrelated.Remove(vertex);
            vertex.unrelated.Remove(this);
            this.degree++;
            vertex.degree++;
        }
    }
}
