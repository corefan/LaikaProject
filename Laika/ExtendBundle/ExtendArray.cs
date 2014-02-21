using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.ExtendBundle
{
    public static class ExtendArray
    {
        public static bool IsNullOrZeroLengh(this Array arr)
        {
            if (arr == null || arr.Length <= 0)
                return true;
            return false;
        }
    }
}
