using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.ServerRandom
{
    public static class ServerRandom
    {
        private static Random RandGenerator
        {
            get 
            { 
                return new Random(unchecked((int)DateTime.UtcNow.Ticks));
            }
        }

        public static int RandRange(int min, int max)
        {
            return RandGenerator.Next(min, max);
        }
    }
}
