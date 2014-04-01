using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.UIDGen
{
    public static class UniqueIDGenerator
    {
        public static long GetID()
        {
            return _idWorker.NextId();
        }
        private static Laika.Snowflake.IdWorker _idWorker = new Snowflake.IdWorker(0, 0);
    }
}
