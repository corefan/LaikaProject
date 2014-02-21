using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Laika.Log;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            FileLogTest();
        }

        private static void FileLogTest()
        {
            FileLogParameter param = new FileLogParameter();
            param.FileName = "SPEED";
            param.Type = PartitionType.TIME;
            param.Debug = true;
            int i = 0;
            IFileLog log = FileLogFactory.CreateFileLog(param);

            while (i < 500000)
            {
                log.DEBUG_LOG("Debug");
                log.ERROR_LOG("Error");
                log.FATAL_LOG("fatal");
                log.INFO_LOG("info");
                log.WARNING_LOG("warn");
                i++;
            }
            log.Dispose();
        }
    }
}
