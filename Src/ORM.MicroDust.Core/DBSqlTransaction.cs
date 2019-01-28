using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MicroDust.Core
{
    public sealed class DBSqlTransaction : IDisposable
    {
        private static readonly object synObject = new object();
        private List<ExecuteSql> executeSqls = new List<ExecuteSql>();
        private DBSqlTransaction() { }

        public static DBSqlTransaction SqlTransaction
        {
            get
            {
                lock (synObject)
                {
                    return new DBSqlTransaction();
                }
            }
        }

        #region 增加
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Add<T>(T entity) where T : class
        {
            this.GetExecuteSql<T>(entity, SQLOperate.ADD);
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        public void Add<T>(string sql) where T : class
        {
            this.Add<T>(sql, null);
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameters">参数</param>
        public void Add<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            this.GetExecuteSql<T>(sql, sqlParameters, SQLOperate.ADD);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entity">实体对象</param>
        public void Update<T>(T entity) where T : class
        {
            this.GetExecuteSql<T>(entity, SQLOperate.UPDATE);
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void Update<T>(object updateObj) where T : class
        {
            using (SQL<T> sql = new SQL<T>())
            {
                sql.Update(updateObj);

                executeSqls.Add(new ExecuteSql
                {
                    SQL = sql.SQLSentence.ToString(),
                    SqlParameters = sql.SqlParameters
                });
            }
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void Update<T>(object updateObj, object value) where T : class
        {
            this.Update<T>(updateObj, null, value);
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void Update<T>(object updateObj, string key, object value) where T : class
        {
            using (SQL<T> sql = new SQL<T>())
            {
                sql.Update(updateObj, key, value);

                executeSqls.Add(new ExecuteSql
                {
                    SQL = sql.SQLSentence.ToString(),
                    SqlParameters = sql.SqlParameters
                });
            }
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        public void Update<T>(string sql) where T : class
        {
            this.Update<T>(sql, null);
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameters">参数</param>
        public void Update<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            this.GetExecuteSql<T>(sql, sqlParameters, SQLOperate.UPDATE);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entity">实体对象</param>
        public void Delete<T>(T entity) where T : class
        {
            this.GetExecuteSql<T>(entity, SQLOperate.DELETE);
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="expression">lambda表达式</param>
        /// <returns></returns>
        public void Delete<T>(Expression<Func<T, bool>> expression) where T : class
        {
            using (SQL<T> sql = new SQL<T>())
            {
                sql.Delete(expression);

                executeSqls.Add(new ExecuteSql
                {
                    SQL = sql.SQLSentence.ToString(),
                    SqlParameters = sql.SqlParameters
                });
            }
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        public void Delete<T>(string sql) where T : class
        {
            this.Delete<T>(sql, null);
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameters">参数</param>
        public void Delete<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            this.GetExecuteSql<T>(sql, sqlParameters, SQLOperate.DELETE);
        }
        #endregion
        /// <summary>
        /// 将加入事务的操作提交到数据库，执行错误异常则回滚
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            SqlConnection conn = null;
            SqlTransaction tran = null;
            int rows = 0;

            try
            {
                conn = DBHelper.GetConnection();
                conn.Open();
                tran = conn.BeginTransaction();
                foreach (ExecuteSql sql in this.executeSqls)
                {
                    Console.WriteLine(sql.SQL);

                    rows += DBHelper.ExecuteNonQuery(tran, CommandType.Text, sql.SQL, sql.SqlParameters.ToArray());
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();

                throw ex;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();
                if (conn != null)
                    conn.Close();
            }

            return rows;
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqlTransactionParams"></param>
        /// <returns></returns>
        public int Execute(params SqlTransactionParam[] sqlTransactionParams)
        {
            SqlConnection conn = null;
            SqlTransaction tran = null;
            int rows = 0;

            try
            {
                conn = DBHelper.GetConnection();
                conn.Open();
                tran = conn.BeginTransaction();
                foreach (SqlTransactionParam param in sqlTransactionParams)
                {
                    rows += DBHelper.ExecuteNonQuery(tran, param.CommandType, param.SqlText, param.SqlParameters);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();

                throw ex;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();
                if (conn != null)
                    conn.Close();
            }

            return rows;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        #region Private Method
        /// <summary>
        ///  获取执行sql
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">执行sql语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <param name="operate">sql操作</param>
        private void GetExecuteSql<T>(string sql, SqlParameter[] sqlParameters, SQLOperate operate) where T : class
        {
            using (SQL<T> sqlT = new SQL<T>())
            {
                switch (operate)
                {
                    case SQLOperate.ADD:
                        sqlT.Add(sql, sqlParameters);
                        break;
                    case SQLOperate.UPDATE:
                        sqlT.Update(sql, sqlParameters);
                        break;
                    case SQLOperate.DELETE:
                        sqlT.Delete(sql, sqlParameters);
                        break;
                    default:
                        throw new MicroDustException("无效sql操作");
                }

                executeSqls.Add(new ExecuteSql
                {
                    SQL = sqlT.SQLSentence.ToString(),
                    SqlParameters = sqlT.SqlParameters
                });
            }
        }
        /// <summary>
        /// 获取执行sql
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="operate">sql操作</param>
        private void GetExecuteSql<T>(T entity, SQLOperate operate) where T : class
        {
            using (SQL<T> sql = new SQL<T>())
            {
                switch (operate)
                {
                    case SQLOperate.ADD:
                        sql.Add(entity);
                        break;
                    case SQLOperate.UPDATE:
                        sql.Update(entity);
                        break;
                    case SQLOperate.DELETE:
                        sql.Delete(entity);
                        break;
                    default:
                        throw new MicroDustException("无效sql操作");
                }

                executeSqls.Add(new ExecuteSql
                {
                    SQL = sql.SQLSentence.ToString(),
                    SqlParameters = sql.SqlParameters
                });
            }
        }
        #endregion
    }
}
