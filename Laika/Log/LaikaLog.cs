using System;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using Laika.ExtendBundle;

namespace Laika.Log
{
	internal abstract class LaikaLog : ILog
	{
		internal delegate void LogAppenderHandle(string log);
		internal event LogAppenderHandle LogAppender;

		internal LaikaLog(LogParameter param)
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
			if (Param.OnlyTraceDebugMode == false)
			{
				LogAppender += this.WriteFile;
				FileInitialize();
			}

			if (Param.PrintConsole == true)
			{
				LogAppender += this.WriteConsole;
			}
			if (Param.UsingTrace == true)
			{
				LogAppender += this.WriteTrace;
			}

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

		private void WriteTrace(string log)
		{
			Trace.WriteLine(log);
		}

		private void WriteConsole(string log)
		{
			Console.WriteLine(log);
		}

		private void WriteFile(string log)
		{
			if (NeedCreateFile() == true)
			{
				FileInitialize();
				WriteSize = 0;
			}

			_logStreamWriter.WriteLine(log);

			if (Param.Type == PartitionType.FILE_SIZE)
				WriteSize += (log.LongCount() + 1);
		}

		private void Worker()
		{
			string item = null;
			while (_logQueue.TryTake(out item, -1))
			{
				if (item == null)
					continue;

				LogAppender(item);
			}
		}

		~LaikaLog()
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

		protected LogParameter Param;
		protected long WriteSize = 0;
	}
}
