namespace DotNETworkTool
{
    using DotNETworkTool.Common.Config;
    using DotNETworkTool.Common.NetworkModels;
    using DotNETworkTool.Common.Util;
    using DotNETworkTool.Services;
    using DotNETworkTool.Services.Interfaces;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public class DotNETworkScanner
    {
        private readonly INetworkScanner _networkScanner;
        private readonly IHostToolsService _hostToolsService;
        private readonly ILoggingService _loggingService;

        private IEnumerable<Host> Hosts;

        public const string FeatureSelection = "Select one of the following options...";

        public DotNETworkScanner(INetworkScanner networkScanner, IHostToolsService hostToolsService, ILoggingService loggingService)
        {
            _networkScanner = networkScanner;
            _hostToolsService = hostToolsService;
            _loggingService = loggingService;
        }

        public void StartApp()
        {
            Config.BuildConfig();
            var loggingText = RunPhase1();
            LoggingPrompt(loggingText);
            RunPhase2();

        }

        public string[] RunPhase1()
        {
            Hosts = Enumerable.Empty<Host>();

        Input:
            var ifaces = _networkScanner.FindInterfaces();

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
            CommonConsole.WriteToConsole("Select an interface to scan on...", ConsoleColor.Yellow);

            var input = Console.ReadLine();

            // not ideal but prevents any other selections
            if (!ValidateInput(input, ifaces.Length) && !IsAPIPA(int.Parse(input), ifaces))
            {
                InvalidSelection();
                goto Input;
            }
            else
            {
                Hosts = _networkScanner.StartScan(input);
            }

            var formattedText = _loggingService.DisplayHostList(Hosts);

            return formattedText;

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

        private void LoggingPrompt(string[] textArray)
        {
        LoggingPrompt:
            CommonConsole.WriteToConsole("Write to logfile? [Y/N]", ConsoleColor.Yellow);
            var logging = Console.ReadKey(true);

            if (logging.Key == ConsoleKey.Y)
            {
                CommonConsole.WriteToConsole("Writing to logfile...", ConsoleColor.Yellow);
                _loggingService.LogToFile(textArray);
            }
            else if (logging.Key == ConsoleKey.N)
            {
                CommonConsole.WriteToConsole("Skipping log file", ConsoleColor.Yellow);
            }
            else
            {
                CommonConsole.WriteToConsole(CommonConsole.InvalidSelection, ConsoleColor.Red);
                goto LoggingPrompt;
            }

            CommonConsole.WriteToConsole(CommonConsole.spacer, ConsoleColor.Yellow);
        }

        public void InvalidSelection()
        {
            CommonConsole.WriteToConsole("Invalid selection", ConsoleColor.Red);
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

        private void RunPhase2()
        {
            _hostToolsService.ChooseService(Hosts);
        }


    }
}

