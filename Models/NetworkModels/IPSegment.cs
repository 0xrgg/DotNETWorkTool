namespace DotNETworkTool.Common.NetworkModels
{
    using System.Globalization;

    public class IPSegment
    {

        private uint _ip;
        private uint _mask;

        public IPSegment(string ip, string mask)
        {
            _ip = ip.ParseIp();
            _mask = mask.ParseIp();
        }

        public uint NumberOfHosts
        {
            get { return ~_mask + 1; }
        }

        public uint NetworkAddress
        {
            get { return _ip & _mask; }
        }

        public uint BroadcastAddress
        {
            get { return NetworkAddress + ~_mask; }
        }

        public IEnumerable<uint> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

    }

    public static class IpHelpers
    {
        public static string ToIpString(this uint value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> (3 - i) * 8;
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }
            return string.Join(".", parts);
        }

        public static uint ParseIp(this string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            uint ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + uint.Parse(splitted[i]);
            }
            return ip;
        }
    }
}