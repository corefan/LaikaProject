using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Laika.Data.XmlUtility
{
    /// <summary>
    /// XML 유틸리티 클래스
    /// </summary>
    public class XmlUtility
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="file">파일 위치 및 이름</param>
        public XmlUtility(string file)
        {
            _file = file;
            Parsing();
        }
        /// <summary>
        /// xml document
        /// </summary>
        public XmlDocument Document
        {
            get
            {
                return _document;
            }
        }
        /// <summary>
        /// 속성 값 구하는 메소드
        /// </summary>
        /// <param name="attributeName">속성 이름</param>
        /// <param name="xPaths">xpath list</param>
        /// <returns>값</returns>
        public string GetAttributeValue(string attributeName, params string[] xPaths)
        {
            XmlNode node = GetNode(xPaths);
            if (node.Attributes[attributeName] == null)
                return null;

            return node.Attributes[attributeName].Value;
        }
        /// <summary>
        /// node 값을 구하는 메소드
        /// </summary>
        /// <param name="xPaths">xpath list</param>
        /// <returns>값</returns>
        public string GetValue(params string[] xPaths)
        {
            XmlNode node = GetNode(xPaths);

            return node.InnerText;
        }
        /// <summary>
        /// node list 구하는 메소드
        /// </summary>
        /// <param name="node">상위 노드</param>
        /// <param name="xPath">구하려는 node list의 xpath</param>
        /// <returns></returns>
        public static List<XmlNode> GetNodes(XmlNode node, string xPath)
        {
            var list = node.SelectNodes(xPath);
            return GetNodes(list);
        }
        /// <summary>
        /// node list 구하는 메소드
        /// </summary>
        /// <param name="doc">xml document</param>
        /// <param name="xPath">구하려는 node list xpath</param>
        /// <returns>노드 리스트</returns>
        public static List<XmlNode> GetNodes(XmlDocument doc, string xPath)
        {
            var list = doc.DocumentElement.SelectNodes(xPath);
            return GetNodes(list);
        }

        private static List<XmlNode> GetNodes(XmlNodeList list)
        {
            if (list.Count <= 0)
                return null;

            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in list)
            {
                nodes.Add(node);
            }

            return nodes;
        }
        /// <summary>
        /// 노드 값 구하는 메소드
        /// </summary>
        /// <param name="node">xml node</param>
        /// <returns>값</returns>
        public static string GetNodeValue(XmlNode node)
        {
            return node.InnerText;
        }
        /// <summary>
        /// 속성 값 구하는 메소드
        /// </summary>
        /// <param name="node">xml node</param>
        /// <param name="attributeName">속성 이름</param>
        /// <returns>값</returns>
        public static string GetAttributeValue(XmlNode node, string attributeName)
        {
            if (node.Attributes[attributeName] == null)
                return null;

            return node.Attributes[attributeName].Value;
        }
        /// <summary>
        /// node 구하는 메소드
        /// </summary>
        /// <param name="doc">xml element</param>
        /// <param name="xPath">xPath</param>
        /// <returns>XmlNode</returns>
        public static XmlNode GetNode(XmlDocument doc, string xPath)
        {
            return doc.DocumentElement.SelectSingleNode(xPath);
        }
        /// <summary>
        /// node 구하는 메소드
        /// </summary>
        /// <param name="node">xml node</param>
        /// <param name="xPath">xPath</param>
        /// <returns>xml node</returns>
        public static XmlNode GetNode(XmlNode node, string xPath)
        {
            return node.SelectSingleNode(xPath);
        }

        private XmlNode GetNode(string[] args)
        {
            IEnumerator e = args.GetEnumerator();

            XmlNode node = null;
            while (e.MoveNext())
            {
                if (node == null)
                    node = _document.DocumentElement.SelectSingleNode((string)e.Current);
                else
                    node = node.SelectSingleNode((string)e.Current);
            }
            return node;
        }

        private void Parsing()
        {
            _document = new XmlDocument();
            _document.Load(_file);
        }

        private XmlDocument _document;
        private string _file;
    }
}
