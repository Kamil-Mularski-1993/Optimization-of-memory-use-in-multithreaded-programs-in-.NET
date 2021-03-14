using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Decision_Tree.Data_Structures
{
    class Category
    {
        public string name { get; set; }
        public int quantity { get; set; }

        public Category(string name)
        {
            this.name = name;
            this.quantity = 1;
        }
    }
}
