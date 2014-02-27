
namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// Job 클래스
    /// </summary>
    public class MySqlJob : SqlJob
    {
        /// <summary>
        /// 일반 쿼리 job 생성
        /// </summary>
        /// <param name="job">job 람다 메소드</param>
        /// <param name="ex">예외 처리 람다 메소드</param>
        /// <returns></returns>
        public static MySqlJob CreateMySQLJob(MySqlJobDelegate job, ExceptionJobDelegate ex = null)
        {
            return new MySqlJob(job, ex);
        }

        /// <summary>
        /// Transaction job 생성
        /// </summary>
        /// <param name="job">job 람다 메소드</param>
        /// <param name="ex">예외처리 람다 메소드</param>
        /// <returns></returns>
        public static MySqlJob CreateMySQLTransactionJob(MySqlTransactionJobDelegate job, ExceptionJobDelegate ex = null)
        {
            return new MySqlJob(job, ex);
        }

        MySqlJob(MySqlJobDelegate job, ExceptionJobDelegate ex)
        {
            MySqlQueryJob = job;
            ExceptionJob = ex;
        }

        MySqlJob(MySqlTransactionJobDelegate job, ExceptionJobDelegate ex)
        {
            MySqlTransactionQueryJob = job;
            ExceptionJob = ex;
        }
    }
}
