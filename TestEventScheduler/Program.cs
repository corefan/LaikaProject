using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Laika.Event;

namespace TestEventScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            EventScheduler es = new EventScheduler();
            es.AddEvent("hello",
                new Event().
                MaxRuns(10).
                StartAt(new TimeSpan(0, 0, 1)).
                StopAt(new TimeSpan(0, 0, 5)).
                Every(new TimeSpan(0, 0, 1)),
                () => { Console.WriteLine("hello"); }
                );
            
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne();
        }
    }
}
