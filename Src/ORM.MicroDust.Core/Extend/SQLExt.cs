using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    public static class SQLExt
    {
        /// <summary>
        /// 指定字段升序排序
        /// </summary>
        /// <typeparam name="TSource">类型</typeparam>
        /// <typeparam name="TKey">排序字段</typeparam>
        /// <param name="sql"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SQL<TSource> OrderBy<TSource, TKey>(this SQL<TSource> sql, Expression<Func<TSource, TKey>> expression, OrderStatus orderStatus = OrderStatus.DEFAULT) where TSource : class
        {
            sql.SQLSentence.Append($" ORDER BY { GetOrderByString(expression, orderStatus)}");

            return sql;
        }
        /// <summary>
        /// 然后再升序排序字段
        /// </summary>
        /// <typeparam name="TSource">类型</typeparam>
        /// <typeparam name="TKey">排序字段</typeparam>
        /// <param name="sql"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SQL<TSource> ThenBy<TSource, TKey>(this SQL<TSource> sql, Expression<Func<TSource, TKey>> expression, OrderStatus orderStatus = OrderStatus.DEFAULT) where TSource : class
        {
            sql.SQLSentence.Append($" ,{ GetOrderByString(expression, orderStatus)}");

            return sql;
        }
        /// <summary>
        /// 取的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rowCount">行数</param>
        /// <returns></returns>
        public static SQL<TSource> Top<TSource>(this SQL<TSource> sql, int rowCount) where TSource : class
        {
            sql.SQLSentence.Insert(sql.SQLSentence.ToString().IndexOf("SELECT") + 6, $" TOP {rowCount}");

            return sql;
        }
        /// <summary>
        /// 获取排序字段内容
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        private static string GetOrderByString<TSource, TKey>(Expression<Func<TSource, TKey>> expression, OrderStatus orderStatus) where TSource : class
        {
            var member = ((MemberExpression)expression.Body).Member;
            string orderBy = string.Empty;

            switch (orderStatus)
            {
                case OrderStatus.DEFAULT:
                case OrderStatus.ASC:
                    orderBy = "ASC";
                    break;
                case OrderStatus.DESC:
                    orderBy = "DESC";
                    break;
                default:
                    throw new ArgumentException("orderStatus 无效");
            }

            return $" {member.Name} {orderBy}";
        }
    }
}
