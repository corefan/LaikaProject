using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Laika.Database
{
    /// <summary>
    /// 트랜잭션 컨텍스트 베이스 클래스
    /// </summary>
    public abstract class TransactionContextBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connection">DB Connection</param>
        public TransactionContextBase(DbConnection connection)
        {
            Connection = connection;
            Transaction = connection.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
        }

        internal void Commit()
        {
            if (NeedRollback == false)
                Transaction.Commit();
            else
                throw new Exception("Transaction rollback need.");
        }

        internal void Rollback()
        {
            Transaction.Rollback();
        }

        protected DbCommand CreateCommand(string query)
        {
            var cmd = Connection.CreateCommand();
            cmd.Connection = Connection;
            cmd.Transaction = Transaction;
            cmd.CommandText = query;
            return cmd;
        }

        internal DbTransaction Transaction { get; private set; }
        protected DbConnection Connection;
        /// <summary>
        /// 롤백이 필요할 경우 job 메소드가 끝나기 전에 true로 설정
        /// </summary>
        public bool NeedRollback { get; set; }
    }
}
