using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Log
{
    /// <summary>
    /// 파일로그 파티션 타입. 
    /// </summary>
    public enum PartitionType
    {
        /// <summary>
        /// 일반 로그. 한개의 파일에 로그를 누적합니다.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 시간별 로그. 단위는 분이며, 로깅 시작 후 분별로 파일을 만듭니다. 
        /// 로그가 없을 경우 매 단위 분마다 파일을 생성하지는 않습니다.
        /// </summary>
        TIME,
        /// <summary>
        /// 파일 사이즈별 로그. 단위는 바이트(Byte)이며, 
        /// 파일이 지정한 사이즈 이상 넘어갈 경우 새로운 파일을 만듭니다.
        /// </summary>
        FILE_SIZE,
        /// <summary>
        /// 윈도우즈 이벤트 로그. 관리자 권한이 필요합니다.
        /// </summary>
        WINDOWS_EVENT,
    }
}
