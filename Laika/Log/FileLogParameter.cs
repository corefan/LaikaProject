using System.IO;
namespace Laika.Log
{
    public class FileLogParameter
    {
        public FileLogParameter(string path = null, PartitionType type = PartitionType.NONE, string fileName = "log", int time = 60, int size = 1024000, bool debug = false, bool console = false, bool trace = false, string windowsEventSource = "ApplicationSource")
        {
            if (path == null)
                path = Directory.GetCurrentDirectory();

            Type = type;
            Path = path;
            FileName = fileName;
            Debug = debug;
            Time = time;
            Size = size;
            PrintConsole = console;
            UsingTrace = trace;
            WindowsEventSource = windowsEventSource;
        }

        /// <summary>
        /// 파일 로그 분할 타입
        /// </summary>
        public PartitionType Type { get; set; }
        /// <summary>
        /// 파일 저장 위치
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 파일 이름
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 분할 타입이 시간일 경우 단위 시간(분)
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 분할 타입이 파일 사이즈 일 경우 단위(바이트)
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 디버그 로그 기록 여부
        /// </summary>
        public bool Debug { get; set; }
        /// <summary>
        /// 콘솔 출력 여부
        /// </summary>
        public bool PrintConsole { get; set; }
        /// <summary>
        /// Trace 사용 여부
        /// </summary>
        public bool UsingTrace { get; set; }
        /// <summary>
        /// Event log source
        /// </summary>
        public string WindowsEventSource { get; set; }
    }
}
