namespace DotNETworkTool.Services.Interfaces
{
    using DotNETworkTool.Common.NetworkModels;
    using System.Net;
    using System.Net.NetworkInformation;

    public interface INetworkScanner
    {
        NetworkInterface[] FindInterfaces();
        bool PingHost(IPAddress targetIp);
        IEnumerable<Host> StartScan(string iface);
        IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask);
    }
}