namespace DotNETworkTool.Services
{
    using DotNETworkTool.Common.Config;
    using DotNETworkTool.Common.HostTools;
    using DotNETworkTool.Common.NetworkModels;
    using DotNETworkTool.Common.Util;
    using DotNETworkTool.Services.Interfaces;
    using System.Linq.Expressions;

    public class HostToolsService : IHostToolsService
    {
        private readonly ILoggingService _loggingService;

        private PortScanner PortScanner;

        public HostToolsService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            PortScanner = new PortScanner(_loggingService);
        }

        public void ChooseService(IEnumerable<Host> hosts)
        {
            try
            {
                Start: 
                CommonConsole.WriteToConsole($"Continue to tool selection? [y/n]", ConsoleColor.Yellow);
                var selection = Console.ReadKey(true);

                if (selection.Key == ConsoleKey.Y)
                {
                    var selectedHost = HostSelect(hosts);

                    var runAgain = PortScanner.CheckHost(selectedHost.IP);                        

                    if (runAgain)
                    {
                        goto Start;
                    }

                } else
                {
                    CommonConsole.WriteToConsole("Exiting program...", ConsoleColor.Yellow);
                    Environment.Exit(0);
                }

            }
            catch (Exception e)
            {
                CommonConsole.WriteToConsole("Error loading custom config...", ConsoleColor.Red);
                throw e;
            }

        }

        private Host HostSelect(IEnumerable<Host> hosts)
        {
            CommonConsole.WriteToConsole($"Select a host [1 - {hosts.Count()}] ", ConsoleColor.Yellow);
            var selectedHost = int.Parse(Console.ReadLine()) - 1;

            CommonConsole.WriteToConsole($"Selected: {hosts.ElementAt(selectedHost).IP}", ConsoleColor.Yellow);

            return hosts.ElementAt(selectedHost);
        }
    }
}
