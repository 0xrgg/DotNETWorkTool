using System.Text;

namespace DotNETworkTool.Common.Util
{
    public static class CommonOperations
    {
        public static byte[] ConvertStringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static void WriteToFile(string fileName, string value)
        {
            using FileStream fs = File.OpenWrite(fileName);
            byte[] buffer;

            buffer = CommonOperations.ConvertStringToBytes(value);
            fs.Write(buffer, 0, buffer.Length);
        }

        public static List<string> LoadListFromFile(string filename)
        {
            List<string> list = new List<string>();

            try
            {
                foreach (var line in File.ReadAllLines(filename))
                    list.Add(line.Trim());
            }
            catch (Exception)
            {
                CommonConsole.Write("Error reading file.", ConsoleColor.Red);

                return new List<string>();
            }
            return list;
        }
    }
}
