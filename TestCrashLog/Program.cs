using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Crash;

namespace TestCrashLog
{
    class Program
    {
        static void Main(string[] args)
        {
            // 핸들러 생성
            CrashHandler handler = new CrashHandler(null, "crash_log");
            // 핸들러 등록
            handler.RegisterCrashLogWriter();
            SetMiniDump();
            // 만약 catch 되지 않은 예외가 발생할 경우 예외를 로그에 남기고 프로세스는 종료가 됨.
            throw new Exception("fds");
            
            // 핸들러 등록 해지
            handler.UnRegisterCrashLogWriter();
        }

        private static void SetMiniDump()
        {
            MiniDump m = new MiniDump();
            // MiniDump m = new MiniDump(System.IO.Directory.GetCurrentDirectory(), "Dump", MiniDump.Option.WithFullMemory);
            m.Register();
            // m.UnrRegister();
        }
    }
}
