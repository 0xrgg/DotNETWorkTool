namespace DotNETworkTool.Services
{
    using DotNETworkTool.Common.NetworkModels;
    using DotNETworkTool.Common.Util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class LoggingService : ILoggingService
    {
        public readonly static string configPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs";
        public readonly static string logPath = $"{configPath}\\NetworkScanLog_{DateTime.UtcNow.ToString("dMMyyyyHHmmss")}.txt";

        public LoggingService()
        {
            CommonConsole.TableHeader = $"{CommonConsole.DeviceTableHeaderMessages[0]}|{CommonConsole.DeviceTableHeaderMessages[1]}|{CommonConsole.DeviceTableHeaderMessages[2]}|{CommonConsole.DeviceTableHeaderMessages[3]}|{CommonConsole.DeviceTableHeaderMessages[4]}|";
        }

        public void LogToConsole(string message, ConsoleColor color)
        {
            CommonConsole.WriteToConsole(message, color);
        }

        public void LogToFile(string[] textArray)
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            using FileStream fs = File.OpenWrite(logPath);
            byte[] buffer;

            foreach (var str in CommonConsole.DeviceTableHeaderMessages)
            {
                buffer = CommonOperations.ConvertStringToBytes(str + "|");
                fs.Write(buffer, 0, buffer.Length);
            }

            buffer = CommonOperations.ConvertStringToBytes("\r\n");
            fs.Write(buffer, 0, buffer.Length);

            foreach (var str in textArray)
            {
                buffer = CommonOperations.ConvertStringToBytes(str + "\r\n");
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public string[] DisplayHostList(IEnumerable<Host> hosts)
        {
            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            CommonConsole.WriteToConsole(CommonConsole.TableHeader, ConsoleColor.Red);

            var formattedTextArray = new string[hosts.Count()];

            for (int i = 0; i < hosts.Count(); i++)
            {
                var host = hosts.Select(x => new Host() { HostName = x.HostName, MAC = x.MAC, IP = x.IP, Vendor = x.Vendor }).ElementAt(i);
                var paddedHost = StringTableFormatter.PadPropertiesForDisplay(host, CommonConsole.DeviceTableHeaderMessages[4]);

                var formattedText = String.Format(
                        "{0,3} |{1}| {2,5} |{3}| {4} |",
                        i + 1,
                        paddedHost.IP,
                        paddedHost.MAC,
                        paddedHost.Vendor,
                        paddedHost.HostName);

                CommonConsole.WriteToConsole(formattedText, ConsoleColor.Red);

                formattedTextArray[i] = formattedText;

            }

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            return formattedTextArray;
        }

        public void DisplayPortList(List<PortInfo> openPorts)
        {
            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);

            var formattedText = String.Format("|{0} | {1} |", CommonConsole.PortTableHeaderMessages[0], CommonConsole.PortTableHeaderMessages[1]);
            CommonConsole.WriteToConsole(formattedText, ConsoleColor.Yellow);

            foreach (var port in openPorts)
            {
                var textString = String.Format("| {0,-11} | {1,-43} |", port.PortNum, port.PortName);
                CommonConsole.WriteToConsole(textString, ConsoleColor.Yellow);
            }

        }
    }
}
