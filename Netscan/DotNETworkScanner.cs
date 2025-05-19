using DotNETworkTool.Common.Config;
using DotNETworkTool.Common.HostTools;
using DotNETworkTool.Common.NetworkModels;
using DotNETworkTool.Common.Util;
using DotNETworkTool.Services;
using DotNETworkTool.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DotNETworkTool.Netscan
{
    public class DotNETworkScanner
    {
        private readonly NetworkScanner NetworkScanner;
        private readonly PortScanner PortScanner;

        private IEnumerable<Host> Hosts;

        public const string FeatureSelection = "Select one of the following options...";

        public DotNETworkScanner()
        {
            NetworkScanner = new NetworkScanner();
            PortScanner = new PortScanner();
        }

        public void StartApp()
        {
            CommonConsole.CreateTableHeaders();

            var result = new ScanResult();

            ToolConfig.BuildConfig();
            var hostList = RunHostScan();

            DisplayHostsAndLogPrompt();

            CommonConsole.Write("Perform full scan or simple scan? [F/S]", ConsoleColor.Yellow);
            var response = Console.ReadKey(true);

            if (response.Key == ConsoleKey.F)
            {
                foreach(var host in hostList)
                {
                    var openPorts = PortScanner.CheckHost(host.IP);

                    CommonConsole.Write($"Found {openPorts.Count()} open ports for host {host.IP}", ConsoleColor.Green);

                    result.HostScanResult.Add(new HostScanResult() { Host = host, PortInfo = openPorts });
                }

                CommonConsole.Write("Port scan complete... \r\n", ConsoleColor.Yellow);

                foreach(var host in result.HostScanResult)
                {
                    if (host.PortInfo.Any())
                    {
                        CommonConsole.Write($"Ports open @ {host.Host.IP}", ConsoleColor.Yellow);
                        LoggingService.DisplayPortList(host.PortInfo.ToList());
                        CommonConsole.Write("\r\n", ConsoleColor.Yellow);
                    }
                }

            } else
            {
                var portList = RunPortScan();
            }
        }

        public IEnumerable<Host> RunHostScan()
        {
            Hosts = Enumerable.Empty<Host>();

        Input:
            var ifaces = NetworkScanner.FindInterfaces();

            CommonConsole.Write(CommonConsole.spacer, ConsoleColor.Yellow);
            CommonConsole.Write("Select an interface to scan on...", ConsoleColor.Yellow);

            var input = Console.ReadLine();

            // not ideal but prevents any other selections
            if (!ValidateInput(input, ifaces.Length) && !IsAPIPA(int.Parse(input), ifaces))
            {
                InvalidSelection();
                goto Input;
            }
            else
            {
                Hosts = NetworkScanner.StartScan(input);
                return Hosts;
            }
        }

        private bool IsAPIPA(int input, NetworkInterface[] interfaces)
        {
            if (interfaces[input - 1].GetIPProperties().UnicastAddresses.Select(x => x)
                            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(i => i.Address)
                            .First().MapToIPv4().ToString().StartsWith("169.254."))
            {
                return true;
            }

            return false;
        }

        private void DisplayHostsAndLogPrompt()
        {
            var textArray = LoggingService.DisplayHostList(Hosts);

        LoggingPrompt:
            CommonConsole.Write("Write to logfile? [Y/N]", ConsoleColor.Yellow);
            var logging = Console.ReadKey(true);

            if (logging.Key == ConsoleKey.Y)
            {
                CommonConsole.Write("Writing to logfile...", ConsoleColor.Yellow);
                LoggingService.LogToFile(textArray);
                Thread.Sleep(2000);
            }
            else if (logging.Key == ConsoleKey.N)
            {
                CommonConsole.Write("Skipping log file", ConsoleColor.Yellow);
                Thread.Sleep(2000);
            }
            else
            {
                CommonConsole.Write(CommonConsole.InvalidSelection, ConsoleColor.Red);
                goto LoggingPrompt;
            }

            Console.Clear();
            CommonConsole.Write(CommonConsole.spacer, ConsoleColor.Yellow);
        }

        public void InvalidSelection()
        {
            CommonConsole.Write("Invalid selection", ConsoleColor.Red);
        }

        public bool ValidateInput(string input, int interfaceCount)
        {
            if (int.TryParse(input, out var parsedInput))
            {
                if (parsedInput - 1 < interfaceCount && parsedInput - 1 > -1)
                    return true;

                return false;
            }
            else
            {
                return false;
            }
        }

        private IEnumerable<PortInfo> RunPortScan()
        {
            var portList = Enumerable.Empty<PortInfo>();

            try
            {
            Start:
                Console.Clear();
                var formattedText = LoggingService.DisplayHostList(Hosts);

                CommonConsole.Write($"Continue to port scan? [y/n]", ConsoleColor.Yellow);
                var selection = Console.ReadKey(true);

                if (selection.Key == ConsoleKey.Y)
                {
                    var selectedHost = HostSelect(Hosts);

                    portList = PortScanner.CheckHost(selectedHost.IP);

                    CommonConsole.Write(CommonConsole.spacer, ConsoleColor.Yellow);
                    CommonConsole.Write($"Return to tool selection? [y/n]", ConsoleColor.Yellow);

                    selection = Console.ReadKey(true);

                    if (selection.Key == ConsoleKey.Y)
                    {
                        goto Start;
                    }
                    else
                    {
                        CommonConsole.Write($"Exiting program...", ConsoleColor.Yellow);
                        Environment.Exit(0);
                    }
                }
                else if (selection.Key == ConsoleKey.N)
                {
                    CommonConsole.Write("Exiting program...", ConsoleColor.Yellow);
                    Environment.Exit(0);
                } else
                {
                    CommonConsole.Write("Invalid selection...", ConsoleColor.Red);
                    goto Start;
                }

            }
            catch (Exception e)
            {
                CommonConsole.Write("Error loading custom config...", ConsoleColor.Red);
                throw e;
            }

            return portList;

        }

        private Host HostSelect(IEnumerable<Host> hosts)
        {
            Select:
            CommonConsole.Write($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            if (selectedHost > hosts.Count() ||  selectedHost < 0)
            {
                CommonConsole.Write("Invalid selection, try again..", ConsoleColor.Red);
                goto Select;
            }

            CommonConsole.Write($"Selected: {hosts.ElementAt(selectedHost).IP}", ConsoleColor.Yellow);

            return hosts.ElementAt(selectedHost);
        }
    }

    public class HostScanResult
    {
        public Host Host { get; set; }
        public IEnumerable<PortInfo> PortInfo { get; set; }
    }

    public class ScanResult
    {
        public List<HostScanResult> HostScanResult { get; set; } = new();
    }
}