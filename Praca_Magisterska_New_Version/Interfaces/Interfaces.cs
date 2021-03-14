using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optymalizacja_wykorzystania_pamieci.Diagnostics;

namespace Optymalizacja_wykorzystania_pamieci.Interfaces
{
    interface TaskInterface
    {
        void Execute(Diagnostician diag);
    }
}
