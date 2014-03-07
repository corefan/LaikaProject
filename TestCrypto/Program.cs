using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Crypto;

namespace TestCrypto
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAES256();
            TestTripleDes();
            TestMD5();
            TestSHA1();
            TestSHA256();
            TestSHA512();
        }

        private static void TestSHA512()
        {
            string hash = LaikaSHA512.Sha512Hash("test");
            bool b = LaikaSHA512.VerifySha512HashForString("test", hash);
        }

        private static void TestSHA256()
        {
            string hash = LaikaSHA256.Sha256Hash("test");
            bool b = LaikaSHA256.VerifySha256HashForString("test", hash);
        }

        private static void TestSHA1()
        {
            string hash = LaikaSHA1.Sha1Hash("test");
            bool b = LaikaSHA1.VerifySha1HashForString("test", hash);
        }

        private static void TestMD5()
        {
            string hash = LaikaMD5.GetMd5Hash("test");
            bool b = LaikaMD5.VerifyMd5HashForString("test", hash);
        }

        private static void TestTripleDes()
        {
            LaikaTripleDES td = new LaikaTripleDES("1234567890123456");
            byte[] encData = td.Encrypt(Encoding.UTF8.GetBytes("test"));
            byte[] decData = td.Decrypt(encData);
            string result = Encoding.UTF8.GetString(decData);
        }

        private static void TestAES256()
        {
            AES256 aes = new AES256("12345678901234567890123456789012");
            byte[] encData = aes.Encrypt("test", Encoding.UTF8);
            string decString = aes.Decrypt(encData, Encoding.UTF8);
        }
    }
}
