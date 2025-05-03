namespace DotNETworkTool.Common.NetworkModels
{
    public class Subnet
    {
        public int NetworkBits { get; set; }
        public string SubnetMask { get; set; }
        public int NumberOfHosts { get; set; }

    }

    public class SubnetsList
    {
        private string[] SubnetMaskList = new string[] {
            "128.0.0.0",            //2,147,483,646
            "192.0.0.0",            //1,073,741,822
            "224.0.0.0",            //536,870,910
            "240.0.0.0",            //268,435,454
            "248.0.0.0",            //134,217,726
            "252.0.0.0",            //67,108,862
            "254.0.0.0",            //33,554,430
            "255.0.0.0",            //16777214
            "255.128.0.0",          //8388606
            "255.192.0.0",          //4194302
            "255.224.0.0",          //2097150
            "255.240.0.0",          //1048574
            "255.248.0.0",          //524286
            "255.252.0.0",          //262142
            "255.254.0.0",          //131070
            "255.255.0.0",          //65534
            "255.255.128.0",        //32766
            "255.255.192.0",        //16382
            "255.255.224.0",        //8190
            "255.255.240.0",        //4094
            "255.255.248.0",        //2046
            "255.255.252.0",        //1022
            "255.255.254.0",        //510
            "255.255.255.0",        //254
            "255.255.255.128",      //126
            "255.255.255.192",      //62
            "255.255.255.224",      //30
            "255.255.255.240",      //14
            "255.255.255.248",      //6
            "255.255.255.252",      //2                 
            "255.255.255.254",
            "255.255.255.255"
        };

        private int[] HostNumbers = new int[]
        {
            2147483646,
            1073741822,
            536870910,
            268435454,
            134217726,
            67108862,
            33554430,
            16777214,
            8388606,
            4194302,
            2097150,
            1048574,
            524286,
            262142,
            131070,
            65534,
            32766,
            16382,
            8190,
            4094,
            2046,
            1022,
            510,
            254,
            126,
            62,
            30,
            14,
            6,
            2,
            1,
            0,
        };

        public List<Subnet> Subnets = new List<Subnet>();

        public SubnetsList()
        {
            for (int i = 0; i < 32; i++)
            {
                Subnets.Add(new Subnet()
                {
                    NetworkBits = i + 1,
                    SubnetMask = SubnetMaskList[i],
                    NumberOfHosts = HostNumbers[i]
                });
            }

        }

        public Subnet ReturnSubnetInfo(string subnetMask)
        {
            return Subnets.Where(x => x.SubnetMask == subnetMask).First();
        }
    }
}
