using System;
using System.Diagnostics;

namespace Laika.Log
{
	class TimeLog : LaikaLog
	{
		internal TimeLog(LogParameter param)
			: base(param)
		{
			_startTime = DateTime.Now;
			_period = new TimeSpan(0, param.Time, 0);
		}

		protected override string GetFileName()
		{
			DateTime now = DateTime.Now;
			return string.Format(FileNameFormat, Param.FileName, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, Process.GetCurrentProcess().Id);
		}

		protected override bool NeedCreateFile()
		{
			DateTime now = DateTime.Now;
			if (now.Subtract(_startTime) >= _period)
			{
				_startTime = DateTime.Now;
				return true;
			}
			return false;
		}

		private readonly TimeSpan _period;
		private DateTime _startTime;
		private const string FileNameFormat = "{0}_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}_PID#{7}.log";
	}
}
