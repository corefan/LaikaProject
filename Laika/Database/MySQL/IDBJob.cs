using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Database;

namespace Laika.Database.MySqlDB
{
    public interface IDBJob
    {
        MySqlJob.JobDelegate QueryJob { get; }
        MySqlJob.TransactionJobDelegate TransactionQueryJob { get; }
        MySqlJob.ExceptionJobDelegate ExceptionJob { get; }
    }
}
