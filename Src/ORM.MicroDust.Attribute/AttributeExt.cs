using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Attribute
{
    public static class AttributeExt
    {
        /// <summary>
        /// 获取属性上标识的列别名称
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns></returns>
        public static string GetColumnName(this PropertyInfo propertyInfo)
        {
            string columnName = propertyInfo.Name;

            if (propertyInfo.IsDefined(typeof(ColumnAttribute), true))
            {
                ColumnAttribute column = propertyInfo.GetCustomAttributes<ColumnAttribute>().FirstOrDefault();
                if (null != column)
                {
                    columnName = column.ColumnName;
                }
            }

            return columnName;
        }
        /// <summary>
        /// 是否忽略列
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns>是否忽略</returns>
        public static bool ColumnIsIgnore(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(ColumnAttribute), true))
            {
                ColumnAttribute column = propertyInfo.GetCustomAttributes<ColumnAttribute>().FirstOrDefault();

                return column.IsIgnore;
            }

            return false;
        }
        /// <summary>
        /// 获取类上标记的表别称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            string tableName = type.Name;

            if (type.IsDefined(typeof(TableAttribute), true))
            {
                TableAttribute table = type.GetCustomAttributes<TableAttribute>().FirstOrDefault();
                if (null != table)
                {
                    tableName = table.TableName;
                }
            }

            return tableName;
        }
    }
}
