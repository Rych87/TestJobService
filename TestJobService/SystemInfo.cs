using System.Diagnostics;
using System.Management;
using System.Net;

namespace TestJobService
{
    internal static class SystemInfo
    {
        internal static InfoModel GetInfo() 
        {
            InfoModel ret = new InfoModel();
            ret.HostName = Dns.GetHostName();
            ret.IP = Dns.GetHostAddresses(ret.HostName).FirstOrDefault(_ => _.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString(); // Берём только IPv4 при необходимости можно переписать для IPv6 или писать оба
            ret.DateTime = DateTime.Now;

            ret.CpuUsage().MemoryUsage().FreeDiskSpace().Processes().WinVersion();

            return ret;
        }

        internal static InfoModel CpuUsage(this InfoModel model)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor"))
                if (int.TryParse(searcher.GetInfo("LoadPercentage"), out int res))
                    model.CpuLoad = res;
            return model;
        }

        internal static InfoModel MemoryUsage(this InfoModel model)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem"))
                if (ulong.TryParse(searcher.GetInfo("TotalVisibleMemorySize"), out ulong total)
                    && ulong.TryParse(searcher.GetInfo("FreePhysicalMemory"), out ulong free))
                {
                    ulong used = total - free;
                    model.MemoryUsage = (int)((double)used / total * 100.0);    //если важны доли процентов, можно переделать в float/double
                }
            return model;
        }

        internal static InfoModel FreeDiskSpace(this InfoModel model)
        {
            ulong totalFree = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady)
                    continue;
                totalFree += (ulong)drive.AvailableFreeSpace;
            }

            model.FreeDiskSpace = totalFree / 1024 / 1024;  // В мегабайтах
            return model;
        }

        internal static InfoModel Processes(this InfoModel model)
        {
            model.Processes = Process.GetProcesses().Select(_ => _.ProcessName).Distinct().ToArray();
            return model;
        }

        internal static InfoModel WinVersion(this InfoModel model)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT Caption, Version, BuildNumber FROM Win32_OperatingSystem"))
                model.WinVersion = searcher.GetInfo("Caption") + " " + searcher.GetInfo("Version") + " " + searcher.GetInfo("BuildNumber");
            return model;
        }

        private static string GetInfo(this ManagementObjectSearcher searcher, string propertyName)
        {
            using (var res = searcher.Get())
                return res.OfType<ManagementObject>()?.FirstOrDefault()?[propertyName]?.ToString() ?? string.Empty;
        }
    }
}
