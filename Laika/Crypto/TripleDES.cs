using System;
using System.Text;
using System.Security.Cryptography;

namespace Laika.Crypto
{
    /// <summary>
    /// Triple DES 암호화 클래스
    /// </summary>
    public class LaikaTripleDES
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">스트링 키</param>
        public LaikaTripleDES(string key)
            : this (Encoding.UTF8.GetBytes(key))
        { 
        
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">바이트 키</param>
        public LaikaTripleDES(byte[] key)
            : this(key, null)
        { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">16바이트나 24바이트 길이를 갖는 키</param>
        /// <param name="IV">8바이트의 초기화 벡터</param>
        public LaikaTripleDES(byte[] key, byte[] IV)
        {
            _key = key;
            
            if (IV == null)
                _IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            else
                _IV = IV;

            if (_key == null || (_key.Length != 16 && _key.Length != 24))
                throw new ArgumentException("Invalid Key length. Key length must be 16 Bytes or 24 Bytes.");

            if (_IV == null || _IV.Length != 8)
                throw new ArgumentException("Invalid IV length. IV length must be 8 Bytes.");
        }
        /// <summary>
        /// 암호화 메소드
        /// </summary>
        /// <param name="source">암호화할 대상</param>
        /// <returns>암호화된 바이트 배열</returns>
        public byte[] Encrypt(byte[] source)
        {
            byte[] result = null;
            using (TripleDESCryptoServiceProvider des = GetProvider())
            {
                using (ICryptoTransform ic = des.CreateEncryptor())
                {
                    result = ic.TransformFinalBlock(source, 0, source.Length);
                }
            }
            return result;
        }
        /// <summary>
        /// 복호화 메소드
        /// </summary>
        /// <param name="source">복호화할 대상</param>
        /// <returns>복호화된 바이트 배열</returns>
        public byte[] Decrypt(byte[] source)
        {
            byte[] result = null;
            using (TripleDESCryptoServiceProvider des = GetProvider())
            {
                using (ICryptoTransform ic = des.CreateDecryptor())
                { 
                    result = ic.TransformFinalBlock(source, 0, source.Length);
                }
            }
            return result;
        }

        private TripleDESCryptoServiceProvider GetProvider()
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = _key;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.Zeros;
            des.IV = _IV;

            return des;
        }

        private byte[] _key;
        private byte[] _IV;
    }
}
