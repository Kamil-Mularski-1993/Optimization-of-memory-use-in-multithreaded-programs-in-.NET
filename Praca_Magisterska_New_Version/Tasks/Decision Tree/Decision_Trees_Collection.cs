using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;
using Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree
{
    class Decision_Trees_Collection
    {
        //Pliki z danymi nie moga zawierać dodatkowych linii po linii z ostatnimi danymi. Pliki z danymi muszą zawierać same dane, lub zaczynac się od słowa GENERAL oraz zawierac pozycje Class POSITION i DATA
        public List<Decision_Tree> list_of_decision_trees { get; set; }
        public List<Data> datas { get; set; }
        public List<string> files { get; set; }
        public Decision_Trees_Collection(string path, int number_of_tasks)
        {
            this.list_of_decision_trees = new List<Decision_Tree>();
            this.datas = new List<Data>();

            this.files = Directory.GetFiles(path).ToList<string>();

            for(int i = 0; i < files.Count(); i++)
            {
                this.datas.Add(new Data());
            }

            for(int i = 0; i < number_of_tasks; i++)
            {
                list_of_decision_trees.Add(new Decision_Tree());
            }
        }

        public Queue<TaskInterface> PrepareForTreeCreation(Options op)
        {
            int index = 0;
            foreach(Data data in this.datas)
            {
                data.LoadData(files[index++]);               
            }

            index = 0;
            foreach(Decision_Tree tree in list_of_decision_trees)
            {
                tree.root = new Node(datas[index].SoftCopy());
                tree.root.data.UpdateCategories();
                index++;
                if (index == datas.Count()) index = 0;
            }

            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();

            for (int i = 0; i < this.list_of_decision_trees.Count(); i++)
            {
                //Dodawanie do kolejki zadań
                if(op.allocation)
                    list_of_tasks.Enqueue(new Engine_Task<Decision_Tree, Trees_Parameters>
                    (i, this.list_of_decision_trees[i], new TypeOfTask<Decision_Tree, Trees_Parameters>(Decision_Tree.TreeBuildingWithAdditionalMemory), null));
                else
                    list_of_tasks.Enqueue(new Engine_Task<Decision_Tree, Trees_Parameters>
                    (i, this.list_of_decision_trees[i], new TypeOfTask<Decision_Tree, Trees_Parameters>(Decision_Tree.TreeBuilding), null));

            }
            return list_of_tasks;

        }

        public void CheckTrees()
        {
                foreach (Decision_Tree tree in list_of_decision_trees)
                {
                        Console.WriteLine("\n\nTREE:");
                        tree.ShowResults(tree.root, 0);
                        Console.WriteLine("\n");
                }
            
        }
    }
}
