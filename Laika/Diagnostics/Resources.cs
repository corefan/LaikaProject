using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Laika.Diagnostics
{
    public static class MeasureResource
    {
        public static float GetMachineCpuUsage()
        {
            if (_machineCounter == null)
            {
                _machineCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            
            return _machineCounter.NextValue();
        }

        public static float GetProcessCpuUsage()
        {
            if (_cpuCounter == null)
            {
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _currentProcess.ProcessName);
            }

            return _cpuCounter.NextValue();
        }

        public static long GetWorkingSet64()
        {
            _currentProcess.Refresh();
            return _currentProcess.WorkingSet64;
        }

        private static Process _currentProcess = Process.GetCurrentProcess();
        private static PerformanceCounter _cpuCounter = null;
        private static PerformanceCounter _machineCounter = null;
    }
}
