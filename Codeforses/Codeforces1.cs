using System;
using System.Collections.Generic;

namespace Codeforses
{
    class Program
    {
        static void Main(string[] args)
        {
            int q = Int32.Parse(Console.ReadLine());

            Dictionary<int, string> nk = new Dictionary<int, string>();
            List<int> answers = new List<int>();

            for(int j = 0; j < q; j++)
            {
                nk.Add(j, Console.ReadLine());
            }

            foreach (var item in nk)
            {
                string[] items = item.Value.Split(' ');
                int k = Int32.Parse(items[0]);
                int n = Int32.Parse(items[1]);
                int a = Int32.Parse(items[2]);
                int b = Int32.Parse(items[3]);

                int current_n = 0;
                bool ableToWhile = true;

                int ans = 0;

                if ((k / b) >= n)
                {
                    do
                    {
                        bool locked = false;
                        if (k > a && !locked)
                        { 
                            ans++;
                            k -= a;
                            locked = true;
                        }
                        else if (k > b && !locked)
                        {
                            k -= b;
                            locked = true;
                        }
                        else if (k <= Math.Min(a,b) && !locked)
                        {
                            ans = -1;
                            locked = true;
                        }

                        current_n++;
                        if (current_n == n)
                        {
                            ableToWhile = false;
                            answers.Add(ans);
                        }
                    }
                    while (ableToWhile);
                }
                else
                {
                    answers.Add(-1);
                }
            }

            foreach(var item in answers)
            {
                Console.WriteLine(item);
            }
        }
    }
}
