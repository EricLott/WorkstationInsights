using System;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace WorkstationInsights.Services
{
    public class SystemMonitorService
    {
        private PerformanceCounter cpuCounter;
        private ComputerInfo computerInfo;

        public SystemMonitorService()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpuCounter.NextValue(); // First call usually 0
                computerInfo = new ComputerInfo();
            }
            catch { /* Skip on counter errors */ }
        }

        public (float CpuUsage, float RamUsage) GetCurrentMetrics()
        {
            try
            {
                float cpuLoad = cpuCounter?.NextValue() ?? 0;
                long totalRam = (long)computerInfo.TotalPhysicalMemory;
                long availableRam = (long)computerInfo.AvailablePhysicalMemory;
                float ramUsedPercent = 100 - ((float)availableRam / totalRam * 100);

                return (cpuLoad, ramUsedPercent);
            }
            catch { return (0, 0); }
        }

        public void Dispose()
        {
            cpuCounter?.Dispose();
        }
    }
}
