using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Files
{
    class Files
    {
        public string path { get; set; }     
        public List<File_Entry> list_of_posts { get; set; }
        public string word { get; set; }
        private List<string> list_of_files { get; set; }

        private object blockade = new object();

        public Files(string path, string word)
        {
            this.path = path;
            this.word = word;
            this.list_of_files = new List<string>();
            this.list_of_posts = new List<File_Entry>();
            this.blockade = new object();
        }

        public Queue<TaskInterface> FileSearch(Diagnostician diag)
        {
            Directory.GetFiles(this.path);
            this.list_of_files = Directory.GetFiles(this.path).ToList<string>();

            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();

            for (int i = 0; i < this.list_of_files.Count(); i++)
            {
                list_of_tasks.Enqueue(new Engine_Task<Files, int>(i, this, new TypeOfTask<Files, int>(FindWord), i));
            }

            return list_of_tasks;
        }

        private void FindWord(Files files, int which_file)
        {
            using (var sr = new StreamReader(files.list_of_files[which_file]))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (String.IsNullOrEmpty(line)) continue;
                    if (line.IndexOf(files.word, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        try
                        {
                            File_Entry registration = files.list_of_posts.Find(x => x.file_name == files.list_of_files[which_file]);
                            registration.line_numbers.Add(i);
                        }
                        catch
                        {
                            lock (blockade)
                            {
                                files.list_of_posts.Add(new File_Entry(files.list_of_files[which_file], i));
                            }
                        }
                    }
                    i++;
                }
            }
        }

        public static void AlignFilesToTasks(int number_of_tasks, string path)
        {
            
            List<string> files = Directory.GetFiles(path).ToList<string>();
            int existed_files = files.Count();
            int index = 0;
            Random rand = new Random();
            Console.WriteLine("-- {0}", existed_files, number_of_tasks);
            try
            {
                if (existed_files < number_of_tasks)
                {
                    for (int i = existed_files; i < number_of_tasks; i++)
                    {
                        File.Copy(files[index], Path.Combine(path, i.ToString() + rand.Next(1000000).ToString() + ".txt"), true);
                        index++;
                        if (index == existed_files) index = 0;
                    }
                }
                else if (existed_files > number_of_tasks)
                {
                    for (int i = number_of_tasks; i < existed_files; i++)
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Napotkano problem z dostosowaniem liczby plikow do zadan");
                Console.ReadKey();
            }

        }

        public void ShowResults()
        {
            Console.WriteLine("ILOSC WPISOW: {0}", this.list_of_posts.Count());
            foreach (File_Entry post in this.list_of_posts)
            {
                Console.Write("W pliku {0} wyraz {1} pojawil sie w linii: ", post.file_name, this.word);
                foreach (int i in post.line_numbers)
                {
                    Console.Write("{0} ", i);
                }
                Console.WriteLine();
            }
            foreach (string s in this.list_of_files)
            {
                Console.WriteLine("{0}", s);
            }
           
        }
    }
}
