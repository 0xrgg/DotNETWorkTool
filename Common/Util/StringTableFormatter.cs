namespace DotNETworkTool.Common.Util
{
    using DotNETworkTool.Common.NetworkModels;
    using System.Text;

    public class StringTableFormatter
    {
        private static int IPMaxLength = 15;

        public static string PadIP(string IP, StringBuilder sb)
        {
            sb.Clear();

            var count = IP.Length;

            sb.Append("  ");
            sb.Append(IP);
            sb.Append(' ', IPMaxLength - count);

            return sb.ToString();
        }

        public static string PadVendor(string vendor, StringBuilder sb)
        {
            sb.Clear();

            var count = 40;

            sb.Append(" ");
            sb.Append(vendor);
            sb.Append(' ', count - vendor.Length);

            return sb.ToString();
        }

        public static string PadHostname(string hostname, string tableHeaderMsg, StringBuilder sb)
        {
            sb.Clear();

            var count = tableHeaderMsg.Length;

            sb.Append(" ");
            sb.Append(hostname);
            sb.Append(' ', count - (hostname.Length + 3));

            return sb.ToString();
        }

        public static Host PadPropertiesForDisplay(Host host, string tableHeaderMsg)
        {
            var sb = new StringBuilder();

            var newHost = new Host()
            {
                IP = PadIP(host.IP, sb),
                Vendor = PadVendor(host.Vendor, sb),
                HostName = PadHostname(host.HostName, tableHeaderMsg, sb),
                MAC = host.MAC
            };

            return newHost;
        }
    }
}
