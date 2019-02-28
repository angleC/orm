using MicroDust.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    internal static class ModelConvertor
    {
        /// <summary>
        /// 将查询的内容转换成对应的实体对象
        /// </summary>
        /// <returns></returns>
        public static T ConvertToEntity<T>(DataTable table) where T : class
        {
            return table == null ? default(T) : ConvertToEntity<T>(table.CreateDataReader() as IDataReader);
        }
        /// <summary>
        /// 将查询的内容转换成对应的实体对象
        /// </summary>
        /// <returns></returns>
        public static T ConvertToEntity<T>(IDataReader reader) where T : class
        {
            List<T> list = ConvertToList<T>(reader);

            if (null == list)
            {
                return default(T);
            }

            return list.FirstOrDefault();
        }
        /// <summary>
        ///     从 DataTale 对象中逐行读取记录并将记录转化为 T 类型的集合
        /// </summary>
        /// <typeparam name="T">目标类型参数</typeparam>
        /// <param name="reader">DataTale 对象。</param>
        /// <returns>指定类型的对象集合。</returns>
        public static List<T> ConvertToList<T>(DataTable table)
            where T : class
        {
            return table == null
                ? new List<T>()
                : ModelConvertor.ConvertToList<T>(table.CreateDataReader() as IDataReader);
        }
        /// <summary>
        ///     从 reader 对象中逐行读取记录并将记录转化为 T 类型的集合
        /// </summary>
        /// <typeparam name="T">目标类型参数</typeparam>
        /// <param name="reader">实现 IDataReader 接口的对象。</param>
        /// <returns>指定类型的对象集合。</returns>
        public static List<T> ConvertToList<T>(IDataReader reader) where T : class
        {
            List<T> list = new List<T>();
            T obj = default(T);
            Type t = typeof(T);
            Assembly ass = t.Assembly;

            Dictionary<string, PropertyInfo> propertys = ModelConvertor.GetFields<T>(reader);
            PropertyInfo p = null;
            if (reader != null)
            {
                while (reader.Read())
                {
                    obj = ass.CreateInstance(t.FullName) as T;
                    foreach (string key in propertys.Keys)
                    {
                        p = propertys[key];
                        p.SetValue(obj, ModelConvertor.ChangeType(reader[p.GetColumnName()], p.PropertyType), null);
                    }
                    list.Add(obj);
                }
            }

            return list;
        }
        /// <summary>
        /// 获取删除字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ConvertToDeleteSql<T>()
        {
            Type t = typeof(T);

            return $"DELETE FROM [{t.GetTableName()}]";
        }
        /// <summary>
        /// 根据类属性名称转换成select sql字符串
        /// <para>通过默认排序的方式</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ConvertToSelectSql<T>() where T : class
        {
            return $"SELECT {ConvertToFieldStr<T>("[", "]")} FROM [{typeof(T).GetTableName()}]";
        }
        /// <summary>
        /// 根据类属性名称转换成insert sql字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ConvertToInsertSql<T>() where T : class
        {
            string selectSql = string.Empty;
            Type t = typeof(T);

            return $"INSERT INTO [{t.GetTableName()}] ({ConvertToFieldStr<T>()}) VALUES ({ConvertToFieldStr<T>("@")})";
        }
        /// <summary>
        /// 修改sql字符串
        /// </summary>
        /// <returns></returns>
        public static string ConvertToUpdateSql<T>()
        {
            string selectSql = string.Empty;
            Type t = typeof(T);

            return $"UPDATE [{t.GetTableName()}] SET";
        }
        /// <summary>
        /// 根据类属性名称转换成字段字符串
        /// 
        /// <para>field,field,field,...</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefixString">前缀字符串</param>
        /// <param name="suffixString">后缀字符串</param>
        /// <returns></returns>
        public static string ConvertToFieldStr<T>(string prefixString = "", string suffixString = "") where T : class
        {
            Type t = typeof(T);
            StringBuilder sbFieldStr = new StringBuilder();

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (!pi.ColumnIsIgnore())
                {
                    sbFieldStr.Append($"{prefixString}{pi.GetColumnName()}{suffixString},");
                }
            }

            if (sbFieldStr.Length > 0)
            {
                sbFieldStr.Remove(sbFieldStr.Length - 1, 1);
            }

            return sbFieldStr.ToString();
        }
        /// <summary>
        ///     获取reader存在并且在 T 类中包含同名可写属性的集合
        /// </summary>
        /// <param name="reader">
        ///     可写域的集合
        /// </param>
        /// <returns>
        ///     以属性名为键，PropertyInfo 为值得字典对象
        /// </returns>
        public static Dictionary<string, PropertyInfo> GetFields<T>(IDataReader reader) where T : class
        {
            Dictionary<string, PropertyInfo> result = new Dictionary<string, PropertyInfo>();
            int columnCount = reader.FieldCount;
            Type t = typeof(T);

            PropertyInfo[] properties = t.GetProperties();
            if (properties != null)
            {
                List<string> readerFields = new List<string>();
                for (int i = 0; i < columnCount; i++)
                {
                    readerFields.Add(reader.GetName(i));
                }
                IEnumerable<PropertyInfo> resList =
                    (from PropertyInfo prop in properties
                     where prop.CanWrite && readerFields.Contains(prop.GetColumnName())
                     select prop);

                foreach (PropertyInfo p in resList)
                {
                    result.Add(p.Name, p);
                }
            }
            return result;
        }
        /// <summary>
        ///     将数据转化为 type 类型
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="type">目标类型</param>
        /// <returns>转化为目标类型的 Object 对象</returns>
        public static object ChangeType(object value, Type type)
        {
            if (type.FullName == typeof(string).FullName)
            {
                return Convert.ChangeType(Convert.IsDBNull(value) ? null : value, type);
            }
            else if (type.FullName == typeof(Guid).FullName)
            {
                if (null == value) return Guid.Empty;
                string str = value.ToString();
                if (!string.IsNullOrEmpty(str))
                    return Guid.Parse(str);
                else
                    return Guid.Empty;
            }
            else if (type.FullName == typeof(int).FullName)
            {
                if (null == value) return 0;
                string str = value.ToString();
                if (!string.IsNullOrEmpty(str))
                    return int.Parse(str);
                else
                    return 0;
            }
            else if (type.FullName == typeof(bool).FullName)
            {
                if (null == value) return false;
                string str = value.ToString();
                if (!string.IsNullOrEmpty(str))
                    return bool.Parse(str);
                else
                    return false;
            }
            else if (type.FullName == typeof(DateTime?).FullName)
            {
                if (value.Equals(DBNull.Value)) return null;

                return (DateTime?)value;
            }
            else if (type.FullName == typeof(DateTime).FullName)
            {
                if (value.Equals(DBNull.Value)) return DateTime.MinValue;

                bool bo = DateTime.TryParse(value.ToString(), out DateTime result);

                return bo ? result : DateTime.MinValue;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter convertor = new NullableConverter(type);
                return Convert.IsDBNull(value) ? null : convertor.ConvertFrom(value);
            }
            return value;
        }
    }
}
