using MicroDust.Attribute;
using MicroDust.Core.Helper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MicroDust.Core
{
    public class SQL<TSource> : IDisposable where TSource : class
    {
        /// <summary>
        /// sql 字符串
        /// </summary>
        private StringBuilder sbSql = new StringBuilder();
        /// <summary>
        /// 执行参数
        /// </summary>
        private List<SqlParameter> sqlParameters = new List<SqlParameter>();
        /// <summary>
        /// sql语句
        /// </summary>
        internal StringBuilder SQLSentence { get => this.sbSql; }
        /// <summary>
        /// sql 语句参数
        /// </summary>
        internal List<SqlParameter> SqlParameters { get => this.sqlParameters; }

        #region Find
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SQL<TSource> Find()
        {
            string selectStr = ModelConvertor.ConvertToSelectSql<TSource>();
            this.sbSql.Append(selectStr);

            return this;
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public SQL<TSource> Find(string sql)
        {
            return this.Find(sql, null);
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public SQL<TSource> Find(string sql, params SqlParameter[] sqlParameters)
        {
            return this.UseSql(sql, sqlParameters);
        }
        /// <summary>
        /// 查询全部根据条件
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public SQL<TSource> Find(Expression<Func<TSource, bool>> expression)
        {
            string selectStr = ModelConvertor.ConvertToSelectSql<TSource>();
            Tuple<string, List<SqlParameter>> whereParam = LambdaToSqlHelper.SqlServerResolver.ToSql(expression.Body);
            string whereStr = whereParam.Item1;
            this.sqlParameters = whereParam.Item2;

            if (!string.IsNullOrEmpty(whereStr))
            {
                this.sbSql.Append($"{selectStr} WHERE {whereStr}");
            }
            else
            {
                this.sbSql.Append($"{selectStr}");
            }

            return this;
        }
        /// <summary>
        /// 选择获取项信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public object Select(Func<TSource, object> selector)
        {
            Type type = typeof(TSource);
            TSource ts = type.Assembly.CreateInstance(type.FullName) as TSource;
            StringBuilder sbJson = new StringBuilder("[");
            string columnName = string.Empty;

            object obj = selector?.Invoke(ts);

            DataTable dt = DBHelperExt.ExecuteDataTable(System.Data.CommandType.Text, this.sbSql.ToString(), this.sqlParameters.ToArray());

            foreach (DataRow dr in dt.Rows)
            {
                sbJson.Append("{");
                foreach (PropertyInfo pi in obj.GetType().PropertyByASC())
                {
                    columnName = pi.GetColumnName();

                    sbJson.Append($"\"{columnName}\":\"{dr[columnName]}\",");
                }
                sbJson.Remove(sbJson.Length - 1, 1);
                sbJson.Append("},");
            }

            sbJson.Remove(sbJson.Length - 1, 1);
            sbJson.Append("]");

            return JsonConvert.DeserializeObject(sbJson.ToString());
        }

        /// <summary>
        /// 通过主键值查询
        /// </summary>
        /// <param name="value">主键值</param>
        /// <returns></returns>
        public SQL<TSource> FindByKey(object value)
        {
            if (null == value)
            {
                new MicroDustException("参数不能为NULL");
            }

            string selectStr = ModelConvertor.ConvertToSelectSql<TSource>();
            string key = this.GetKeyName();

            this.sbSql.Append($"{selectStr} WHERE {key}=@{key}");
            this.sqlParameters.Add(new SqlParameter($"@{key}", value));

            return this;
        }
        #endregion

        #region Add
        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SQL<TSource> Add(TSource entity)
        {
            if (null == entity)
                new MicroDustException("添加实体对象不可为NULL");

            this.sbSql.Append(ModelConvertor.ConvertToInsertSql<TSource>());

            Type t = entity.GetType();
            PropertyInfo[] properties = t.PropertyByASC();

            foreach (PropertyInfo pi in properties)
            {
                if (!pi.ColumnIsIgnore())
                {
                    this.sqlParameters.Add(new SqlParameter($"@{pi.GetColumnName()}", pi.GetValue(entity)));
                }
            }

            return this;
        }

        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SQL<TSource> Add(string sql)
        {
            return this.Add(sql, null);
        }
        /// <summary>
        /// 添加实体数据到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SQL<TSource> Add(string sql, params SqlParameter[] sqlParameters)
        {
            return this.UseSql(sql, sqlParameters);
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
        internal SQL<TSource> Increment(object updateObj)
        {
            return this.Increment(updateObj, string.Empty, null);
        }
        /// <summary>
        /// 修改增值字段,修改字段必须为int、long类型
        /// 
        /// <para>updateObj 的字段名称与 TSource 的字段名称保持一致</para>
        /// </summary>
        /// <param name="updateObj"></param>
        /// <param name="value">主键对应的值</param>
        /// <returns></returns>
        internal SQL<TSource> Increment(object updateObj, object value)
        {
            return this.Increment(updateObj, string.Empty, value);
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
        internal SQL<TSource> Increment(object updateObj, string key, object value)
        {
            string updateStr = ModelConvertor.ConvertToUpdateSql<TSource>();
            string whereStr = string.Empty;
            StringBuilder sbUpdateField = new StringBuilder();
            string columnName = string.Empty;

            Type t = updateObj.GetType();
            long increment = 0;
            object proValue;

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                columnName = pi.GetColumnName();
                proValue = pi.GetValue(updateObj);
                if (null != proValue && long.TryParse(proValue.ToString(), out increment))
                {
                    sbUpdateField.Append($"{columnName}={columnName} + (@{columnName}),");

                    this.sqlParameters.Add(new SqlParameter($"@{columnName}", increment));
                }
            }

            if (sbUpdateField.Length > 0)
            {
                sbUpdateField.Remove(sbUpdateField.Length - 1, 1);
            }

            if (string.IsNullOrEmpty(key) && null != value)
            {
                key = this.GetKeyName();
                if (string.IsNullOrEmpty(key))
                {
                    throw new MicroDustException("无法找到有效标识主键属性");
                }
            }

            if (!string.IsNullOrEmpty(key) && null != value)
            {
                whereStr = $"{key}=@{key}";
                this.sqlParameters.Add(new SqlParameter($"@{key}", value));
            }

            if (string.IsNullOrEmpty(whereStr))
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()}");
            }
            else
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()} WHERE {whereStr}");
            }

            return this;
        }
        #endregion

        #region Delete
        /// <summary>
        /// 从数据库删除实体
        /// </summary>
        /// <param name="value">键值</param>
        /// <returns></returns>
        public SQL<TSource> Delete(object value)
        {
            string key = this.GetKeyName();

            return this.Delete(key, value);
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public SQL<TSource> Delete(TSource entity)
        {
            string key = string.Empty;
            object value = string.Empty;
            this.GetKeyValue(entity, out key, out value);

            return this.Delete(key, value);
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="expression">lambda表达式</param>
        /// <returns></returns>
        public SQL<TSource> Delete(Expression<Func<TSource, bool>> expression)
        {
            string deleteStr = ModelConvertor.ConvertToDeleteSql<TSource>();
            Tuple<string, List<SqlParameter>> whereParam = LambdaToSqlHelper.SqlServerResolver.ToSql(expression.Body);
            string whereStr = whereParam.Item1;
            this.sqlParameters = whereParam.Item2;

            this.sbSql.Append($"{deleteStr} WHERE {whereStr}");

            return this;
        }
        /// <summary>
        /// 从数据库中删除实体
        /// </summary>
        /// <param name="key">键字段名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public SQL<TSource> Delete(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = this.GetKeyName();
                if (string.IsNullOrEmpty(key))
                {
                    throw new MicroDustException("无法找到主键标识属性");
                }
            }
            if (null == value)
            {
                throw new MicroDustException("value不能为NULL");
            }
            string deleteStr = ModelConvertor.ConvertToDeleteSql<TSource>();
            string whereStr = $"{key} = @{key}";

            this.sbSql.Append($"{deleteStr} WHERE {whereStr}");

            this.sqlParameters.Add(new SqlParameter($"@{key}", value));

            return this;
        }
        /// <summary>
        ///  从数据库中删除实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public SQL<TSource> Delete(string sql)
        {
            return Delete(sql, null);
        }
        /// <summary>
        ///  从数据库中删除实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParameters">参数</param>
        /// <returns></returns>
        public SQL<TSource> Delete(string sql, params SqlParameter[] sqlParameters)
        {
            return UseSql(sql, sqlParameters);
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public SQL<TSource> Update(object updateObj)
        {
            return this.Update(updateObj, null, null);
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public SQL<TSource> Update(object updateObj, object value)
        {
            return this.Update(updateObj, null, value);
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// <param name="updateObj">修改对象</param>
        /// <param name="key">主键字段名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public SQL<TSource> Update(object updateObj, string key, object value)
        {
            string updateStr = ModelConvertor.ConvertToUpdateSql<TSource>();
            string whereStr = string.Empty;
            StringBuilder sbUpdateField = new StringBuilder();
            string columnName = string.Empty;

            Type t = updateObj.GetType();

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                columnName = pi.GetColumnName();

                if (!pi.ColumnIsIgnore())
                {
                    sbUpdateField.Append($"{columnName}=@{columnName},");

                    this.sqlParameters.Add(new SqlParameter($"@{columnName}", pi.GetValue(updateObj)));
                }
            }

            if (sbUpdateField.Length > 0)
            {
                sbUpdateField.Remove(sbUpdateField.Length - 1, 1);
            }

            if (string.IsNullOrEmpty(key) && null != value)
            {
                key = this.GetKeyName();
                if (string.IsNullOrEmpty(key))
                {
                    throw new MicroDustException("无法找到有效标识主键属性");
                }
            }

            if (!string.IsNullOrEmpty(key) && null != value)
            {
                whereStr = $"{key}=@{key}";
                this.sqlParameters.Add(new SqlParameter($"@{key}", value));
            }

            if (!string.IsNullOrEmpty(whereStr))
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()} WHERE {whereStr}");
            }
            else
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()}");
            }

            return this;
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <returns></returns>
        public SQL<TSource> Update(TSource entity)
        {
            string updateStr = ModelConvertor.ConvertToUpdateSql<TSource>();
            string whereStr = string.Empty;
            string key = string.Empty;
            StringBuilder sbUpdateField = new StringBuilder();
            Type t = entity.GetType();
            string columnName = string.Empty;

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                columnName = pi.GetColumnName();
                if (pi.IsDefined(typeof(KeyAttribute)))
                {
                    key = pi.Name;
                    whereStr = $"{columnName}=@{columnName}";
                }
                else
                {
                    sbUpdateField.Append($"{columnName}=@{columnName},");
                }
                this.sqlParameters.Add(new SqlParameter($"@{columnName}", pi.GetValue(entity)));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new MicroDustException("无法找到主键标识属性");
            }

            if (sbUpdateField.Length > 0)
            {
                sbUpdateField.Remove(sbUpdateField.Length - 1, 1);
            }

            if (!string.IsNullOrEmpty(whereStr))
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()} WHERE {whereStr}");
            }
            else
            {
                this.sbSql.Append($"{updateStr} {sbUpdateField.ToString()}");
            }

            return this;
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <returns></returns>
        public SQL<TSource> Update(string sql)
        {
            return this.Update(sql, null);
        }
        /// <summary>
        /// 修改
        /// <para>修改对象的属性字段必须与TSource的一致</para>
        /// </summary>
        /// <returns></returns>
        public SQL<TSource> Update(string sql, params SqlParameter[] sqlParameters)
        {
            return this.UseSql(sql, sqlParameters);
        }
        #endregion
        /// <summary>
        /// 建立分组字段
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SQL<TSource> GroupBy(Func<TSource, object> groupBy)
        {
            Type t = typeof(TSource);
            Assembly ass = t.Assembly;
            TSource tSource = ass.CreateInstance(t.FullName) as TSource;
            object obj = groupBy?.Invoke(tSource);
            StringBuilder sbGroupByField = new StringBuilder();

            Type tGroupBy = obj.GetType();

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                if (!pi.ColumnIsIgnore())
                {
                    sbGroupByField.Append($" {pi.GetColumnName()},");
                }
            }

            if (sbGroupByField.Length > 0)
            {
                sbGroupByField.Remove(sbGroupByField.Length - 1, 1);
            }
            this.sbSql.Append($" GROUP BY {sbGroupByField}");

            return this;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            Console.WriteLine(this.sbSql.ToString());

            return DBHelperExt.ExecuteNonQuery(System.Data.CommandType.Text, this.sbSql.ToString(), this.sqlParameters.ToArray());
        }
        /// <summary>
        /// 将查询到的内容转换成list集数据
        /// </summary>
        /// <returns></returns>
        public List<TSource> ToList()
        {
            Console.WriteLine(this.sbSql.ToString());

            List<TSource> list = ModelConvertor.ConvertToList<TSource>(DBHelperExt.ExecuteDataTable(System.Data.CommandType.Text, this.sbSql.ToString(), this.sqlParameters.ToArray()));

            return list ?? new List<TSource>();
        }
        /// <summary>
        /// 将查询到的内容转换成实体数据
        /// </summary>
        /// <returns></returns>
        public TSource ToEntity()
        {
            Console.WriteLine(this.sbSql.ToString());

            return ModelConvertor.ConvertToEntity<TSource>(DBHelperExt.ExecuteDataTable(System.Data.CommandType.Text, this.sbSql.ToString(), this.sqlParameters.ToArray()));
        }

        #region Private Method
        /// <summary>
        /// 通过sql语句的方式执行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        private SQL<TSource> UseSql(string sql, SqlParameter[] sqlParameters)
        {
            if (string.IsNullOrEmpty(sql))
                new MicroDustException("执行sql语句不能为空字符或NULL");

            this.sbSql.Append(sql);
            if (null != sqlParameters)
                this.sqlParameters = sqlParameters.ToList();

            return this;
        }
        /// <summary>
        /// 从实体中获取键值字符串
        /// 
        /// <para>key=value</para>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private void GetKeyValue(TSource entity, out string key, out object value)
        {
            Type t = entity.GetType();
            key = string.Empty;
            value = string.Empty;

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                if (pi.IsDefined(typeof(KeyAttribute)))
                {
                    key = pi.Name;
                    value = pi.GetValue(entity);
                }
            }
        }
        /// <summary>
        /// 获取键值名称
        /// </summary>
        /// <returns></returns>
        private string GetKeyName()
        {
            Type t = typeof(TSource);

            foreach (PropertyInfo pi in t.PropertyByASC())
            {
                if (pi.IsDefined(typeof(KeyAttribute)))
                {
                    return pi.GetColumnName();
                }
            }

            return string.Empty;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        #endregion
    }
}
