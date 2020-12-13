using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersbrodBot
{
    class TPLConsole
    {
        public static void ConsoleWrite(string message)
        {
            Task.Run(() => 
            {
                Console.WriteLine(message);
            }); 
        }

        public static void ConsoleWrite(int message)
        {
            Task.Run(() =>
            {
                Console.WriteLine(message);
            });
        }
        public static void ConsoleWrite(string message, ConsoleColor color)
        {
            Task.Run(() =>
            {
                ConsoleColor lastColor = Console.ForegroundColor;

                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = lastColor;
            });
        }

        public static void ConsoleWrite(int message, ConsoleColor color)
        {
            Task.Run(() =>
            {
                ConsoleColor lastColor = Console.ForegroundColor;

                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = lastColor;
            });
        }
    }
}
