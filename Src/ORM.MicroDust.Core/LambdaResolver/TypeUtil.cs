using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public static class TypeUtil
    {
        private static Dictionary<RuntimeTypeHandle, DbType> typeMap;

        static TypeUtil()
        {
            typeMap = new Dictionary<RuntimeTypeHandle, DbType>
            {
                [typeof(byte).TypeHandle] = DbType.Byte,
                [typeof(sbyte).TypeHandle] = DbType.SByte,
                [typeof(short).TypeHandle] = DbType.Int16,
                [typeof(ushort).TypeHandle] = DbType.UInt16,
                [typeof(int).TypeHandle] = DbType.Int32,
                [typeof(uint).TypeHandle] = DbType.UInt32,
                [typeof(long).TypeHandle] = DbType.Int64,
                [typeof(ulong).TypeHandle] = DbType.UInt64,
                [typeof(float).TypeHandle] = DbType.Single,
                [typeof(double).TypeHandle] = DbType.Double,
                [typeof(decimal).TypeHandle] = DbType.Decimal,
                [typeof(bool).TypeHandle] = DbType.Boolean,
                [typeof(string).TypeHandle] = DbType.String,
                [typeof(char).TypeHandle] = DbType.StringFixedLength,
                [typeof(Guid).TypeHandle] = DbType.Guid,
                [typeof(DateTime).TypeHandle] = DbType.DateTime,
                [typeof(DateTimeOffset).TypeHandle] = DbType.DateTimeOffset,
                [typeof(TimeSpan).TypeHandle] = DbType.Time,
                [typeof(byte[]).TypeHandle] = DbType.Binary,
                [typeof(byte?).TypeHandle] = DbType.Byte,
                [typeof(sbyte?).TypeHandle] = DbType.SByte,
                [typeof(short?).TypeHandle] = DbType.Int16,
                [typeof(ushort?).TypeHandle] = DbType.UInt16,
                [typeof(int?).TypeHandle] = DbType.Int32,
                [typeof(uint?).TypeHandle] = DbType.UInt32,
                [typeof(long?).TypeHandle] = DbType.Int64,
                [typeof(ulong?).TypeHandle] = DbType.UInt64,
                [typeof(float?).TypeHandle] = DbType.Single,
                [typeof(double?).TypeHandle] = DbType.Double,
                [typeof(decimal?).TypeHandle] = DbType.Decimal,
                [typeof(bool?).TypeHandle] = DbType.Boolean,
                [typeof(char?).TypeHandle] = DbType.StringFixedLength,
                [typeof(Guid?).TypeHandle] = DbType.Guid,
                [typeof(DateTime?).TypeHandle] = DbType.DateTime,
                [typeof(DateTimeOffset?).TypeHandle] = DbType.DateTimeOffset,
                [typeof(TimeSpan?).TypeHandle] = DbType.Time,
                [typeof(object).TypeHandle] = DbType.Object
            };
        }
        /// <summary>
        /// 将类型转成DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(this Type type)
        {
            if (typeMap.ContainsKey(type.TypeHandle))
            {
                return typeMap[type.TypeHandle];
            }

            return DbType.Object;
        }
        /// <summary>
        /// 是否为集合或数组类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArrayOrCollection(this Type type)
        {
            if (null == type) return false;

            return type.IsArray || type.IsGenericType;
        }
    }
}
