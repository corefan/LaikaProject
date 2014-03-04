using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Laika.Data.Ini;
using Laika.Data.Json;
using Laika.Data.XmlUtility;
using System.IO;

namespace TestData
{
    class Program
    {
        static void Main(string[] args)
        {
            TestJson();
            TestIni();
            TestXml();
        }

        private static void TestXml()
        {
            //XML file
            //<BookStore>
            //    <book id="1234">
            //        <author>tony1</author>
            //        <number id="attr">1</number>
            //    </book>
            //</BookStore>
            string dirInfo = Directory.GetCurrentDirectory();
            XmlUtility xu = new XmlUtility(Path.Combine(dirInfo, "test.xml"));
            string value = xu.GetValue("book", "author");
            string attr = xu.GetAttributeValue("id", "book", "number");
            XmlNode node = XmlUtility.GetNode(xu.Document, "book");
            XmlNode node1 = XmlUtility.GetNode(node, "author");
            string value1 = XmlUtility.GetNodeValue(node1);
            XmlNode node2 = XmlUtility.GetNode(node, "number");
            string attr1 = XmlUtility.GetAttributeValue(node2, "id");
        }

        private static void TestIni()
        {
            // ini file
            //[my_section]
            //hello=world
            //myname=Tony Stark
            //[your_section]
            //hi=there

            string dirInfo = Directory.GetCurrentDirectory();
            Ini i = new Ini(Path.Combine(dirInfo, "test.ini"));
            i.SetIniValue("my_section", "hello", "world");
            i.SetIniValue("your_section", "hi", "there");
            i.SetIniValue("my_section", "myname", "Tony Stark");
            string value = i.GetIniValue("my_section", "hello");
        }

        private static void TestJson()
        {
            //json file
            //{
            //    "test":
            //    {
            //        "key1":"value1",
            //        "key2":["v1", "v2", "v3"],
            //    }
            //}
            string dirInfo = Directory.GetCurrentDirectory();
            JsonToObject j = new JsonToObject(Path.Combine(dirInfo, "test.json"));
            object o = j.GetRoot();
            object c = j.GetValue(o, "test");
            object l = j.GetValue(c, "key2");
        }
    }
}
