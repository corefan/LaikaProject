using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.ServerRandom;

namespace TestRandom
{
    class Program
    {
        static void Main(string[] args)
        {
            int rand = ServerRandom.RandRange(0, 100);
        }
    }
}
