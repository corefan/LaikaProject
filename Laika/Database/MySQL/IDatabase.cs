using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Database.MySqlDB
{
    public interface IDatabase : IDisposable
    {
        Task DoJob(IDBJob job);
    }
}
