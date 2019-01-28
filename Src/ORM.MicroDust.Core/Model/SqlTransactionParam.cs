using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    public sealed class SqlTransactionParam
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 执行的sql语句
        /// </summary>
        public string SqlText { get; set; }
        /// <summary>
        /// sql语句参数数组
        /// </summary>
        public SqlParameter[] SqlParameters { get; set; }
    }
}
