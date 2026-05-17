
namespace TestJobService
{
    public class InfoModel
    {
        public string? HostName { get; set; }
        public string? IP {  get; set; }

        public string? WinVersion { get; set; }

        public DateTime DateTime { get; set; }

        public int? CpuLoad { get; set; }

        public ulong? MemoryTotal { get; set; }
        public ulong? MemoryFree { get; set; }

        /// <summary>
        /// В мегабайтах
        /// </summary>
        public ulong? FreeDiskSpace { get; set; }

        public string[]? Processes { get; set; }

        public string[]? TrackedProcesses { get; set; }
    }
}
