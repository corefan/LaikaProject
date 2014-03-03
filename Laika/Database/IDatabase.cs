using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Database
{
    /// <summary>
    /// Database 인터페이스
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// 비동기 작업 수행
        /// </summary>
        /// <param name="job">DB 작업</param>
        /// <returns>비동기 Task</returns>
        Task AsyncDoJob(DbJobBase job);
        /// <summary>
        /// 동기 작업 수행
        /// </summary>
        /// <param name="job">DB 작업</param>
        void DoJob(DbJobBase job);
    }
}
