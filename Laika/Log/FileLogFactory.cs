using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Log
{
    /// <summary>
    /// 파일로그 인스턴스 팩토리 클래스
    /// </summary>
    public class FileLogFactory
    {
        /// <summary>
        /// 파일로그 인스턴스를 생성하여 리턴합니다.
        /// </summary>
        /// <param name="param">원하는 옵션을 파라미터로 지정합니다.</param>
        /// <returns>로그 인스턴스를 리턴합니다.</returns>
        public static IFileLog CreateFileLog(FileLogParameter param)
        {
            if (param.Type == PartitionType.NONE)
            {
                return new NormalLog(param);
            }
            else if (param.Type == PartitionType.FILE_SIZE)
            {
                return new SizeLog(param);
            }
            else if (param.Type == PartitionType.TIME)
            {
                return new TimeLog(param);
            }
            else
                throw new ArgumentException("Invalid partition type.");
        }
    }
}
