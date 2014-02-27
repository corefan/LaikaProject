using System;
using System.Threading.Tasks;

namespace Laika.Database
{
    /// <summary>
    /// 데이터베이스 인터페이스
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// DB job 수행 비동기 메소드
        /// </summary>
        /// <param name="job">job 객체</param>
        /// <returns>job은 비동기로 작업을 하며, Task를 return 합니다.</returns>
        Task DoJobAsync(IDBJob job);
        /// <summary>
        /// DB job 수행 메소드
        /// </summary>
        /// <param name="job">job 객체</param>
        /// <returns>job은 동기로 작업 합니다.</returns>
        void DoJob(IDBJob job);
    }
}
