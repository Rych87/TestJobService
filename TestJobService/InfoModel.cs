using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJobService
{
    public class InfoModel
    {
        public string? HostName { get; set; }
        public string? IP {  get; set; }

        public string? WinVersion { get; set; }

        public DateTime DateTime { get; set; }

        public int CpuLoad { get; set; }
        public int MemoryUsage { get; set; }

        /// <summary>
        /// В мегабайтах
        /// </summary>
        public ulong FreeDiskSpace { get; set; }

        public string[]? Processes { get; set; }
    }
}
