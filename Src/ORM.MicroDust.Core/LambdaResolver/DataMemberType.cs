using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public enum DataMemberType
    {
        /// <summary>
        /// 默认
        /// </summary>
        None = 0,
        /// <summary>
        /// 字段类型
        /// </summary>
        Key,
        /// <summary>
        /// 值类型
        /// </summary>
        Value
    }
}
