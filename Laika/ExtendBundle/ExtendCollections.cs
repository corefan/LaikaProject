using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Laika.ExtendBundle
{
    public static class ExtendCollections
    {
        public static bool IsNullOrEmpty(this ICollection collection)
        {
            if (collection == null || collection.Count <= 0)
                return true;
            return false;
        }
    }
}
