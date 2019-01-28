using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MicroDust.Core
{
    public class DBHelperExt : DBHelper
    {
        private DBHelperExt() { }

        #region ExecuteNonQuery
        /// <summary>
        /// 2018.02.01 张超 添加
        /// 执行指定连接字符串,类型的SqlCommand.  使用默认获取的数据库连接字符串
        /// </summary>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名称或SQL语句</param> 
        /// <returns></returns>
        public static int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(DBHelper.GetConnSting(), commandType, commandText, (SqlParameter[])null);
        }

        /// <summary> 
        ///  2018.02.01 张超 添加
        /// 执行指定连接字符串,类型的SqlCommand.如果没有提供参数,不返回结果. 使用默认获取的数据库连接字符串
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
        /// <param name="commandText">存储过程名称或SQL语句</param> 
        /// <param name="commandParameters">SqlParameter参数数组</param> 
        /// <returns>返回命令影响的行数</returns> 
        public static int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return DBHelper.ExecuteNonQuery(DBHelper.GetConnSting(), commandType, commandText, commandParameters);
        }
        #endregion

        #region DataTable
        /// <summary> 
        /// 执行指定数据库连接字符串的命令,返回DataTable.  使用默认数据库连接字符串
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataTable dt = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataTable</returns> 
        public static DataTable ExecuteDataTable(CommandType commandType, string commandText)
        {
            return ExecuteDataTable(commandType, commandText, (SqlParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,返回DataTable. 使用默认数据库连接字符串
        /// </summary> 
        /// <remarks> 
        /// 示例: 
        ///  DataTable dt = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的数据库连接字符串</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名称或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamters参数数组</param> 
        /// <returns>返回一个包含结果集的DataTable</returns> 
        public static DataTable ExecuteDataTable(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataTable(DBHelper.ExecuteDataset(DBHelper.GetConnSting(), commandType, commandText, commandParameters));
        }

        /// <summary> 
        /// 执行指定数据库连接字符串的命令,直接提供参数值,返回DataTable. 使用默认数据库连接字符串
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输出参数和返回值. 
        /// 示例: 
        ///  DataTable dt = ExecuteDataset(connString, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个包含结果集的DataTable</returns> 
        public static DataTable ExecuteDataTable(string spName, params object[] parameterValues)
        {
            return ExecuteDataTable(DBHelper.ExecuteDataset(DBHelper.GetConnSting(), spName, parameterValues));
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,返回DataTable.
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataTable dt = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataTable. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataTable dt = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回一个包含结果集的DataTable</returns> 
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataTable(DBHelper.ExecuteDataset(connection, commandType, commandText, commandParameters));
        }

        /// <summary> 
        /// 执行指定数据库连接对象的命令,指定参数值,返回DataTable. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输入参数和返回值. 
        /// 示例.:  
        ///  DataTable dt = ExecuteDataset(conn, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="connection">一个有效的数据库连接对象</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataTable ExecuteDataTable(SqlConnection connection, string spName, params object[] parameterValues)
        {
            return ExecuteDataTable(DBHelper.ExecuteDataset(connection, spName, parameterValues));
        }

        /// <summary> 
        /// 执行指定事务的命令,返回DataTable. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <returns>返回一个包含结果集的DataSet</returns> 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary> 
        /// 执行指定事务的命令,指定参数,返回DataTable. 
        /// </summary> 
        /// <remarks> 
        /// 示例:  
        ///  DataTable dt = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param> 
        /// <param name="commandText">存储过程名或T-SQL语句</param> 
        /// <param name="commandParameters">SqlParamter参数数组</param> 
        /// <returns>返回一个包含结果集的DataTable</returns> 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return DBHelperExt.ExecuteDataTable(DBHelper.ExecuteDataset(transaction, commandType, commandText, commandParameters));
        }

        /// <summary> 
        /// 执行指定事务的命令,指定参数值,返回DataTable. 
        /// </summary> 
        /// <remarks> 
        /// 此方法不提供访问存储过程输入参数和返回值. 
        /// 示例.:  
        ///  DataTable dt = ExecuteDataset(trans, "GetOrders", 24, 36); 
        /// </remarks> 
        /// <param name="transaction">事务</param> 
        /// <param name="spName">存储过程名</param> 
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param> 
        /// <returns>返回一个结果集</returns> 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            return DBHelperExt.ExecuteDataTable(DBHelper.ExecuteDataset(transaction, spName, parameterValues));
        }

        /// <summary>
        /// 从DataSet表集中获取第一张表，如果没有则返回null
        /// </summary>
        /// <param name="dataSet">表集</param>
        /// <returns>返回第一张表</returns>
        private static DataTable ExecuteDataTable(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }

            return null;
        }
        #endregion
    }
}
