using System.Text.RegularExpressions;

namespace Laika.Database
{
    public class MySqlUtility
    {
        public static bool IsSafeString(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return false;

            return _safeStringChecker.IsMatch(parameter);
        }
        private static Regex _safeStringChecker = new Regex(@"^[\w\s\.\?!@#%&=~:/,_-]*$", RegexOptions.Compiled);

        public static bool isSafeStringLoose( string parameter )
        {
            if(string.IsNullOrEmpty(parameter))
                return false;

            return _safeStringLooseChecker.IsMatch( parameter );
        }
        private static Regex _safeStringLooseChecker = new Regex( @"^[\w\s\.\$\^\{\[\(\|\)\*\+\?!@#%&=~_,<>:/-]*$", RegexOptions.Compiled );


        /// <summary>
        /// 쿼리에 들어가는 인자가 SQL Injection 공격에 안전한 한 단어 문자인지 체크합니다.
        /// </summary>
        /// <param name="queryParameter">쿼리에 들어가는 문자열 인자</param>
        /// <returns>해당 인자가 안전한 한 단어 문자인지의 여부</returns>
        public static bool isSafeOneWordString( string parameter )
        {
            if (string.IsNullOrEmpty(parameter))
                return false;

            return _safeOneWordStringChecker.IsMatch( parameter );
        }
        private static Regex _safeOneWordStringChecker = new Regex( @"^\w*$", RegexOptions.Compiled );
    }
}
