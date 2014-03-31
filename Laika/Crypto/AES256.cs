using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Laika.ExtendBundle;

namespace Laika.Crypto
{
    public class AES256
    {
        /// <summary>
        /// AES256 Constructor.
        /// </summary>
        /// <param name="key">32Bytes의 암호화 키</param>
        public AES256(byte[] key, byte[] IV = null)
        {
            if (key.Length != 32)
                throw new ArgumentException("Invalid key length. must be 256 bits.");

            if (IV == null)
                _IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            else
                _IV = IV;

            if (_IV.IsNullOrZeroLengh())
                throw new ArgumentNullException();

            if (_IV.Length != 16)
                throw new ArgumentException("Invalid IV length. must be 16 Bytes");

            _key = key;
        }

        /// <summary>
        /// AES256 생성자
        /// </summary>
        /// <param name="key">32Bytes의 암호화 key. UTF8으로 인코딩된 스트링</param>
        public AES256(string key)
            : this(Encoding.UTF8.GetBytes(key))
        { }

        /// <summary>
        /// 암호화 메소드
        /// </summary>
        /// <param name="source">암호화할 대상</param>
        /// <returns>암호화된 byte array</returns>
        public byte[] Encrypt(byte[] source)
        {
            RijndaelManaged aes = GetAesInstance();
            ICryptoTransform encrypt = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] xBuff = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                }
                xBuff = ms.ToArray();
            }

            encrypt.Dispose();
            aes.Dispose();

            return xBuff;
        }

        /// <summary>
        /// 복호화 메소드
        /// </summary>
        /// <param name="source">복호화 대상</param>
        /// <returns>복호화된 byte array</returns>
        public byte[] Decrypt(byte[] source)
        {
            RijndaelManaged aes = GetAesInstance();
            ICryptoTransform decrypt = aes.CreateDecryptor();

            byte[] xBuff = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                }
                xBuff = ms.ToArray();
            }
            decrypt.Dispose();
            aes.Dispose();

            return xBuff;
        }

        /// <summary>
        /// 암호화 메소드
        /// </summary>
        /// <param name="source">암호화할 스트링</param>
        /// <param name="type">스트링의 encoding 방식</param>
        /// <returns>암호화된 byte array</returns>
        public byte[] Encrypt(string source, Encoding type)
        {
            byte[] targetBytes = type.GetBytes(source);
            return Encrypt(targetBytes);
        }

        /// <summary>
        /// 복호화 메소드
        /// </summary>
        /// <param name="source">복호화할 byte array</param>
        /// <param name="type">복호화 되는 스트링의 encoding 방식</param>
        /// <returns>복호화된 스트링</returns>
        public string Decrypt(byte[] source, Encoding type)
        {
            byte[] byteData = Decrypt(source);
            return type.GetString(byteData);
        }

        private RijndaelManaged GetAesInstance()
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.Zeros;
            aes.Key = _key;
            aes.IV = _IV;
            return aes;
        }

        private byte[] _key;
        private byte[] _IV;
        private const int _keyBitLength = 256;
        private const int _keyByteLenth = 32;
    }
}
