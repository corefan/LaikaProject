using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.ThreadPoolManager;

namespace ThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b = AppDomainThreadPoolManager.IsSetMinThreadPool;
            AppDomainThreadPoolManager.SetMinThreadPool(100, 100);
            b = AppDomainThreadPoolManager.IsSetMinThreadPool;
        }
    }
}
