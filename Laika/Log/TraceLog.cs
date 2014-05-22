using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Log
{
	class TraceLog : LaikaLog
	{
		internal TraceLog(LogParameter param)
			: base(param)
		{

		}

		protected override bool NeedCreateFile()
		{
			return false;
		}

		protected override string GetFileName()
		{
			return null;
		}
	}
}
