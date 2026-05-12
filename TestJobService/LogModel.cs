using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJobService
{
    public class LogModel
    {
        public string? HostName { get; set; }
        public string? IP {  get; set; }

        public DateTime DateTime { get; set; }

        public int? CpuLoad { get; set; }
        public string? MemoryUsage { get; set; }
        public string? DiskUsage { get; set; }
    }
}
