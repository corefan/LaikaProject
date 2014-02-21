using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Laika.Log
{
    class NormalLog : FileLog
    {
        internal NormalLog(FileLogParameter param)
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
