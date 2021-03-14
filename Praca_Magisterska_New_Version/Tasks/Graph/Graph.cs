using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Graph
{
    class Graph
    {
        public int number_of_colors { get; set; }
        public int number_of_vertices { get; set; }
        private List<Graph> list_of_subgraphs { get; set; }
        public List<Vertex> list_of_vertices { get; set; }
        private bool show { get; set; }
        private object blockade { get; set; }

        //-------------------------------------------------------Konstruktory----------------------------------------------------------------------------
        public Graph(int number_of_vertices, bool show)          //KONSTRUKTOR PODSTAWOWY
        {
            this.number_of_colors = 0;
            this.number_of_vertices = number_of_vertices;
            this.list_of_vertices = new List<Vertex>();
            this.blockade = new object();
            this.show = show;
        }

        public Graph(int number_of_vertices, int number_of_edges, bool show)             //KONSTRUKTOR JEDNOWATKOWY
        {
            this.number_of_colors = 0;
            this.number_of_vertices = number_of_vertices;
            list_of_vertices = new List<Vertex>();
            this.blockade = new object();
            this.show = show;

            if (number_of_edges < (number_of_vertices - 1)) number_of_edges = number_of_vertices - 1;
            if (number_of_edges > (number_of_vertices * (number_of_vertices - 1) / 2))
                number_of_edges = number_of_vertices * (number_of_vertices - 1) / 2;

            for (int i = 0; i < number_of_vertices; i++)
            {
                this.list_of_vertices.Add(new Vertex(i, this));
            }


            List<Subgraph> list_of_postponed_subgraphs = new List<Subgraph>();
            List<Subgraph> list_of_operational_subgraphs = new List<Subgraph>();

            foreach (Vertex p in this.list_of_vertices)
            {
                list_of_operational_subgraphs.Add(new Subgraph(p));
            }
            Random random = new Random();
            while (list_of_operational_subgraphs.Count > 1)
            {
                int number_of_elements = list_of_operational_subgraphs.Count();

                for (int i = 0; i < number_of_elements / 2; i++)
                {
                    int rand = random.Next(1, list_of_operational_subgraphs.Count - 1); //losowanie indeksu do łączenia poza I, który zawsze jest brany

                    Subgraph first = list_of_operational_subgraphs[0];                      //pierwszy grafik do łączenia
                    Subgraph drawn = list_of_operational_subgraphs[rand];               //drugi grafik do łączenia

                    first.representative.AddConnection(drawn.representative);  //łączenia punktów z wylosowanych
                    first.list_of_vertices.AddRange(drawn.list_of_vertices);                //dodawanie jednej listy grafiku 2 do drugiej w grafiku 1
                    first.representative = first.list_of_vertices[random.Next(0, first.list_of_vertices.Count - 1)];     //losowanie nowego reprezentanta
                    list_of_postponed_subgraphs.Add(first);                            //odkładanie na listę do zapamietania

                    list_of_operational_subgraphs.RemoveAt(rand);                          //usuwanie wylosowanego grafiku z listy operacyjnej
                    list_of_operational_subgraphs.RemoveAt(0);                               //usuwanie pierwszego grafiku z listy operacyjnej
                }

                if (list_of_operational_subgraphs.Count > 0)                      //gdy pozostanie 1 nieprzypisany do pary grafik
                {
                    int rand = random.Next(0, list_of_postponed_subgraphs.Count - 1);
                    list_of_postponed_subgraphs[rand].list_of_vertices.AddRange(list_of_operational_subgraphs[0].list_of_vertices);
                    list_of_postponed_subgraphs[rand].representative.AddConnection(list_of_operational_subgraphs[0].representative);
                    list_of_postponed_subgraphs[rand].representative =
                        list_of_postponed_subgraphs[rand].list_of_vertices[random.Next(0, list_of_postponed_subgraphs[rand].list_of_vertices.Count - 1)];
                    list_of_operational_subgraphs.RemoveAt(0);
                }
                list_of_operational_subgraphs.Clear();
                list_of_operational_subgraphs.AddRange(list_of_postponed_subgraphs);
                list_of_postponed_subgraphs.Clear();
            }

            number_of_edges -= (number_of_vertices - 1);

            while (number_of_edges > 0)           //przy dużej liczbie krawędzi może paru nie znaleźć - natrafi na punkt całkowicie zajęty, a mimo to odejmie krawędź (do poprawy w wolnym czasie)
            {
                int rand = random.Next(0, number_of_vertices - 1);
                if (this.list_of_vertices[rand].unrelated.Count > 0)
                {
                    int rand_2 = random.Next(0, this.list_of_vertices[rand].unrelated.Count - 1);
                    this.list_of_vertices[rand].AddConnection(this.list_of_vertices[rand].unrelated[rand_2]);
                    this.list_of_vertices[rand].degree++;

                }
                number_of_edges--;
            }

        }

        //---------------------------------------------------------------Tworzenie grafu przez wiele wątków---------------------------------------------------
        public Queue<TaskInterface> GraphConstructing(int number_of_vertices, int number_of_edges, int number_of_tasks, Diagnostician diag) //Pseudokonstruktor wielowątkowy (poprawić)
        {
            number_of_colors = 0;
            this.number_of_vertices = number_of_vertices;
            list_of_subgraphs = new List<Graph>();
            list_of_vertices = new List<Vertex>();
            this.show = false;

            int rest_v = number_of_vertices % number_of_tasks;
            int rest_e = number_of_edges % number_of_tasks;

            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();

            for (int i = 0; i < number_of_tasks; i++)
            {
                Graph_Constructing_Parameters parameters = new Graph_Constructing_Parameters(number_of_vertices / number_of_tasks, number_of_edges / number_of_tasks);
                if (rest_v > 0)
                {
                    parameters.number_of_vertices++;
                    rest_v--;
                }
                if(rest_e > 0)
                {
                    parameters.number_of_edges++;
                    rest_e--;
                }

                list_of_tasks.Enqueue(new Engine_Task<Graph, Graph_Constructing_Parameters>(i, this, new TypeOfTask<Graph, Graph_Constructing_Parameters>(GraphPartsConstructor), parameters));
            }
          
            return list_of_tasks;
        }

        private void GraphPartsConstructor(Graph graph, Graph_Constructing_Parameters parameters)
        {
            Graph graph_part = new Graph(parameters.number_of_vertices, parameters.number_of_edges, false);
            lock (blockade)
            {
                graph.list_of_subgraphs.Add(graph_part);
            }
        }


        public void Finalization()
        {
            Random random = new Random();

            foreach (Graph g in list_of_subgraphs)
            {
                foreach (Vertex p in g.list_of_vertices)
                    this.list_of_vertices.Add(p);
            }

            for (int i = 0; i < list_of_subgraphs.Count(); i++)
            {
                Vertex vertex_1 = list_of_vertices.Find(x => x == list_of_subgraphs[i].list_of_vertices[0]);
                for (int j = i + 1; j < list_of_subgraphs.Count(); j++)
                {
                    Vertex vertex_2 = list_of_vertices.Find(x => x == list_of_subgraphs[j].list_of_vertices[0]);
                    if (vertex_1 != vertex_2)
                    {
                        vertex_1.AddConnection(vertex_2);
                    }
                }
            }
            foreach (Vertex vertex in list_of_vertices)
                vertex.related.Remove(vertex);
        }

        //------------------------------------------------------------------------Metody odnośnie kolorowania------------------------------------------------------
        private Graph Copy()
        {
            Graph graph_copy = new Graph(this.number_of_vertices, false);
            graph_copy.show = this.show;
            foreach (Vertex vertex in this.list_of_vertices)
            {
                graph_copy.list_of_vertices.Add(new Vertex(vertex.id, graph_copy));
            }

            foreach (Vertex vertex in this.list_of_vertices)
            {
                foreach (Vertex related_vertex in vertex.related)
                {
                    graph_copy.list_of_vertices[vertex.id].related.Add(graph_copy.list_of_vertices[related_vertex.id]);
                    graph_copy.list_of_vertices[vertex.id].unrelated.Remove(graph_copy.list_of_vertices[vertex.id].related[graph_copy.list_of_vertices[vertex.id].related.Count - 1]);
                }
            }

            return graph_copy;
        }

        public Queue<TaskInterface> Coloring(Options op, Diagnostician diag)
        {
            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();
            int nr = 0;
            int i = 0;
            while (true)
            {
                i++;
                for (int j = 1; j < 4; j++)
                {
                    if (i != j)
                        for (int k = 1; k < 4; k++)
                        {
                            if ((i != k) && (j != k))
                            {
                                nr++;
                                if(op.allocation)
                                    list_of_tasks.Enqueue(new Engine_Task<Graph, Graph_Coloring_Parameters>(nr, this, new TypeOfTask<Graph, Graph_Coloring_Parameters>(ColoringMethodWithAdditionalMemory), new Graph_Coloring_Parameters(i, j, k)));
                                else
                                    list_of_tasks.Enqueue(new Engine_Task<Graph, Graph_Coloring_Parameters>(nr, this, new TypeOfTask<Graph, Graph_Coloring_Parameters>(ColoringMethod), new Graph_Coloring_Parameters(i, j, k)));
                            }
                            if (nr == op.number_of_tasks) break;
                        }
                    if (nr == op.number_of_tasks) break;
                }
                if (i == 3) i = 0;
                if (nr == op.number_of_tasks) break;
            }
          
            return list_of_tasks;
        }

        private void ColoringMethod(Graph main_graph, Graph_Coloring_Parameters parameters)
        {
            Graph graph = new Graph(main_graph.number_of_vertices, false);

            graph = main_graph.Copy();

            while (graph.list_of_vertices.Count > 1)
            {
                Vertex vertex = graph.list_of_vertices[0];
                Vertex tmp = graph.list_of_vertices[0];
                for (int i = 1; i < graph.list_of_vertices.Count - 1; i++)
                {
                    vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_first);
                    if (vertex == null) vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_second);
                    if (vertex == null) vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_third);
                    if (vertex == null) vertex = graph.list_of_vertices[i];
                    tmp = vertex;
                }
                vertex.ColorIn();
                if (vertex.color >= graph.number_of_colors) graph.number_of_colors++;

                for (int i = 0; i < vertex.related.Count(); i++)             
                    vertex.related[i].related.Remove(vertex);

                graph.list_of_vertices.Remove(vertex);
            }

            if (graph.list_of_vertices.Count() > 0)
            {
                graph.list_of_vertices[0].ColorIn();

                if (graph.list_of_vertices[0].color == graph.number_of_colors) graph.number_of_colors++;
            }

            if(graph.show)
                graph.ShowResults();

        }

        private void ColoringMethodWithAdditionalMemory(Graph main_graph, Graph_Coloring_Parameters parameters)
        {
            Graph graph = new Graph(main_graph.number_of_vertices, false);

            graph = main_graph.Copy();

            while (graph.list_of_vertices.Count > 1)
            {
                Vertex vertex = new Vertex(0, null);
                Vertex tmp = new Vertex(0, null);
                vertex = graph.list_of_vertices[0];
                tmp = graph.list_of_vertices[0];
                for (int i = 1; i < graph.list_of_vertices.Count - 1; i++)
                {
                    vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_first);
                    if (vertex == null) vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_second);
                    if (vertex == null) vertex = Comparision(tmp, graph.list_of_vertices[i], parameters.method_third);
                    if (vertex == null) vertex = graph.list_of_vertices[i];
                    tmp = vertex;
                }
                vertex.ColorIn();
                if (vertex.color >= graph.number_of_colors) graph.number_of_colors++;

                for (int i = 0; i < vertex.related.Count(); i++)           
                    vertex.related[i].related.Remove(vertex);

                graph.list_of_vertices.Remove(vertex);
            }

            if (graph.list_of_vertices.Count() > 0)
            {
                graph.list_of_vertices[0].ColorIn();

                if (graph.list_of_vertices[0].color == graph.number_of_colors) graph.number_of_colors++;
            }


            graph.ShowResults();

        }

        private static Vertex Comparision(Vertex vertex_1, Vertex vertex_2, int option)
        {
            switch (option)
            {
                case 1:
                    if (vertex_1.degree > vertex_2.degree) return vertex_1;
                    if (vertex_1.degree < vertex_2.degree) return vertex_2;
                    return null;
                case 2:
                    if (vertex_1.saturation > vertex_2.saturation) return vertex_1;
                    if (vertex_1.saturation < vertex_2.saturation) return vertex_2;
                    return null;
                case 3:
                    if (vertex_1.intensity > vertex_2.intensity) return vertex_1;
                    if (vertex_1.intensity < vertex_2.intensity) return vertex_2;
                    return null;
                default:
                    return null;
            }
        }

        public void ShowResults()
        {
            Console.WriteLine("Graf pokolorowano na {0} barw", this.number_of_colors);
        }
    }
}
