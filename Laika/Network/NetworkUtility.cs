using System;
using System.Text;

namespace Laika.Network
{
    public class NetworkUtility
    {
        public static uint IPToDecimal(string address)
        {
            string[] val = address.Split('.');
            if (val.Length != 4)
                throw new Exception("Invalid IPv4 length.");

            byte[] bytesData = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytesData[3 - i] = byte.Parse(val[i]);
            }

            return BitConverter.ToUInt32(bytesData, 0);
        }

        public static string DecimalToIP(uint decimalAddress)
        {
            string[] val = new string[4];

            byte[] bytesData = BitConverter.GetBytes(decimalAddress);

            for (int i = 0; i < 4; i++)
            {
                val[3 - i] = bytesData[i].ToString();
            }

            return string.Join(".", val);
        }
    }
}
