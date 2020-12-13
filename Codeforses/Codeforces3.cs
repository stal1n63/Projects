using System;

namespace Codeforses
{
    class Program
    {
        static void Main(string[] args)
        {
            string _numberToCheck = Console.ReadLine();

            int result;
            Int32.TryParse(_numberToCheck, out result);

            bool isWork = true;
            do
            {
                string localResult = result.ToString();
                char[] symbols = localResult.ToCharArray();

                int sum = 0;
                foreach (var ch in symbols)
                {
                    sum += Int32.Parse(ch.ToString());
                }

                if (sum % 4 == 0)
                {
                    isWork = false;
                    Console.WriteLine(result);
                }
                else
                {
                    result++;
                }
            }
            while (isWork);
        }
    }
}

/*
Поликарп знает, что если сумма цифр числа делится на 3, то и само число делится на 3. Он предполагает, что числа, сумма цифр которых делится на 4, тоже в чём-то интересные. Таким образом, он считает положительное целое число n интересным, если его сумма цифр делится на 4.

Помогите Поликарпу найти ближайшее большее или равное интересное число по заданному числу a. То есть, найдите такое интересное число n, что n≥a и n — минимально.

Входные данные
В единственной строке входных данных записано целое число a (1≤a≤1000).

Выходные данные
Выведите ближайшее большее или равное интересное число по заданному числу a. Иными словами, выведите такое интересное число n, что n≥a и n — минимально.
*/
