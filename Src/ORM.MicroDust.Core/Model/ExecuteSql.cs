using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    /// <summary>
    /// 执行sql值
    /// </summary>
    public sealed class ExecuteSql
    {
        public ExecuteSql()
        {
            this.SqlParameters = null;
        }

        public string SQL { get; set; }
        public List<SqlParameter> SqlParameters { get; set; }
    }
}
