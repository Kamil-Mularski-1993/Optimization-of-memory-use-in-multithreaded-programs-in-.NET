using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Optymalizacja_wykorzystania_pamieci
{
    public delegate void TypeOfTask<TObject, TParameters>
        (TObject Tobject, TParameters Tlist_of_parameters);
   
}