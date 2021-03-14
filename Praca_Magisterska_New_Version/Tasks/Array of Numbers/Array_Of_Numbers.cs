using Optymalizacja_wykorzystania_pamieci.Diagnostics;
using Optymalizacja_wykorzystania_pamieci.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optymalizacja_wykorzystania_pamieci.Tasks.Array_of_Numbers
{
    class Array_Of_Numbers
    {
        public int[] numbers { get; set; }
        private int[] array_secondary { get; set; }
        private Array_Parameters[] parameters { get; set; }
        private bool allocation { get; set; }
        public Array_Of_Numbers(int size, bool allocation)
        {
            this.numbers = new int[size];
            Random rand = new Random();
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = rand.Next(100000);
            }
            this.allocation = allocation;
            this.array_secondary = new int[size];
        }

        public Queue<TaskInterface> PrepareForSort(int number_of_tasks, Diagnostician diag)
        {
            Queue<TaskInterface> list_of_tasks = new Queue<TaskInterface>();
            this.parameters = new Array_Parameters[number_of_tasks];

            int whole = numbers.Length / number_of_tasks;
            int rest = numbers.Length % number_of_tasks;

            int param_1 = 0;
            int param_2 = whole + 1;
            for (int i = 0; i < number_of_tasks; i++)
            {
                if (i > 0) param_1 = param_1 + param_2;

                param_2 = whole + 1;

                if (rest < i + 1) param_2--;

                this.parameters[i] = new Array_Parameters(param_1, param_2);

                //Dodawanie do kolejki zadań
                list_of_tasks.Enqueue(new Engine_Task<Array_Of_Numbers, Array_Parameters>
                    (i, this, new TypeOfTask<Array_Of_Numbers, Array_Parameters>(SortingSelection), parameters[i]));
                
            }
            return list_of_tasks;
        }

        private void SortingSelection(Array_Of_Numbers array_original, Array_Parameters parameters)
        {

            if (array_original.allocation)
            {
                MergeSort(array_original.numbers, parameters.start_index, parameters.start_index + parameters.number_of_numbers - 1, array_original.array_secondary, 0, true);
            }
            else
            {
                MergeSort(array_original.numbers, parameters.start_index, parameters.start_index + parameters.number_of_numbers - 1, array_original.array_secondary, 0, false);
            }
        }

        private static void MergeSort(int[] array_of_numbers, int begin, int end, int[] array_secondary, int start_index, bool merge)
        {
            if (begin < end)
            {
                //podziel sortowana tablice na pola
                MergeSort(array_of_numbers, begin, (begin + end) / 2, array_secondary, start_index, merge);
                MergeSort(array_of_numbers, (begin + end) / 2 + 1, end, array_secondary, start_index, merge);
                //scal posortowane tablice
                if(merge)
                    MergingWithAllocation(array_of_numbers, begin, end, array_secondary, start_index);
                else
                    Merging(array_of_numbers, begin, end, array_secondary, start_index);
            }
        }

        private static void Merging(int[] array_of_numbers, int begin, int end, int[] array_secondary, int start_index)
        {
            //Skopiuj wartosci do tablicy pomocniczej
            for (int i = begin; i <= end; i++)
            {
                array_secondary[i-start_index] = array_of_numbers[i];
            }

            //Scalaj tablice
            int p = begin;
            int q = (begin + end) / 2 + 1;
            int r = begin;
            while (p <= (begin + end) / 2 && q <= end)
            {
                if (array_secondary[p-start_index] < array_secondary[q-start_index])
                {
                    array_of_numbers[r] = array_secondary[p-start_index];
                    r++;
                    p++;
                }
                else
                {
                    array_of_numbers[r] = array_secondary[q-start_index];
                    r++;
                    q++;
                }
            }

            //Przepisz koncowke
            while (p <= (begin + end) / 2)
            {
                array_of_numbers[r] = array_secondary[p-start_index];
                r++;
                p++;
            }
        }


        private static void MergingWithAllocation
            (int[] array_of_numbers, int begin, int end, int[] array_secondary, int start_index){
            for (int i = begin; i <= end; i++)
            {
                array_secondary[i - start_index] = array_of_numbers[i];
            }

            int[] array_allocated = new int[end - begin + 1];

            int p = begin;
            int q = (begin + end) / 2 + 1;
            int r = begin;
            while (p <= (begin + end) / 2 && q <= end)
            {
                if (array_secondary[p - start_index] < array_secondary[q - start_index])
                {
                    array_allocated[r - begin] = array_secondary[p - start_index];
                    array_of_numbers[r] = array_allocated[r - begin];
                    r++;
                    p++;
                }
                else
                {
                    array_allocated[r - begin] = array_secondary[q - start_index];
                    array_of_numbers[r] = array_allocated[r - begin];
                    r++;
                    q++;
                }
            }

            while (p <= (begin + end) / 2)
            {
                array_allocated[r - begin] = array_secondary[p - start_index];
                array_of_numbers[r] = array_allocated[r - begin];
                r++;
                p++;
            }
        }
        //-----------------------------------------------------------------------Finalizacja----------------------------------------------------------------
        public void Finalization()
        {
            this.HeapSort();
        }

        //-------------------------------------------------------------------------Metody kopca--------------------------------------------------------------
        private void HeapSort()
        {
            Array.Copy(this.numbers, this.array_secondary, this.numbers.Count());

            int[,] heap = new int[parameters.Length, 2];
            int n = 0;

            for (int i = 0; i < parameters.Length; i++)
            {
                HeapAdd(heap, -this.array_secondary[parameters[i].start_index], i, ref n);
            }
          
            int index = 0;

            for(int i = 0; i < this.numbers.Count(); i++)
            {
                this.numbers[i] = -heap[0, 0];
                index = heap[0, 1];
                HeapRemove(heap, ref n);
                parameters[index].start_index++;
                parameters[index].number_of_numbers--;
                if (parameters[index].number_of_numbers > 0) HeapAdd(heap, -this.array_secondary[parameters[index].start_index], index, ref n);
            }


        }
        private static void HeapAdd(int[,] heap, int value, int part_number, ref int n)
        {
            int i, j;

            i = n++;
            j = (i - 1) / 2;

            while (i > 0 && heap[j, 0] < value)
            {
                heap[i, 0] = heap[j, 0];
                heap[i, 1] = heap[j, 1];
                i = j;
                j = (i - 1) / 2;
            }

            heap[i, 0] = value;
            heap[i, 1] = part_number;
        }

        private static void HeapRemove(int[,] heap, ref int n)
        {
            int i, j;
            int[] v = new int[2];

            if (n-- > 0)
            {
                v[0] = heap[n, 0];
                v[1] = heap[n, 1];

                i = 0;
                j = 1;

                while (j < n)
                {
                    if (j + 1 < n && heap[j + 1, 0] > heap[j, 0]) j++;
                    if (v[0] >= heap[j, 0]) break;
                    heap[i, 0] = heap[j, 0];
                    heap[i, 1] = heap[j, 1];
                    i = j;
                    j = 2 * j + 1;
                }

                heap[i, 0] = v[0];
                heap[i, 1] = v[1];
            }
        }

        public void ShowResults()
        {
            Console.WriteLine("\nFragment tablicy po sortowaniu:\n");
            int i = 0;
            while(i < 200 && i < this.numbers.Length)
            {
                Console.Write("{0}, ", this.numbers[i]);
                i++;
            }
            Console.WriteLine();
        }
     
    }
}
