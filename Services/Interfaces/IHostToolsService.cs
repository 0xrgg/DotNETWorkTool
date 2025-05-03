namespace DotNETworkTool.Services.Interfaces
{
    using DotNETworkTool.Common.Config;
    using DotNETworkTool.Common.NetworkModels;

    public interface IHostToolsService
    {
        void ChooseService(IEnumerable<Host> hosts);
    }
}