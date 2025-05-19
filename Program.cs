// See https://aka.ms/new-console-template for more information
using DotNETworkTool.Netscan;
using DotNETworkTool.Services;
using DotNETworkTool.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            //.AddSingleton<ILoggingService, LoggingService>()
            .BuildServiceProvider();

        //var loggingService = serviceProvider.GetService<ILoggingService>();

        var radar = new DotNETworkScanner();
        radar.StartApp();
    }
}