using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.ThreadPoolManager
{
    public class AppDomainThreadPoolManager
    {
        /// <summary>
        /// 최소 ThreadPool 개수 설정. Task, Timer, Laika mysql connection pool 개수를 고려하여 설정.
        /// </summary>
        /// <param name="minWorkerThreads">최소 워커 스레드 개수</param>
        /// <param name="minCompletionPortThreads">최소 completion port 스레드 개수</param>
        public static void SetMinThreadPool(int minWorkerThreads, int minCompletionPortThreads)
        {
            System.Threading.ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);
            IsSetMinThreadPool = true;
        }

        public static bool IsSetMinThreadPool { get; private set; }
    }
}
