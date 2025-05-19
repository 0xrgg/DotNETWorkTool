namespace DotNETworkTool.Common.Util
{
    public static class CommonConsole
    {
        // misc variables used for displaying to console

        public const string spacer = "************************************************************************************************************************";
        public const string headingspacer = "________________________________________________________________________________________________________________________";
        public const string separator = "|";
        public const string InvalidSelection = "Invalid Selection";

        public static string TableHeader = string.Empty;

        public static readonly string[] PortTableHeaderMessages = { "     Port   ", "                   Service                 " };

        // Used to display results - not pretty but looks okay in the console without resizing. 
        public static readonly string[] DeviceTableHeaderMessages = {
            "  # ",
            "       IP        ",
            "         MAC       ",
            "                 Vendor                  ",
            "             Hostname             "
        };

        public static void CreateTableHeaders()
        {
            TableHeader = $"{CommonConsole.DeviceTableHeaderMessages[0]}|{CommonConsole.DeviceTableHeaderMessages[1]}|{CommonConsole.DeviceTableHeaderMessages[2]}|{CommonConsole.DeviceTableHeaderMessages[3]}|{CommonConsole.DeviceTableHeaderMessages[4]}|";
        }

        public static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }
    }
}
