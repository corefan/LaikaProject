using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Laika.Data.Ini
{
    /// <summary>
    /// cross platform ini
    /// read only class
    /// </summary>
    public class CFIni
    {
        public CFIni(string file)
        {
            _file = file;
            Loading();
        }

        private void Loading()
        {
            _reader = new StreamReader(_file);
            string str = _reader.ReadLine();
            Parsing(str);
        }

        private void Parsing(string line)
        {
            if (line == null)
            {
                // file end.
                _reader.Dispose();
            }
            else if (line == "")
            {
                Parsing(_reader.ReadLine());
            }
            else if (line[0] == '[')
            {
                string section = GetSection(line);
                _currentSection = section;
                _contents.Add(section, new Dictionary<string, string>());
                Parsing(_reader.ReadLine());
            }
            else if (line[0] == ';' || line[0] == '#')
            {
                // comment
                Parsing(_reader.ReadLine());
            }
            else
            {
                string[] arr = line.Split(';', '#');
                string contents = arr[0].Trim();
                string[] keyValue = contents.Split('=');
                _contents[_currentSection].Add(keyValue[0].Trim(), keyValue[1].Trim());
                Parsing(_reader.ReadLine());
            }
        }

        private string GetSection(string line)
        {
            string[] arr = line.Split('[', ']');
            return arr[1].Trim();
        }

        public string GetIniValue(string section, string key)
        {
            if (_contents.ContainsKey(section) == false)
                return null;

            if (_contents[section].ContainsKey(key) == false)
                return null;

            return _contents[section][key];
        }

        private StreamReader _reader = null;
        private string _file;
        private string _currentSection;
        Dictionary<string, Dictionary<string, string>> _contents = new Dictionary<string, Dictionary<string, string>>();
    }
}
