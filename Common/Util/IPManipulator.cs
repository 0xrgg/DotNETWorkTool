using System.Net;

namespace DotNETworkTool.Common.Util
{
    public static class IPManipulator
    {

        public static string ReturnSubnetmask(IPAddress ipaddress)
        {

            uint firstOctet = ReturnFirtsOctet(ipaddress);
            if (firstOctet >= 0 && firstOctet <= 127)
                return "255.0.0.0";
            else if (firstOctet >= 128 && firstOctet <= 191)
                return "255.255.0.0";
            else if (firstOctet >= 192 && firstOctet <= 223)
                return "255.255.255.0";
            else return "0.0.0.0";
        }

        public static uint ReturnFirtsOctet(IPAddress ipAddress)
        {
            byte[] byteIP = ipAddress.GetAddressBytes();
            uint ipInUint = byteIP[0];
            return ipInUint;
        }
    }
}
