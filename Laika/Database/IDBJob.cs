
namespace Laika.Database
{
    /// <summary>
    /// Job 인터페이스
    /// </summary>
    public interface IDBJob
    {
        SqlJob.MySqlJobDelegate MySqlQueryJob { get; }
        SqlJob.MySqlTransactionJobDelegate MySqlTransactionQueryJob { get; }

        /// <summary>
        /// 일반 쿼리 작업 delegate
        /// </summary>
        SqlJob.SqlServerJobDelegate QueryJob { get; }
        /// <summary>
        /// Transaction 작업 delegate
        /// </summary>
        SqlJob.SqlServerTransactionJobDelegate TransactionQueryJob { get; }

        /// <summary>
        /// 예외 발생 시 처리할 작업 delegate
        /// </summary>
        SqlJob.ExceptionJobDelegate ExceptionJob { get; }
    }
}
