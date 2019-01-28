using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public static class DataMemberUtil
    {
        /// <summary>
        /// 将两个数据成员按照类型是否为Key类型排序
        /// </summary>
        /// <param name="member1"></param>
        /// <param name="member2"></param>
        /// <returns>item1是值类型或者非值类型</returns>
        public static Tuple<DataMember, DataMember> GetKeyMember(DataMember member1, DataMember member2)
        {
            if ((member2 != null && member2.MemberType == DataMemberType.Key) && (member1 == null || (member1.MemberType != DataMemberType.Key)))
            {
                return Tuple.Create(member2, member1);
            }
            return Tuple.Create(member1, member2);
        }
        /// <summary>
        /// 元素是否为值类型
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsValue(this DataMember member)
        {
            if (member == null)
            {
                return false;
            }
            return member.MemberType == DataMemberType.Value;
        }

        /// <summary>
        /// 判断类型是否为集合或者数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArrayOrCollection(this DataMember member)
        {
            if (member == null || member.Value == null) return false;

            return member.Value.GetType().IsArrayOrCollection();
        }
    }
}
