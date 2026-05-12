using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestJobService
{
    internal static class SystemInfo
    {
        internal static LogModel GetInfo(CancellationToken stoppingToken) 
        {
            LogModel ret = new LogModel();
            ret.HostName = Dns.GetHostName();
            ret.IP = Dns.GetHostAddresses(ret.HostName).FirstOrDefault(x=>x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString(); // Берём только IPv4 при необходимости можно переписать для IPv6 или писать оба
            ret.DateTime = DateTime.Now;

            ret.GetSysInfo();

            return ret;
        }

        public static void GetSysInfo(this LogModel model)
        {
            var searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor");

            if(int.TryParse(searcher.Get().OfType<ManagementObject>()?.FirstOrDefault()?["LoadPercentager"].ToString(), out int res))
                model.CpuLoad = res;
        }
    }
}
