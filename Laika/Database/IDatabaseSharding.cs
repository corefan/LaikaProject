using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Database
{
    /// <summary>
    /// 데이터베이스 샤딩 인터페이스
    /// </summary>
    /// <typeparam name="KeyType">샤딩 키 타입</typeparam>
    /// <typeparam name="DBKeyType">DB 키 타입</typeparam>
    public interface IDatabaseSharding<KeyType, DBKeyType>
    {
        /// <summary>
        /// 샤딩 키로 데이터베이스를 찾는 메소드
        /// </summary>
        /// <param name="key">샤딩 키</param>
        /// <returns>데이터베이스 인터페이스</returns>
        IDatabase FindDB(KeyType key);
        /// <summary>
        /// 모든 DB에 비동기 작업 수행
        /// </summary>
        /// <param name="job">DB 작업</param>
        /// <returns>모든 작업에 대한 비동기 Task</returns>
        List<Task> AllDatabaseExecuteAsync(DbJobBase job);
        /// <summary>
        /// DB 추가
        /// </summary>
        /// <param name="dbId">DB Key</param>
        /// <param name="db">데이터페이스 인스턴스</param>
        void AddDatabase(DBKeyType dbId, IDatabase db);
        /// <summary>
        /// DB 리스트 메소드
        /// </summary>
        /// <returns>DB 리스트</returns>
        IList<IDatabase> GetDatabaseList();
    }
}
