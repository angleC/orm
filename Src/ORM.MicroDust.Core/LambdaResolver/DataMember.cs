using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public sealed class DataMember
    {
        #region Constructor
        public DataMember()
        {

        }

        public DataMember(object value, DataMemberType type)
            : this(string.Empty, value, type)
        {

        }

        public DataMember(string name, DataMemberType type)
            : this(name, null, type)
        {

        }

        public DataMember(string name, object value, DataMemberType type)
        {
            this.Name = name;
            this.Value = value;
            this.MemberType = type;
        }
        #endregion

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 数据元素类型
        /// </summary>
        public DataMemberType MemberType { get; set; }
    }
}
