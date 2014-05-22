using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laika.Log;

namespace TestLog
{
	class Program
	{
		static void Main(string[] args)
		{
			TestTraceLog();
			//LogParameter param = new LogParameter();
			//param.Debug = true;
			//param.FileName = "ILoveLog";
			////param.Path = "D:\\";
			//param.PrintConsole = true;
			//param.Type = PartitionType.NONE;
			//param.UsingTrace = true;
			////param.Type = PartitionType.FILE_SIZE; // 파일 사이즈가 일정 값이 넘어가면 두번째 파일을 생성함.
			////param.Size = 1000000; // 1,000,000 bytes, 파일 사이즈가 1,000,000 바이트를 넘어가면.
			////param.Type = PartitionType.TIME; // 파일을 생성 후 일정 시간이 넘어가면 두번째 파일을 생성함.
			////param.Time = 10 // 10 min., 10분으로 설정
			////param.Type = PartitionType.WINDOWS_EVENT; 윈도우즈 이벤트 로그
			////param.WindowsEventSource = "TestApplication"; 윈도우즈 이벤트 로그 이벤트 소스

			//// Thread safety함.
			//ILog log = LogFactory.CreateLogObject(param);
			//log.DEBUG_LOG("HELLO {0}", "world"); // param.debug가 false이면 디버그 로그는 남지가 않음.
			//log.ERROR_LOG("ERROR");
			//log.FATAL_LOG("FATAL");
			//log.INFO_LOG("INFO!!");
			//log.WARNING_LOG("WARN!!");

			//ManualResetEvent mre = new ManualResetEvent(false);
			//mre.WaitOne();

			//log.Dispose();
		}

		private static void TestTraceLog()
		{
			LogParameter param = new LogParameter();
			param.OnlyTraceDebugMode = true;
			ILog log = LogFactory.CreateLogObject(param);

			int i = 0;
			while (i < 100)
			{
				log.DEBUG_LOG("hello debug");
				log.ERROR_LOG("hello error");
				log.FATAL_LOG("hello fatal");
				log.INFO_LOG("hello info");
				log.WARNING_LOG("hello warning");
				i++;
			}
			Console.ReadKey();
			log.Dispose();
		}
	}
}
