namespace DotNETworkTool.Services
{
    using DotNETworkTool.Common.NetworkModels;
    using System;

    public interface ILoggingService
    {
        void LogToConsole(string message, ConsoleColor color);
        void LogToFile(string[] textArray);
        string[] DisplayHostList(IEnumerable<Host> hosts);
        void DisplayPortList(List<PortInfo> openPorts);
    }
}