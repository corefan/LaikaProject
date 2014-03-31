using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Laika.Crash
{
    /// <summary>
    /// Dump class
    /// </summary>
    public class MiniDump
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public MiniDump()
        {
            _filePath = Directory.GetCurrentDirectory();
            _fileName = "Minidump";
            _dumpOption = Option.WithFullMemory;
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="filePath">dump file 위치</param>
        /// <param name="fileName">dump file 이름</param>
        /// <param name="dumpOption">dump 옵션</param>
        public MiniDump(string filePath, string fileName, Option dumpOption)
        {
            _filePath = filePath;
            _fileName = fileName;
            _dumpOption = dumpOption;
        }

        private string _filePath;
        private string _fileName;
        private const string _extension = ".mdmp";
        private Option _dumpOption;

        // Taken almost verbatim from http://blog.kalmbach-software.de/2008/12/13/writing-minidumps-in-c/
        [Flags]
        public enum Option : uint
        {
            // From dbghelp.h:
            Normal = 0x00000000,
            WithDataSegs = 0x00000001,
            WithFullMemory = 0x00000002,
            WithHandleData = 0x00000004,
            FilterMemory = 0x00000008,
            ScanMemory = 0x00000010,
            WithUnloadedModules = 0x00000020,
            WithIndirectlyReferencedMemory = 0x00000040,
            FilterModulePaths = 0x00000080,
            WithProcessThreadData = 0x00000100,
            WithPrivateReadWriteMemory = 0x00000200,
            WithoutOptionalData = 0x00000400,
            WithFullMemoryInfo = 0x00000800,
            WithThreadInfo = 0x00001000,
            WithCodeSegs = 0x00002000,
            WithoutAuxiliaryState = 0x00004000,
            WithFullAuxiliaryState = 0x00008000,
            WithPrivateWriteCopyMemory = 0x00010000,
            IgnoreInaccessibleMemory = 0x00020000,
            ValidTypeFlags = 0x0003ffff,
        };

        public enum ExceptionInfo
        {
            None,
            Present
        }
        //typedef struct _MINIDUMP_EXCEPTION_INFORMATION {
        //    DWORD ThreadId;
        //    PEXCEPTION_POINTERS ExceptionPointers;
        //    BOOL ClientPointers;
        //} MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        public struct MiniDumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;

            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }
        //BOOL
        //WINAPI
        //MiniDumpWriteDump(
        //    __in HANDLE hProcess,
        //    __in DWORD ProcessId,
        //    __in HANDLE hFile,
        //    __in MINIDUMP_TYPE DumpType,
        //    __in_opt PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
        //    __in_opt PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
        //    __in_opt PMINIDUMP_CALLBACK_INFORMATION CallbackParam
        //    );

        // Overload requiring MiniDumpExceptionInformation
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, ref MiniDumpExceptionInformation expParam, IntPtr userStreamParam, IntPtr callbackParam);

        // Overload supporting MiniDumpExceptionInformation == NULL
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern uint GetCurrentThreadId();

        public static bool Write(SafeHandle fileHandle, Option options, ExceptionInfo exceptionInfo)
        {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr currentProcessHandle = currentProcess.Handle;
            uint currentProcessId = (uint)currentProcess.Id;
            MiniDumpExceptionInformation exp;
            exp.ThreadId = GetCurrentThreadId();
            exp.ClientPointers = false;
            exp.ExceptionPointers = IntPtr.Zero;
            if (exceptionInfo == ExceptionInfo.Present)
            {
                exp.ExceptionPointers = System.Runtime.InteropServices.Marshal.GetExceptionPointers();
            }

            bool bRet = false;
            if (exp.ExceptionPointers == IntPtr.Zero)
            {
                bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, ref exp, IntPtr.Zero, IntPtr.Zero);
            }
            return bRet;
        }
        /// <summary>
        /// Dump
        /// </summary>
        /// <param name="fileHandle">file handle</param>
        /// <param name="dumpType">dump 타입</param>
        /// <returns>성공 시 true 실패시 false</returns>
        public static bool Write(SafeHandle fileHandle, Option dumpType)
        {
            return Write(fileHandle, dumpType, ExceptionInfo.None);
        }
        /// <summary>
        /// AppDomain.CurrentDomain.UnhandledException 에 등록
        /// </summary>
        public void Register()
        {
            AppDomain.CurrentDomain.UnhandledException += this.DumpHandler;
        }
        /// <summary>
        /// AppDomain.CurrentDomain.UnhandledException 에서 등록 해지
        /// </summary>
        public void UnrRegister()
        {
            AppDomain.CurrentDomain.UnhandledException -= this.DumpHandler;
        }

        private void DumpHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string name = string.Format("{0}_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}#PID{7}{8}", 
                _fileName, 
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 
                Process.GetCurrentProcess().Id,
                _extension);

            string file = Path.Combine(_filePath, name);
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                MiniDump.Write(fs.SafeFileHandle, _dumpOption);
            }
        }
    }
}
