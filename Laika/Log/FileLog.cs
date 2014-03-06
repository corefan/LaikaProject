using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using Laika.ExtendBundle;

namespace Laika.Log
{
    internal abstract class FileLog : IFileLog
    {
        internal FileLog(FileLogParameter param)
        {
            if (param == null)
                throw new ArgumentNullException();

            Param = param;

            _logQueue = new BlockingCollection<string>();
            _workerThread = new Thread(new ThreadStart(Worker));
            Initialize();
        }

        public void INFO_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging("{0} [INFO] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void DEBUG_LOG(string format, params object[] args)
        {
            if (false == Param.Debug)
                return;

            string log = string.Format(format, args);
            logging("{0} [DEBUG] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void WARNING_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging("{0} [WARNING] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void FATAL_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging("{0} [FATAL] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void ERROR_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging("{0} [ERROR] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        private void logging(string format, params object[] args)
        {
            if (_logQueue.IsAddingCompleted == true)
                return;

            string log = string.Format(format, args);
            _logQueue.TryAdd(log);
        }

        protected abstract string GetFileName();
        protected abstract bool NeedCreateFile();

        private void Initialize()
        {
            FileInitialize();
            _workerThread.Start();
        }

        private void FileInitialize()
        {
            string path = Path.Combine(Param.Path, GetFileName());

            if (_logStreamWriter != null)
            {
                _logStreamWriter.Dispose();
            }

            if (_logFileStream != null)
            {
                _logFileStream.Dispose();
            }

            _logFileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
            _logStreamWriter = new StreamWriter(_logFileStream);
            _logStreamWriter.AutoFlush = true;
        }

        private void Worker()
        {
            string item = null;
            while (_logQueue.TryTake(out item, -1))
            {
                if (item == null)
                    continue;

                if (NeedCreateFile() == true)
                {
                    FileInitialize();
                }

                _logStreamWriter.WriteLine(item);

                if (Param.PrintConsole == true)
                {
                    Console.WriteLine(item);
                }
                if (Param.UsingTrace == true)
                {
                    Trace.WriteLine(item);
                }
            }
        }

        ~FileLog()
        {
            Dispose(false);    
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
            {
                Clear();
            }
            _disposed = true;
        }

        private void Clear()
        {
            if (_logQueue != null)
            {
                _logQueue.CompleteAdding();
            }

            _workerThread.Join();

            if (_logStreamWriter != null)
            {
                _logStreamWriter.Dispose();
            }

            if (_logFileStream != null)
            {
                _logFileStream.Dispose();
            }
        }

        private bool _disposed = false;
        
        private BlockingCollection<string> _logQueue = new BlockingCollection<string>();
        private FileStream _logFileStream;
        private StreamWriter _logStreamWriter;
        private Thread _workerThread;

        protected FileLogParameter Param;
    }
}
