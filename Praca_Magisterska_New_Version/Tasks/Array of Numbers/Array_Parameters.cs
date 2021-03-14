using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Array_of_Numbers
{
    class Array_Parameters
    {
        public int start_index { get; set; }
        public int number_of_numbers { get; set; }
        public Array_Parameters(int start_index, int number_of_numbers)
        {
            this.start_index = start_index;
            this.number_of_numbers = number_of_numbers;
        }
    }
}
