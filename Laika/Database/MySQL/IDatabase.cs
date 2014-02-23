using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// 데이터베이스 인터페이스
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// DB job 수행 메소드
        /// </summary>
        /// <param name="job">job 객체</param>
        /// <returns>job은 비동기로 작업을 하며, Task를 return 합니다.</returns>
        Task DoJobAsync(IDBJob job);
    }
}
