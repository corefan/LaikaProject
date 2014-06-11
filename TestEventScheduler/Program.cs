using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Laika.Event;

namespace TestEventScheduler
{
    class Person
    {
        public int Index;
    }
    class Program
    {
        static void Main(string[] args)
        {
            TestInstance();
            //TestEvent();
            //TestPerformance1();
            //TestPerformance2();
            //ManualResetEvent mre = new ManualResetEvent(false);
            //mre.WaitOne();
        }

		private static void TestInstance()
		{
			EventScheduler es = new EventScheduler();
			
			for (int j = 0; j < 10; j++)
			{
				Person p = new Person() { Index = j };
				es.AddEvent(j.ToString(), new Event().Every(new TimeSpan(0, 0, 0, 0, 100)).MaxRuns(2), () => { Console.WriteLine(p.Index); });
			}

			Console.ReadKey();
		}

		private static void TestPerformance1()
        {
            Laika.ThreadPoolManager.AppDomainThreadPoolManager.SetMinThreadPool(200, 200);
            EventScheduler es = new EventScheduler();
            int i = 10000;
            while (true)
            {
                try
                {
                    es.AddEvent(
                        (i++).ToString(),
                        new Event().Every(new TimeSpan(0, 0, 1)).StartAt(new TimeSpan(0, 0, 1)).MaxRuns(10),
                        new Action(() =>
                        {
                            //Console.WriteLine("hello");
                        }));

                    //System.Threading.Thread.Sleep(50);
                    if (i > 19999)
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private static void TestPerformance2()
        {
            Laika.ThreadPoolManager.AppDomainThreadPoolManager.SetMinThreadPool(200, 200);
            EventScheduler es = new EventScheduler();
            int i = 0;
            while (true)
            {
                try
                {
                    es.AddEvent(
                        (i++).ToString(),
                        new Event().Every(new TimeSpan(0, 0, 1)).StartAt(new TimeSpan(0, 0, 1)).MaxRuns(10),
                        new Action(() =>
                        {
                            //Console.WriteLine("hello");
                        }));

                    //System.Threading.Thread.Sleep(50);
                    if (i > 9999)
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private static void TestEvent()
        {
            EventScheduler es = new EventScheduler();
            es.AddEvent("hello",
                new Event().
                MaxRuns(10).
                StartAt(new TimeSpan(0, 0, 1)).
                StopAt(new TimeSpan(0, 0, 5)).
                Every(new TimeSpan(0, 0, 1)).MaxRuns(3),
                () => { Console.WriteLine("hello"); }
                );

            Console.ReadKey();
            //ManualResetEvent mre = new ManualResetEvent(false);
            //mre.WaitOne();

            //es.RemoveEvent("hello");
            es.Dispose();
        }
    }
}
