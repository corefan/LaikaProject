using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.UIDGen;

namespace TestUIDGen
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++ )
                Console.WriteLine(UniqueIDGenerator.GetID());
            Console.ReadKey();
        }
    }
}
