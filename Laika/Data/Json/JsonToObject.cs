using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Laika.Data.Json
{
    /// <summary>
    /// Json 파일 읽기 클래스
    /// </summary>
    public class JsonToObject
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="file">json 파일 위치 및 이름</param>
        public JsonToObject(string file)
        {
            _file = file;
            Parsing();
        }

        private void Parsing()
        {
            using (StreamReader reader = File.OpenText(_file))
            {
                _contents = reader.ReadToEnd();
            }
        }
        /// <summary>
        /// 최상위 root 구하기
        /// </summary>
        /// <returns>Json format에 따라 IList 또는, IDictionary</returns>
        public object GetRoot()
        {
            return MiniJSON.Json.Deserialize(_contents);
        }
        /// <summary>
        /// 값 구하는 메소드
        /// </summary>
        /// <param name="keyValueData">콜렉션</param>
        /// <param name="key">키</param>
        /// <returns>값</returns>
        public object GetValue(object keyValueData, string key)
        {
            IDictionary data = (IDictionary)keyValueData;
            if (data == null)
                throw new ArgumentNullException();

            if (data.Contains(key) == false)
                throw new ArgumentNullException();

            return data[key];
        }
        /// <summary>
        /// IDictionary / IList 객체나 간단한 타입(string, int 등) json string으로 변환
        /// </summary>
        /// <param name="data">IDictionary나 IList</param>
        /// <returns>json string</returns>
        public string Serialize(object data)
        {
            return MiniJSON.Json.Serialize(data);
        }

        private string _file;
        private string _contents;
    }
}
