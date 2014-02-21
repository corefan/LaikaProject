using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Database;

namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// Job 인터페이스
    /// </summary>
    public interface IDBJob
    {
        /// <summary>
        /// 일반 쿼리 작업 delegate
        /// </summary>
        MySqlJob.JobDelegate QueryJob { get; }

        /// <summary>
        /// Transaction 작업 delegate
        /// </summary>
        MySqlJob.TransactionJobDelegate TransactionQueryJob { get; }

        /// <summary>
        /// 예외 발생 시 처리할 작업 delegate
        /// </summary>
        MySqlJob.ExceptionJobDelegate ExceptionJob { get; }
    }
}
