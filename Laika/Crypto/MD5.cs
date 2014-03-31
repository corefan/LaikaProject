using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Laika.ExtendBundle;

namespace Laika.Crypto
{
    public static class LaikaMD5
    {
        /// <summary>
        /// MD5 hash를 구하는 메소드
        /// </summary>
        /// <param name="source">원본 스트링. UTF8</param>
        /// <param name="upper">대문자 설정 여부</param>
        /// <returns>Hash 값</returns>
        public static string GetMd5Hash(string source, bool upper = false)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            return GetStringFromMd5hashBytes(GetMd5Hash(Encoding.UTF8.GetBytes(source)), upper);
        }

        /// <summary>
        /// MD5 hash를 구하는 메소드
        /// </summary>
        /// <param name="source">원본 바이트 배열</param>
        /// <returns>Hash 값</returns>
        public static byte[] GetMd5Hash(byte[] source)
        {
            if (source.IsNullOrZeroLengh())
                return null;

            byte[] hash = null;
            using (MD5 md5hash = MD5.Create())
            {
                hash = md5hash.ComputeHash(source);
            }
            return hash;
        }

        /// <summary>
        /// MD5 Byte Hash 값을 string 으로 변환
        /// </summary>
        /// <param name="hash">Byte Hash 값</param>
        /// <param name="upper">대문자 설정 여부</param>
        /// <returns>Hash String</returns>
        public static string GetStringFromMd5hashBytes(byte[] hash, bool upper = false)
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
        /// MD5 원본 및 Hash 확인
        /// </summary>
        /// <param name="source">원본 데이터</param>
        /// <param name="hash">Hash 값</param>
        /// <returns>일치 여부</returns>
        public static bool VerifyMd5Hash(byte[] source, byte[] hash)
        {
            if (source.IsNullOrZeroLengh() || hash.IsNullOrZeroLengh())
                return false;

            byte[] result = GetMd5Hash(source);

            return result.SequenceEqual(hash);
        }

        /// <summary>
        /// MD5 원본 및 Hash 확인
        /// </summary>
        /// <param name="source">원본 데이터. UTF8</param>
        /// <param name="hash">Hash 값</param>
        /// <returns>일치 여부</returns>
        public static bool VerifyMd5HashForString(string source, string hash)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(hash))
                return false;

            string hashOfInput = GetMd5Hash(source);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Compare(hashOfInput, hash) == 0)
                return true;

            return false;
        }
    }
}
