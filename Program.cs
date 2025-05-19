// See https://aka.ms/new-console-template for more information
using DotNETworkTool.Netscan;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();

        var radar = new DotNETworkScanner();
        radar.StartApp();
    }
}