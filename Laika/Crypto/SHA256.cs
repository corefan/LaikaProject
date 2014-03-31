using System;
using System.Linq;
using System.Text;
using Laika.ExtendBundle;
using System.Security.Cryptography;

namespace Laika.Crypto
{
    public static class LaikaSHA256
    {
        /// <summary>
        /// SHA256 Hash를 구하는 메소드
        /// </summary>
        /// <param name="source">원본 바이트 데이터</param>
        /// <returns>Hash 바이트 데이터</returns>
        public static byte[] Sha256Hash(byte[] source)
        {
            if (source.IsNullOrZeroLengh())
                return null;

            using (SHA256 hash = SHA256Managed.Create())
            {
                return hash.ComputeHash(source);
            }
        }
        /// <summary>
        /// SHA256 hash를 구하는 메소드
        /// </summary>
        /// <param name="source">원본 UTF8 스트링 데이터</param>
        /// <param name="upper">대문자 여부</param>
        /// <returns>Hash 스트링 데이터</returns>
        public static string Sha256Hash(string source, bool upper = false)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            return GetStringFromSha256hashBytes(Sha256Hash(Encoding.UTF8.GetBytes(source)), upper);
        }
        /// <summary>
        /// SHA256 hash를 구하는 메소드
        /// </summary>
        /// <param name="hash">원본 바이트 데이터</param>
        /// <param name="upper">대문자 여부</param>
        /// <returns>Hash 스트링 데이터</returns>
        public static string GetStringFromSha256hashBytes(byte[] hash, bool upper = false)
        {
            if (hash.IsNullOrZeroLengh())
                return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            if (upper == false)
                return sb.ToString();

            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// Hash 검증
        /// </summary>
        /// <param name="source">원본 바이트 데이터</param>
        /// <param name="hash">Hash 바이드 데이터</param>
        /// <returns>일치 여부</returns>
        public static bool VerifySha256Hash(byte[] source, byte[] hash)
        {
            if (source.IsNullOrZeroLengh() || hash.IsNullOrZeroLengh())
                return false;

            byte[] result = Sha256Hash(source);

            return result.SequenceEqual(hash);
        }
        /// <summary>
        /// Hash 검증
        /// </summary>
        /// <param name="source">원본 스트링 데이터</param>
        /// <param name="hash">원본 Hash 스트링 데이터</param>
        /// <returns>일치여부</returns>
        public static bool VerifySha256HashForString(string source, string hash)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(hash))
                return false;

            string hashOfInput = Sha256Hash(source);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Compare(hashOfInput, hash) == 0)
                return true;

            return false;
        }
    }
}
