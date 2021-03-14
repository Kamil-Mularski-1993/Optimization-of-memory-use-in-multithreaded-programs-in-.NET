using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree
{
    class Node
    {
        public int number_of_feature { get; set; }
        public string condition { get; set; }
        public Node node_left { get; set; }
        public Node node_right { get; set; }
        public Data data { get; set; }
        public Node():this(new Data())
        {

        }
        public Node(Data data)
        {
            this.data = data;
            this.node_left = null;
            this.node_right = null;
            this.condition = null;
            this.number_of_feature = -1;
        }
        


        public void ExtendNode()
        {
            int number_of_features = 0;
            if (data.list_of_records != null)
                number_of_features = data.list_of_records[0].record.Count;
          
            if (this.data.categories.Count > 1)
            {      //jeżeli są różne klasy
                for (int current_feature = 0; current_feature < number_of_features; current_feature++)      //poruszanie się względem cechy
                {

                    data.list_of_records.Sort(new Comparision_List(current_feature));
  
                    for (int i = 0; i < data.list_of_records.Count() - 1; i++)        //porównywanie poszczególnych cech
                    {
                        if (String.Compare(data.list_of_records[i].record[current_feature], data.list_of_records[i + 1].record[current_feature]) != 0)    //jeśli cechy różne
                        {

                            var datas = this.data.SplitData(i);

                            this.SplitNode(new Node(datas.Item1), new Node(datas.Item2), i, current_feature);

                        }
                    }
                }
            }

            if (this.node_left != null)                       //rekurencja
                this.node_left.ExtendNode();
            if (this.node_right != null)
                this.node_right.ExtendNode();
        }

        public void ExtendNodeWithAdditionalMemory()
        {
            int number_of_features = 0;
            if (data.list_of_records != null)
                number_of_features = data.list_of_records[0].record.Count;

            if (this.data.categories.Count > 1)
            {      //jeżeli są różne klasy
                for (int current_feature = 0; current_feature < number_of_features; current_feature++)      //poruszanie się względem cechy
                {

                    data.list_of_records.Sort(new Comparision_List(current_feature));

                    for (int i = 0; i < data.list_of_records.Count() - 1; i++)        //porównywanie poszczególnych cech
                    {
                        if (String.Compare(data.list_of_records[i].record[current_feature], data.list_of_records[i + 1].record[current_feature]) != 0)    //jeśli cechy różne
                        {

                            var datas = this.data.SplitData(i);
                            var datas_2 = this.data.SplitData(i);

                            this.SplitNode(new Node(datas.Item1), new Node(datas.Item2), i, current_feature);
                            this.SplitNode(new Node(datas.Item1), new Node(datas.Item2), i, current_feature);
                        }
                    }
                }
            }

            if (this.node_left != null)                       //rekurencja
                this.node_left.ExtendNode();
            if (this.node_right != null)
                this.node_right.ExtendNode();
        }

        public void SplitNode(Node node_1, Node node_2, int index, int current_feature)
        {

            string tmp_condition = data.list_of_records[index].record[current_feature];   //wartość cechy branej do sprawdzania
            int tmp_number_of_feature = current_feature;               //pozycja cechy brana do sprawdzania

            if (this.node_left != null || this.node_right != null)
            {
                int a = this.node_left.data.list_of_records.Count() - this.node_left.data.most_common_category.quantity + this.node_right.data.list_of_records.Count() - this.node_right.data.most_common_category.quantity;
                int b = node_1.data.list_of_records.Count() - node_1.data.most_common_category.quantity + node_2.data.list_of_records.Count() - node_2.data.most_common_category.quantity;

                if(a > b)
                {
                    this.condition = tmp_condition;
                    this.number_of_feature = tmp_number_of_feature;
                    node_left = node_1;
                    node_right = node_2;
                }
            }
            else
            {
                this.condition = tmp_condition;
                this.number_of_feature = tmp_number_of_feature;
                node_left = node_1;
                node_right = node_2;
            }           
        }
    }
}
