using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LiveHostSweeper
{
    internal class PingResponse
    {
        public string TargetIp { get; set; }
        public PingReply TargetIpPingReply { get; set; }
    }
}