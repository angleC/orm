using MicroDust.Attribute;

namespace MicroDust.Core
{
    public static class ToSqlExt
    {
        [ToSql("{0} IN ({1})")]
        public static bool In<T>(this T obj, T[] array)
        {
            return true;
        }
        [ToSql("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T obj, T[] array)
        {
            return true;
        }
        [ToSql("{0} LIKE {1}")]
        public static bool Like(this string str, string likeStr)
        {
            return true;
        }
        [ToSql("{0} NOT LIKE {1}")]
        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
    }
}
