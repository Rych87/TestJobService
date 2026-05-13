using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace TestJobService
{
    public class Settings
    {
        public string ApiUri { get; set; } = "https://myservice.com/api/sysinfo";
    }
}
