using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laika.Database.Sharding
{
    /// <summary>
    /// 데이터베이스 샤딩 기본 클래스.
    /// 사용하기 위해서는 DatabaseShardingBase 클래스를 상속 받아서 합니다.
    /// </summary>
    /// <typeparam name="ShardKeyType">샤딩 키 타입</typeparam>
    /// <typeparam name="DBKeyType">DB 키 타입</typeparam>
    public abstract class DatabaseShardingBase<ShardKeyType, DBKeyType> : IDatabaseSharding<ShardKeyType, DBKeyType>
    {
        /// <summary>
        /// 샤딩 키로 DB 검색
        /// </summary>
        /// <param name="key">샤딩 키</param>
        /// <returns>데이터베이스</returns>
        public abstract IDatabase FindDB(ShardKeyType key);
        /// <summary>
        /// 모든 DB에 비동기 작업 수행
        /// </summary>
        /// <param name="job">작업</param>
        /// <returns>비동기 Task 리스트</returns>
        public List<Task> AllDatabaseExecuteAsync(DbJobBase job)
        {
            List<Task> list = new List<Task>();
            foreach (IDatabase db in GetDatabaseList())
            {
                Task task = db.AsyncDoJob(job);
                list.Add(task);
            }

            return list;
        }
        /// <summary>
        /// 데이터베이스 추가
        /// </summary>
        /// <param name="dbId">DB key</param>
        /// <param name="db">데이터베이스</param>
        public void AddDatabase(DBKeyType dbId, IDatabase db)
        {
            if (AllDataBase.ContainsKey(dbId) == true)
                throw new ArgumentException("Already contains key. duplicated dbId");

            AllDataBase.Add(dbId, db);
        }
        /// <summary>
        /// 데이터베이스 구하기
        /// </summary>
        /// <returns>읽기만 가능한 데이터베이스 리스트</returns>
        public IList<IDatabase> GetDatabaseList()
        {
            return AllDataBase.Values.ToList().AsReadOnly();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
            {
                Clear();
            }
            _disposed = true;
        }

        private void Clear()
        {
            List<IDatabase> dbList = AllDataBase.Values.ToList();
            dbList.ForEach(x => x.Dispose());
            AllDataBase.Clear();
        }

        ~DatabaseShardingBase()
        {
            Dispose(false);
        }

        protected ShardKeyType _shardKey = default(ShardKeyType);
        protected Dictionary<DBKeyType, IDatabase> AllDataBase = new Dictionary<DBKeyType, IDatabase>();
        private bool _disposed = false;
    }
}
