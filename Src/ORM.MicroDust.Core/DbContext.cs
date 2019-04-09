using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    public class DbContext
    {
        public DbContext(string name)
        {
            DBHelperExt.SetConnSting(name);
        }
        /// <summary>
        /// 获取事务执行类
        /// </summary>
        /// <returns></returns>
        public DBSqlTransaction DBSqlTransaction
        {
            get { return DBSqlTransaction.SqlTransaction; }
        }

        #region Find 
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SQL<T> Find<T>() where T : class
        {
            return new SQL<T>().Find();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public SQL<T> Find<T>(string sql) where T : class
        {
            return this.Find<T>(sql, null);
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public SQL<T> Find<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            return new SQL<T>().Find(sql, sqlParameters);
        }
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SQL<T> Find<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return new SQL<T>().Find(expression);
        }
        /// <summary>
        /// 通过主键值查询
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SQL<T> FindByKey<T>(object value) where T : class
        {
            return new SQL<T>().FindByKey(value);
        }
        #endregion

        #region Add
        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Add<T>(T entity) where T : class
        {
            return new SQL<T>().Add(entity).Execute();
        }
        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add<T>(string sql) where T : class
        {
            return this.Add<T>(sql, null);
        }
        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            return new SQL<T>().Add(sql, sqlParameters).Execute();
        }
        #endregion

        #region Delete 
        /// <summary>
        /// 从数据库删除实体
        /// </summary>
        /// <param name="value">键值</param>
        /// <returns></returns>
        public int Delete<T>(object value) where T : class
        {
            return this.Delete<T>(null, value);
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="key">键字段名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int Delete<T>(string key, object value) where T : class
        {
            return new SQL<T>().Delete(key, value).Execute();
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public int Delete<T>(T entity) where T : class
        {
            return new SQL<T>().Delete(entity).Execute();
        }
        /// <summary>
        ///  从数据库中删除实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int Delete<T>(string sql) where T : class
        {
            return this.Delete<T>(sql, null);
        }
        /// <summary>
        ///  从数据库中删除实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public int Delete<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            return new SQL<T>().Delete(sql, sqlParameters).Execute();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public int Delete<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return new SQL<T>().Delete(expression).Execute();
        }
        #endregion

        #region Increment

        /// <summary>
        /// 修改增值字段,修改字段必须为int、long类型
        /// 
        /// <para>updateObj 的字段名称与 TSource 的字段名称保持一致</para>
        /// </summary>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        public int Increment<T>(object updateObj) where T : class
        {
            return this.Increment<T>(updateObj, string.Empty, null);
        }
        /// <summary>
        /// 修改增值字段,修改字段必须为int、long类型
        /// 
        /// <para>updateObj 的字段名称与 TSource 的字段名称保持一致</para>
        /// </summary>
        /// <param name="updateObj"></param>
        /// <param name="value">主键对应的值</param>
        /// <returns></returns>
        public int Increment<T>(object updateObj, object value) where T : class
        {
            return this.Increment<T>(updateObj, string.Empty, value);
        }
        /// <summary>
        /// 修改增值字段,修改字段必须为int、long类型
        /// 
        /// <para>updateObj 的字段名称与 TSource 的字段名称保持一致</para>
        /// </summary>
        /// <param name="updateObj"></param>
        /// <param name="key">主键字段名称</param>
        /// <param name="value">主键对应的值</param>
        /// <returns></returns>
        public int Increment<T>(object updateObj, string key, object value) where T : class
        {
            return new SQL<T>().Increment(updateObj, key, value).Execute();
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int Update<T>(object updateObj) where T : class
        {
            return new SQL<T>().Update(updateObj).Execute();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="updateObj">修改字段对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int Update<T>(object updateObj, object value) where T : class
        {
            return new SQL<T>().Update(updateObj, value).Execute();
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// <param name="updateObj">修改对象</param>
        /// <param name="key">主键字段名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int Update<T>(object updateObj, string key, object value) where T : class
        {
            return new SQL<T>().Update(updateObj, key, value).Execute();
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <returns></returns>
        public int Update<T>(T entity) where T : class
        {
            return new SQL<T>().Update(entity).Execute();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public int Update<T>(string sql) where T : class
        {
            return this.Update<T>(sql, null);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public int Update<T>(string sql, params SqlParameter[] sqlParameters) where T : class
        {
            return new SQL<T>().Update(sql, sqlParameters).Execute();
        }
        #endregion
    }
}
