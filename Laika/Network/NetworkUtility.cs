using System;
using System.Net;

namespace Laika.Network
{
    public class NetworkUtility
    {
        public static int IPToInteger(string intAddress)
        {
            return BitConverter.ToInt32(IPAddress.Parse(intAddress).GetAddressBytes(), 0);
        }

        public static string IntegerToIP(int intAddress)
        {
            return new IPAddress(BitConverter.GetBytes(intAddress)).ToString();
        }
    }
}
