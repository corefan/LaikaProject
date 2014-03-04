using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Laika.Data.Ini
{
    /// <summary>
    /// ini read, write class
    /// </summary>
    public class Ini
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="file">ini 파일 위치 및 이름</param>
        public Ini(string file)
        {
            _file = file;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        /// <summary>
        /// ini 읽기
        /// </summary>
        /// <param name="section">섹션</param>
        /// <param name="key">키</param>
        /// <returns>값</returns>
        public string GetIniValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(_size);
            int i = GetPrivateProfileString(section, key, "", temp, _size, _file);
            return temp.ToString();
        }
        /// <summary>
        /// ini 쓰기
        /// </summary>
        /// <param name="section">섹션</param>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public void SetIniValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _file);
        }

        private string _file;
        private const int _size = 255;
    }
}
