using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree
{
    class Decision_Tree
    {
        public Node root { get; set; }
        public Decision_Tree()
        {
            this.root = new Node();
        }

        public static void TreeBuilding(Decision_Tree tree, Trees_Parameters parameters)
        {
            if (tree != null)
                if (tree.root != null)
                {
                        tree.root.ExtendNode();
                }
        }

        public static void TreeBuildingWithAdditionalMemory(Decision_Tree tree, Trees_Parameters parameters)
        {
            if (tree != null)
                if (tree.root != null)
                {
                    tree.root.ExtendNodeWithAdditionalMemory();
                }
        }


        public void ShowResults(Node node, int depth)
        {
            depth++;
            if (node.node_left != null)
            {
                ShowResults(node.node_left, depth);
            }
            if(node != null)
            {
                for (int i = 1; i < depth; i++)
                    Console.Write("-----------");
                Console.Write("Class: ");
                foreach(Category c in node.data.categories)
                {
                    Console.Write("{0} ", c.name);
                }
                Console.Write("  Cond: {0} | {1}", node.condition, node.number_of_feature);
                Console.WriteLine();
            }
            if(node.node_right != null)
            {
                ShowResults(node.node_right, depth);
            }
        }
    }
}
