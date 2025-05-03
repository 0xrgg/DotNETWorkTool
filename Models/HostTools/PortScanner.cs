namespace DotNETworkTool.Common.HostTools
{
    using DotNETworkTool.Common.Config;
    using DotNETworkTool.Common.NetworkModels;
    using DotNETworkTool.Common.Util;
    using DotNETworkTool.Services;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;

    public class PortScanner : HostTool
    {
        private readonly ILoggingService _loggingService;

        private List<PortInfo> openPorts;
        private List<PortInfo> targetPorts;

        public readonly static string portInfoPath = Config.PORT_LIST_PATH;

        public PortScanner(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            openPorts = new List<PortInfo>();
            targetPorts = new List<PortInfo>();

            this.Name = "PortScanner";
        }

        public bool CheckHost(string ipAddress)
        {
            if (Config.CUSTOM_PORT_SCAN)
            {
                CreatePortList();
            }
            else
            {
                LoadPortList();
            }

            var threadList = new List<Thread>();

            var length = targetPorts.Any(x => !x.Attempted);

            for (int i = 0; i < targetPorts.Count(); i++)
            {

                threadList.Add(new Thread(() => ThreadedPortRequest(ipAddress, targetPorts[i])));
                threadList[i].Start();
                Thread.Sleep(50);
            }

            threadList.WaitAll();
            threadList.Clear();

            CommonConsole.WriteToConsole($"Found {openPorts.Count} open ports [{ipAddress}]", ConsoleColor.Green);

            if (openPorts.Any())
            {
                _loggingService.DisplayPortList(openPorts);
            }

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
            CommonConsole.WriteToConsole($"Return to tool selection? [y/n]", ConsoleColor.Yellow);

            var selection = Console.ReadKey(true);

            if (selection.Key == ConsoleKey.Y)
            {
                return true;
            } else
            {
                CommonConsole.WriteToConsole($"Exiting program...", ConsoleColor.Yellow);
                return false;
            }

        }

        private void LoadPortList()
        {
            var ports = CommonOperations.LoadListFromFile(portInfoPath);

            foreach (var portInfo in ports)
            {
                targetPorts.Add(new PortInfo
                {
                    PortNum = int.Parse(portInfo.Split(CommonConsole.separator[0])[0]),
                    PortName = portInfo.Split(CommonConsole.separator[0])[1]
                });
            }
        }

        private void CreatePortList()
        {
            if (Config.CUSTOM_PORT_SCAN && Config.CUSTOM_PORTS.Any())
            {
                foreach(var port in Config.CUSTOM_PORTS)
                {
                    targetPorts.Add(new PortInfo { PortNum = port });
                }
            }
        }

        private void ThreadedPortRequest(string ipAddress, PortInfo portInfo)
        {
            portInfo.Attempted = true;

            if (targetPorts.Any(x => !x.Attempted))
            {
                using TcpClient tcpClient = new TcpClient();
                CommonConsole.WriteToConsole($"Trying port {portInfo.PortNum}", ConsoleColor.Yellow);

                try
                {
                    if (!tcpClient.ConnectAsync(IPAddress.Parse(ipAddress), portInfo.PortNum).Wait(1000)) {
                        CommonConsole.WriteToConsole($"Port {portInfo.PortNum} closed", ConsoleColor.Red);
                        return;
                    }

                    openPorts.Add(portInfo);

                    CommonConsole.WriteToConsole($"Port {portInfo.PortNum} open", ConsoleColor.Green);

                }
                catch (Exception e)
                {
                    Console.ResetColor();
                    //CommonConsole.WriteToConsole($"Port {portInfo.PortNum} closed", ConsoleColor.Red);
                    CommonConsole.WriteToConsole($"The port maybe open but the produced socket lacks correct permissions", ConsoleColor.Yellow);

                }
            }

        }
    }
}
