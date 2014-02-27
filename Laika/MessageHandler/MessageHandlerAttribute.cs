using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.MessageHandler
{
    /// <summary>
    /// 메시지를 핸들링 하기 위한 attribute.
    /// 메소드에 메시지타입 번호를 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public MessageHandlerAttribute(int typeNo)
        {
            TypeNo = typeNo;
        }
        /// <summary>
        /// 메소드에 지정된 메시지타입 번호
        /// </summary>
        public int TypeNo { get; private set; }
    }
}
