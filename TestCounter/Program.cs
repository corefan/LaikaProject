using System;
using System.Collections.Generic;
using System.Text;
using Laika.Diagnotics;

namespace TestCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Counter c = new Counter(new TimeSpan(0, 0, 10));
            while (true)
            {
                Console.WriteLine(c.Increment());
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
