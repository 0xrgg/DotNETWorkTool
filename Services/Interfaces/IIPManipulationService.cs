namespace DotNETworkTool.Services.Interfaces
{
    using System.Net;

    public interface IIPManipulationService
    {
        uint ReturnFirtsOctet(IPAddress ipAddress);
        string ReturnSubnetmask(IPAddress ipaddress);
    }
}