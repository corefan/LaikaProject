using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Laika.ExtendBundle;

namespace Laika.Log
{
    public class EventLog : IFileLog
    {
        private FileLogParameter _param;
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
            { }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void INFO_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging(EventLogEntryType.Information, "{0} [INFO] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void DEBUG_LOG(string format, params object[] args)
        {
            if (false == _param.Debug)
                return;

            string log = string.Format(format, args);
            logging(EventLogEntryType.Information, "{0} [DEBUG] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void WARNING_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging(EventLogEntryType.Warning, "{0} [WARNING] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void FATAL_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging(EventLogEntryType.FailureAudit, "{0} [FATAL] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        public void ERROR_LOG(string format, params object[] args)
        {
            string log = string.Format(format, args);
            logging(EventLogEntryType.Error, "{0} [ERROR] {1}", DateTime.Now.GetLogDateTimeString(), log);
        }

        private void logging(EventLogEntryType type, string format, params object[] args)
        {
            Task.Factory.StartNew(() => 
            {
                string message = string.Format(format, args);
                System.Diagnostics.EventLog.WriteEntry(_param.WindowsEventSource, message, type);
                if (_param.UsingTrace == true)
                    Trace.WriteLine(message);

                if (_param.PrintConsole == true)
                    Console.WriteLine(message);
            });
        }
        internal EventLog(FileLogParameter param)
        {
            if (param == null)
                throw new ArgumentNullException();

            _param = param;
        }
    }
}
