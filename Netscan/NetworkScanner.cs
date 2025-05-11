using DotNETworkTool.Common.Config;
using DotNETworkTool.Common.Netscan;
using DotNETworkTool.Common.NetworkModels;
using DotNETworkTool.Common.Util;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DotNETworkTool.Netscan
{
    public class NetworkScanner
    {
        private string findingInterfacesMsg = "Searching for network interfaces...",
                       findingNetworkHostsMsg = "Searching for hosts...",
                       firstHost = string.Empty,
                       lastHost = string.Empty;

        private SubnetsList _subnetList;
        private int interfaceCount;

        private NetworkInterface[] ifaces;
        private UnicastIPAddressInformationCollection ipAddresses;
        private IPAddress ipAddress;
        private string subnetMask;
        private string targetIp;
        private Stopwatch stopWatch;

        private List<Host> ActiveHosts = new List<Host>();
        private List<AbstractHost> hostList = new List<AbstractHost>();
        private List<Thread> threadList = new List<Thread>();

        public NetworkScanner()
        {
            _subnetList = new SubnetsList();

            ifaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.GetIPProperties().UnicastAddresses.Any(y => y.Address.GetAddressBytes() is not null))
                .ToArray();
            stopWatch = new Stopwatch();
        }

        public NetworkInterface[] FindInterfaces()
        {
            interfaceCount = ifaces.Count();

            CommonConsole.Write(findingInterfacesMsg, ConsoleColor.Yellow);
            CommonConsole.Write($"Found {interfaceCount.ToString()} interfaces...", ConsoleColor.Yellow);
            CommonConsole.Write(CommonConsole.spacer, ConsoleColor.Yellow);

            int i = 1;

            foreach (var iface in ifaces)
            {
                ipAddresses = iface.GetIPProperties().UnicastAddresses;

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork && u.Address.GetAddressBytes() is not null)
                            .Select(i => i.Address)
                            .FirstOrDefault();

                if (ipAddress == null) break;

                subnetMask = IPManipulator.ReturnSubnetmask(ipAddress);
                CommonConsole.Write($"({i}) {iface.Name}: {ipAddress} / {subnetMask} ", ConsoleColor.Green);

                var bytes = ipAddress.GetAddressBytes();
                var binarySubnetMask = string.Join(".", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                int mask = binarySubnetMask.Count(b => b == '1');

                i++;
            }

            return ifaces;
        }

        public IEnumerable<Host> StartScan(string userInput)
        {
            stopWatch.Start();

            CommonConsole.Write(findingNetworkHostsMsg, ConsoleColor.Yellow);

            subnetMask = ScanInterfaces(userInput);
            var hosts = ScanNetwork(ipAddress, subnetMask.ToString());

            return ActiveHosts.Select(x => x).Distinct().ToArray();
        }

        public string ScanInterfaces(string userInput)
        {
            var iface = ifaces[int.Parse(userInput) - 1];

            try
            {
                ipAddresses = iface.GetIPProperties().UnicastAddresses;
                var ipv4Mask = ipAddresses
                    .Where(x => x.IPv4Mask.ToString() != "0.0.0.0")
                    .Select(x => x.IPv4Mask)
                    .First();

                ipAddress = ipAddresses
                            .Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First();

                CommonConsole.Write($"Selected: {iface.Name} on {ipAddress}/{ipv4Mask} ", ConsoleColor.Green);

                return ipv4Mask.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public IEnumerable<Host> ScanNetwork(IPAddress ipAddress, string subnetMask)
        {
            if (ToolConfig.CUSTOM_IP_SCAN)
            {
                ExecuteCustomScan();
            }
            else
            {
                ExecuteFullScan();
            }

            stopWatch.Stop();

            var elapsedTime = FormatStopwatch();

            CommonConsole.Write($"{CommonConsole.spacer}", ConsoleColor.Yellow);
            CommonConsole.Write($"Found {ActiveHosts.Count()} hosts...", ConsoleColor.Green);
            CommonConsole.Write($"Scan completed in: {elapsedTime}", ConsoleColor.Green);

            return ActiveHosts;

        }

        private IPHostEntry QueryDNS(Host host)
        {
            try
            {
                return Dns.GetHostEntry(host.IP);
            }
            catch (Exception)
            {
                return new IPHostEntry();
            }

        }

        private void ExecuteFullScan()
        {
            var subnet = _subnetList.ReturnSubnetInfo(subnetMask);
            var segment = new IPSegment(ipAddress.ToString(), subnet.SubnetMask);

            firstHost = segment.Hosts().First().ToIpString();
            lastHost = segment.Hosts().Last().ToIpString();
            targetIp = firstHost;

            for (int i = 0; i < subnet.NumberOfHosts; i++)
            {
                hostList.Add(new AbstractHost { IP = targetIp });
                targetIp = IncrementIpAddress(targetIp.ToString());
            }

            ThreadedPingRequest(subnet.NumberOfHosts);


        }

        private void ExecuteCustomScan()
        {
            foreach (var ip in ToolConfig.CUSTOM_IP_ADDRESSES)
            {
                hostList.Add(new AbstractHost { IP = ip });
            }

            ThreadedPingRequest(ToolConfig.CUSTOM_IP_ADDRESSES.Count());
        }

        private void ThreadedPingRequest(int loopCount)
        {
            for (int i = 0; i < loopCount; i++)
            {
                if (targetIp != lastHost)
                {
                    threadList.Add(new Thread(() => PingRequest()));
                    threadList[i].Start();
                    Thread.Sleep(50);
                }
                else
                {
                    continue;
                }
            }

            threadList.WaitAll();
            threadList.Clear();
        }

        public void PingRequest()
        {

            if (hostList.Any(x => x.PingAttempted is false))
            {
                var targetHost = hostList.Select(x => x).Where(x => x.PingAttempted is false).First();
                targetIp = targetHost.IP.ToString();
                hostList.Remove(targetHost);
                CommonConsole.Write($"Trying host: {targetIp}", ConsoleColor.Yellow);

                var result = PingHost(IPAddress.Parse(targetIp));
            }
        }

        public string IncrementIpAddress(string ipAddress)
        {
            var ipSplit = Convert.ToString(ipAddress).Split(".");

            // Increment the last octet
            ipSplit[ipSplit.Count() - 1] = (int.Parse(ipSplit[ipSplit.Count() - 1]) + 1).ToString();

            for (int i = 3; i > 0; i--)
            {
                // check if any octet is 256 (if so - increment previous octet and reset current octet to 0, but only for the fourth, third and second octets)
                if (ipSplit[i] == "256")
                {
                    if (i > 1)
                    {
                        ipSplit[i - 1] = (int.Parse(ipSplit[i - 1]) + 1).ToString();
                        ipSplit[i] = "0";
                    }
                }
            }

            return string.Join(".", ipSplit);
        }

        public bool PingHost(IPAddress targetIp)
        {
            Host host;

            host = ArpScan.Scan(targetIp.ToString());

            if (host.MAC is not null)
            {
                CommonConsole.Write($"Found host: {host.IP}", ConsoleColor.Green);
                host.HostName = QueryDNS(host).HostName ?? "Unknown";
                ActiveHosts.Add(host);
                return true;
            }

            return false;
        }

        private string FormatStopwatch()
        {
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return elapsedTime;
        }
    }

    public class LoggingEvent
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; }
    }
}
