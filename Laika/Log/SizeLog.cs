using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Laika.Log
{
    class SizeLog : FileLog
    {
        internal SizeLog(FileLogParameter param)
            : base(param)
        {
        
        }

        protected override string GetFileName()
        {
            DateTime now = DateTime.Now;
            string fileName = string.Format(FileNameFormat, Param.FileName, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, Process.GetCurrentProcess().Id);
            
            return fileName;
        }

        protected override bool NeedCreateFile()
        {
            if (WriteSize >= Param.Size)
                return true;

            return false;
        }
        private const string FileNameFormat = "{0}_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}_PID#{7}.log";
    }
}
