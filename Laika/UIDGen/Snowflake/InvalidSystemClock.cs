using System;

namespace Laika.Snowflake
{
    public class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}