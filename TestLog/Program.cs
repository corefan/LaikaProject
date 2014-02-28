using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Log;

namespace TestLog
{
    class Program
    {
        static void Main(string[] args)
        {
            FileLogParameter param = new FileLogParameter();
            param.Debug = true;
            param.FileName = "ILoveLog";
            //param.Path = "D:\\";
            param.PrintConsole = true;
            param.Type = PartitionType.NONE;
            //param.Type = PartitionType.FILE_SIZE; // 파일 사이즈가 일정 값이 넘어가면 두번째 파일을 생성함.
            //param.Size = 1000000; // 1,000,000 bytes, 파일 사이즈가 1,000,000 바이트를 넘어가면.
            //param.Type = PartitionType.TIME; // 파일을 생성 후 일정 시간이 넘어가면 두번째 파일을 생성함.
            //param.Time = 10 // 10 min., 10분으로 설정

            // Thread safety함.
            IFileLog log = FileLogFactory.CreateFileLog(param);
            log.DEBUG_LOG("HELLO {0}", "world"); // param.debug가 false이면 디버그 로그는 남지가 않음.
            log.ERROR_LOG("ERROR");
            log.FATAL_LOG("FATAL");
            log.INFO_LOG("INFO!!");
            log.WARNING_LOG("WARN!!");

            log.Dispose();
        }
    }
}
