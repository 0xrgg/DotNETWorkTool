// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using DotNETworkTool;
using DotNETworkTool.Services;
using DotNETworkTool.Services.Interfaces;
using System.Reflection;
using MediatR;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddSingleton<IIPManipulationService, IPManipulationService>()
            .AddSingleton<INetworkScanner, NetworkScanner>()
            .AddSingleton<IHostToolsService, HostToolsService>()
            .AddSingleton<ILoggingService, LoggingService>()
            .BuildServiceProvider();

        var ipManipulationService = serviceProvider.GetService<IIPManipulationService>();
        var networkService = serviceProvider.GetService<INetworkScanner>();
        var toolsService = serviceProvider.GetService<IHostToolsService>();
        var loggingService = serviceProvider.GetService<ILoggingService>();

        var radar = new DotNETworkScanner(networkService, toolsService, loggingService);
        radar.StartApp();
    }
}