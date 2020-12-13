using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LiteNetLib;
using System.Collections;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> stack = new List<int>();
            int chislo = 2018;

            for(int i = 0; stack.Count < chislo; ++i)
            {
                stack.Add(i);
            }

            List<double> tangens = new List<double>();

            foreach(int j in stack)
            {
                tangens.Add(Math.Tan(10^stack[j]));
            }

            int negativeCount = 0;

            foreach(double value in tangens)
            {
                if(value < 0)
                {
                    negativeCount += 1;
                }
            }

            Console.WriteLine(stack.Count + "\r\n" + negativeCount);
            Console.ReadKey();
        }
    }
}
