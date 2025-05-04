using DotNETworkTool.Common.NetworkModels;

namespace DotNETworkTool.Services.Interfaces
{
    public interface ILoggingService
    {
        void LogToConsole(string message, ConsoleColor color);
        void LogToFile(string[] textArray);
        string[] DisplayHostList(IEnumerable<Host> hosts);
        void DisplayPortList(List<PortInfo> openPorts);
    }
}