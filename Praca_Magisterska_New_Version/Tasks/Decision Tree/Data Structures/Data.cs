using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures
{
    class Data
    {
        public List<Single_record> list_of_records { get; set; }
        public List<Category> categories { get; set; }
        public Category most_common_category { get; set; }
        public Data()
        {
            this.list_of_records = new List<Single_record>();
            this.categories = new List<Category>();
            this.most_common_category = null;
        }


        public void UpdateCategories()
        {
            this.categories = new List<Category>();
            most_common_category = null;
            foreach(Single_record record in list_of_records)
            {
                Category category = this.categories.Find(x => x.name.Contains(record.category));
                if(category != null)
                {
                    category.quantity++;
                }
                else
                {
                    this.categories.Add(new Category(record.category));
                }
            }

            this.most_common_category = this.categories.First(x => x.quantity == this.categories.Max(t => t.quantity));
        }

        public Tuple<Data, Data> SplitData(int interval_point)
        {
            var data = new Tuple<Data, Data>(new Data(), new Data());

            for (int local_index = 0; local_index < list_of_records.Count; local_index++)   //rozdział danych na dwie grupy
            {

                if (local_index <= interval_point)
                {
                    data.Item1.list_of_records.Add(list_of_records[local_index]);
                    data.Item1.UpdateCategories();

                }
                else if (local_index > interval_point)
                {
                    data.Item2.list_of_records.Add(list_of_records[local_index]);
                    data.Item2.UpdateCategories();
                }
            }

            return data;
        }

        public Data HardCopy()
        {
            Data data = new Data();
            foreach (Single_record rec in this.list_of_records)
            {
                data.list_of_records.Add(new Single_record());
                foreach (string word in rec.record)
                {
                    data.list_of_records[data.list_of_records.Count() - 1].record.Add(word);
                }
                data.list_of_records[data.list_of_records.Count() - 1].category = rec.category;
            }

            data.UpdateCategories();

            return data;
        }

        public Data SoftCopy()
        {
            Data data = new Data();
            foreach (Single_record rec in this.list_of_records)
            {
                data.list_of_records.Add(rec);
            }

            data.UpdateCategories();

            return data;
        }

        public void LoadData(string file)
        {

            int number_of_data = 0;
            int number_of_features = 0;
            int start_line = 0;
            int class_column = -1;

            bool read_records = true;

            using (var sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line.IndexOf("GENERAL", StringComparison.CurrentCultureIgnoreCase) >= 0) read_records = false;

                    if (read_records)
                    {
                        string[] tmp = line.Split();
                        number_of_features = tmp.Length;
                        number_of_data++;
                    }
                    else
                    {
                        if (line.IndexOf("CLASS POSITION", StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            string[] tmp = line.Split('=');
                            class_column = Int32.Parse(tmp[1]) - 1;
                        }
                        if (line.IndexOf("DATA", StringComparison.CurrentCultureIgnoreCase) >= 0) { read_records = true; start_line++; } else start_line++;
                    }
                }
            }

            string[,]  table_of_records = new string[number_of_data, number_of_features];

            using (var sr = new StreamReader(file))
            {
                int i = 0;
                int j = 0;
                string x;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (i >= start_line)
                    {
                        string[] tmp = line.Split();
                        for (int k = 0; k < number_of_features; k++)
                        {
                            table_of_records[j, k] = tmp[k];
                        }
                        if (class_column != -1)
                        {
                            x = table_of_records[j, class_column];
                            table_of_records[j, class_column] = table_of_records[j, number_of_features - 1];
                            table_of_records[j, number_of_features - 1] = x;
                        }
                        j++;
                    }
                    i++;
                }
            }

            for (int i = 0; i < number_of_data; i++)
            {
                list_of_records.Add(new Single_record(table_of_records, i));
            }

        }
    }
}
