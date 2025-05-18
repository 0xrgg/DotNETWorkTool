namespace DotNETworkTool.Common.NetworkModels
{
    public class AbstractHost
    {
        public string IP { get; set; }
        public bool PingAttempted { get; set; }
        public bool Alive { get; set; }
    }


}
