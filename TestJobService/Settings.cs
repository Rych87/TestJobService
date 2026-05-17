
namespace TestJobService
{
    public class Settings
    {
        public string ApiUri { get; set; }

        public int Interval { get; set; }

        public int Timeout { get; set; }

        public string[] TrackedProcesses { get; set; }
    }
}
