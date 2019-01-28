using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORM.MicroDust.Test.Console.FuncTest
{
    public class FuncSimple
    {
        public static Func<int, int, int> FuncMath = (param1, param2) =>
         {
             return param1 + param2;
         };

        public static int FuncMath1<T>(Func<T, int> func, int value) where T : class
        {
            T obj = default(T);
            Type t = typeof(T);
            Assembly ass = t.Assembly;

            obj = ass.CreateInstance(t.FullName) as T;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                pi.SetValue(obj, value);
            }

            return func.Invoke(obj);
        }

        public static int FuncMath2<T>(Func<T, int> func, T t) where T : class
        {
            return func(t);
        }

        public static T FuncMath3<T>(Action<T> func) where T : class
        {
            T obj = default(T);
            Type t = typeof(T);
            Assembly ass = t.Assembly;

            obj = ass.CreateInstance(t.FullName) as T;

            func.Invoke(obj);

            return obj;
        }

        public static string FuncMath4<T>(Func<T, object> func) where T : class
        {
            T obj = default(T);
            Type t = typeof(T);
            Assembly ass = t.Assembly;

            obj = ass.CreateInstance(t.FullName) as T;

            object objResult = func.Invoke(obj);


            return string.Empty;
        }

        public static string FuncGetPropertyName<TSource, TKey>(Expression<Func<TSource, TKey>> expression)
        {
            var member = ((MemberExpression)expression.Body).Member;

            return member.Name;
        }
    }
}
