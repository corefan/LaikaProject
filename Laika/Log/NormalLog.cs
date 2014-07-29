using System.Diagnostics;

namespace Laika.Log
{
	class NormalLog : LaikaLog
	{
		internal NormalLog(LogParameter param)
			: base(param)
		{

		}

		protected override bool NeedCreateFile()
		{
			return false;
		}

		protected override string GetFileName()
		{
			return string.Format(FileNameFormat, Param.FileName, Process.GetCurrentProcess().Id);
		}

		private const string FileNameFormat = "{0}_PID#{1}.log";
	}
}
