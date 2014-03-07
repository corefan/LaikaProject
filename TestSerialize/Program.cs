using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Serialize.XML;
using Laika.Serialize.ProtobufNet;
using ProtoBuf;

namespace TestSerialize
{
    public class Person
    {
        public string Name;
        public int Age;
    }

    [ProtoContract]
    public class Student
    {
        [ProtoMember(1)]
        public string Class;

        [ProtoMember(2)]
        public int Number;

        [ProtoMember(3)]
        public string Name;
    }

    class Program
    {
        static void Main(string[] args)
        {
            XmlTest();
            ProtoBufTest();
        }

        private static void ProtoBufTest()
        {
            Student s = new Student();
            s.Class = "A";
            s.Name = "Anold";
            s.Number = 10;

            byte[] serialize = ProtobufNetUtility.Serialize<Student>(s);
            Student de = ProtobufNetUtility.Deserialize<Student>(serialize);
            string protoString = ProtobufNetUtility.GetProtoString<Student>();
        }

        private static void XmlTest()
        {
            Person p = new Person();
            p.Name = "Tony";
            p.Age = 33;

            string xml = SimpleXmlSerializeUtility.Serialize(p);
            Person p1 = SimpleXmlSerializeUtility.Deserialize<Person>(xml);
        }
    }
}
