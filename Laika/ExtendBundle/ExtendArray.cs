using System;

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
