using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Laika.Log;

namespace Laika.Crash
{
    /// <summary>
    /// catch 되지 않은 Exception Handling
    /// </summary>
    public class CrashHandler
    {
        private IFileLog _crashLog;
        private FileLogParameter _parameter;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">log file name prefix</param>
        public CrashHandler(string fileName)
            : this (null, "CrashLog")
        { }
        /// <summary>
        /// Constructor
        /// </summary>
        public CrashHandler()
            : this (null, "CrashLog")
        { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">log file path</param>
        /// <param name="fileName">log file name prefix</param>
        public CrashHandler(string filePath, string fileName)
        {
            _parameter = new FileLogParameter();
            _parameter.Debug = true;
            _parameter.FileName = fileName;
            if(filePath != null)
                _parameter.Path = filePath;
        }
        /// <summary>
        /// Register handler
        /// </summary>
        public void RegisterCrashLogWriter()
        {
            AppDomain.CurrentDomain.UnhandledException += CrashExceptionHandler;
        }
        /// <summary>
        /// Unregister handler
        /// </summary>
        public void UnRegisterCrashLogWriter()
        {
            AppDomain.CurrentDomain.UnhandledException -= CrashExceptionHandler;
        }

        private void CrashExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            _crashLog = FileLogFactory.CreateFileLog(_parameter);
            Exception ex = (Exception)e.ExceptionObject;
            _crashLog.FATAL_LOG("Exception Type [{0}]", ex.GetType().ToString());
            _crashLog.FATAL_LOG(ex.ToString());
            _crashLog.Dispose();
        }
    }
}
