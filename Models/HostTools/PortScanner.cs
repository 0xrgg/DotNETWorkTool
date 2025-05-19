using DotNETworkTool.Common.NetworkModels;
using DotNETworkTool.Common.Util;
using DotNETworkTool.Common.Config;
using DotNETworkTool.Services.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace DotNETworkTool.Common.HostTools
{
    public class PortScanner : HostTool
    {
        private List<PortInfo> openPorts;
        private List<PortInfo> targetPorts;

        public readonly static string portInfoPath = ToolConfig.PORT_LIST_PATH;

        public PortScanner()
        {
            openPorts = new List<PortInfo>();
            targetPorts = new List<PortInfo>();
        }

        public IEnumerable<PortInfo> CheckHost(string ipAddress)
        {
            if (ToolConfig.CUSTOM_PORT_SCAN)
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

            return openPorts;

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
            if (ToolConfig.CUSTOM_PORT_SCAN && ToolConfig.CUSTOM_PORTS.Any())
            {
                foreach (var port in ToolConfig.CUSTOM_PORTS)
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
                CommonConsole.Write($"Trying port {portInfo.PortNum}", ConsoleColor.Yellow);

                try
                {
                    if (!tcpClient.ConnectAsync(IPAddress.Parse(ipAddress), portInfo.PortNum).Wait(1000))
                    {
                        CommonConsole.Write($"Port {portInfo.PortNum} closed", ConsoleColor.Red);
                        return;
                    }

                    openPorts.Add(portInfo);

                    CommonConsole.Write($"Port {portInfo.PortNum} open", ConsoleColor.Green);

                }
                catch (Exception)
                {
                    CommonConsole.Write($"The port maybe open but the produced socket lacks correct permissions", ConsoleColor.Yellow);
                }
            }
        }
    }
}
