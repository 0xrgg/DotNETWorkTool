namespace DotNETworkTool.Common.NetworkModels
{
    public class HostTracker
    {
        public IEnumerable<AbstractHost> hosts;
    }

    public class AbstractHost
    {
        public string IP { get; set; }
        public bool PingAttempted { get; set; }
        public bool Alive { get; set; }

        public AbstractHost()
        {
            PingAttempted = false;
            Alive = false;
        }
    }


}
