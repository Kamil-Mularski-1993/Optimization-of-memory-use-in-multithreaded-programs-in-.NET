using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Simple_Allocation
{
    class Simple_Array_Allocation
    {
        private bool allocation { get; set; }

        public Simple_Array_Allocation(bool allocation)
        {
            this.allocation = allocation;
        }
        public Queue<TaskInterface> PrepareForSimpleAllocation(int number_of_tasks)
        {
            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();
            //Dodawanie do kolejki zadań
            if (this.allocation)
                for(int i = 0; i < number_of_tasks; i++)
                    list_of_tasks.Enqueue(new Engine_Task<Simple_Array_Allocation, Simple_Allocation_Parameters>
                        (i, this, new TypeOfTask<Simple_Array_Allocation, Simple_Allocation_Parameters>(SimpleAllocationWithAdditionalMemory), null));
            else
                for (int i = 0; i < number_of_tasks; i++)
                    list_of_tasks.Enqueue(new Engine_Task<Simple_Array_Allocation, Simple_Allocation_Parameters>
                        (i, this, new TypeOfTask<Simple_Array_Allocation, Simple_Allocation_Parameters>(SimpleAllocationWithoutAdditionalMemory), null));


            return list_of_tasks;
        }

        public void SimpleAllocationWithoutAdditionalMemory(Simple_Array_Allocation a, Simple_Allocation_Parameters p)
        {
            try
            {       //Celowo nie użyto pętli
                int[] tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
                tab = new int[1000000];
            }
            catch(Exception)
            {
                Console.WriteLine("\nNie udalo sie zaalokowac tablic - prawdopodobnie przekroczono limit pamieci.\n");
            }
        }

        public void SimpleAllocationWithAdditionalMemory(Simple_Array_Allocation a, Simple_Allocation_Parameters p)
        {
            try
            {       
                for (int i = 0; i < 50; i++)
                {
                    int[] tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                    tab = new int[1000000];
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\nNie udalo sie zaalokowac tablic - prawdopodobnie przekroczono limit pamieci.\n");
            }
        }
    }
}
