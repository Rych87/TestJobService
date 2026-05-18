using System.Diagnostics;
using System.Management;
using System.Net;

namespace TestJobService
{
    internal static class SystemInfo
    {
        internal static InfoModel GetInfo(Settings settings) 
        {
            InfoModel ret = new InfoModel();
            ret.HostName = Dns.GetHostName();
            ret.IP = Dns.GetHostAddresses(ret.HostName).FirstOrDefault(_ => _.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString(); // Берём только IPv4 при необходимости можно переписать для IPv6 или писать оба
            ret.DateTime = DateTime.Now;

            ret.CpuUsage().MemoryUsage().FreeDiskSpace().Processes(settings).WinVersion();

            return ret;
        }

        internal static InfoModel CpuUsage(this InfoModel model)
        {
            using var infoObj = GetInfo("SELECT LoadPercentage FROM Win32_Processor");
            if (int.TryParse(infoObj?["LoadPercentage"]?.ToString(), out int res))
                    model.CpuLoad = res;
            return model;
        }

        internal static InfoModel MemoryUsage(this InfoModel model)
        {
            using var infoObj = GetInfo("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            if (ulong.TryParse(infoObj?["TotalVisibleMemorySize"]?.ToString(), out ulong total))
                model.MemoryTotal = total / 1024;   // В мегабайтах
            if (ulong.TryParse(infoObj?["FreePhysicalMemory"]?.ToString(), out ulong free))
                model.MemoryFree = free / 1024;
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

        internal static InfoModel Processes(this InfoModel model, Settings settings)
        {
            model.Processes = Process.GetProcesses().Select(_ => _.ProcessName).Distinct().ToArray();
            if (settings.TrackedProcesses != null)
                model.TrackedProcesses = model.Processes.Intersect(settings.TrackedProcesses, StringComparer.OrdinalIgnoreCase).ToArray();
            return model;
        }

        internal static InfoModel WinVersion(this InfoModel model)
        {
            using var infoObj = GetInfo("SELECT Caption, Version, BuildNumber FROM Win32_OperatingSystem");
            model.WinVersion = infoObj?["Caption"] + " " + infoObj["Version"] + " " + infoObj["BuildNumber"];
            return model;
        }

        private static ManagementBaseObject? GetInfo(string query)
        {
            using var searcher = new ManagementObjectSearcher(query);
            using var objList = searcher.Get();
            using var enumerator = objList.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            return null;
        }
    }
}
