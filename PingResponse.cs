using System.Net.NetworkInformation;

namespace LiveHostSweeper
{
    internal class PingResponse
    {
        public string TargetIp { get; set; }
        public PingReply TargetIpPingReply { get; set; }
    }
}