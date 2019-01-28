using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public abstract class BaseExpressionResolver<T> : ExpressionResolver where T : class
    {
        private List<T> paramList = new List<T>();
        private readonly string prefix = null;

        public BaseExpressionResolver(string prefix = "")
        {
            this.prefix = prefix;
        }
        /// <summary>
        /// 将表达式转为sql字符串及参数值集
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Tuple<string, List<T>> ToSql(Expression exp)
        {
            DataMember dataMember = this.Resolve(exp);

            return Tuple.Create(dataMember.Name, this.paramList);
        }
        /// <summary>
        /// 获取参数对应值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract Tuple<string, T> GetParameter(object value);

        #region virtual method
        /// <summary>
        /// 拼接左边与右边参数
        /// </summary>
        /// <param name="left">左边参数</param>
        /// <param name="expType">操作符</param>
        /// <param name="right">右边参数</param>
        /// <returns></returns>
        protected virtual DataMember Append(DataMember left, ExpressionType expType, DataMember right)
        {
            if (null == left && null == right)
            {
                return null;
            }
            Tuple<DataMember, DataMember> dataMembers = DataMemberUtil.GetKeyMember(left, right);
            DataMember memberKey = ToParamMember(dataMembers.Item1);
            DataMember memberValue = ToParamMember(dataMembers.Item2);

            this.AddParam(memberKey, memberValue);

            if (expType == ExpressionType.Coalesce)
            {
                return new DataMember
                {
                    Name = $"ISNULL ({memberKey.Name},{memberValue.Name})",
                    Value = null,
                    MemberType = DataMemberType.None
                };
            }

            return new DataMember
            {
                Name = $"({memberKey?.Name} {ResolveExpressionType(expType)} {memberValue?.Name})",
                Value = null,
                MemberType = DataMemberType.None
            };
        }
        /// <summary>
        /// 解析运算符
        /// </summary>
        /// <param name="expType"></param>
        /// <returns></returns>
        protected virtual string ResolveExpressionType(ExpressionType expType)
        {
            string operateSign = string.Empty;
            switch (expType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.AddChecked:
                    operateSign = "+";
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.AndAssign:
                    operateSign = "AND";
                    break;
                case ExpressionType.LessThan:
                    operateSign = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    operateSign = "<=";
                    break;

                case ExpressionType.Equal:
                    operateSign = "=";
                    break;
                case ExpressionType.NotEqual:
                    operateSign = "!=";
                    break;
                case ExpressionType.Not:
                    operateSign = "NOT";
                    break;
                case ExpressionType.GreaterThan:
                    operateSign = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operateSign = ">=";
                    break;
                case ExpressionType.OrElse:
                    operateSign = "OR";
                    break;
            }

            return operateSign;
        }
        /// <summary>
        /// 处理条件方法
        /// </summary>
        /// <param name="conditionMember"></param>
        /// <param name="ifTrueMember"></param>
        /// <param name="ifFalseMember"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveConditionalMethod(DataMember conditionMember, DataMember ifTrueMember, DataMember ifFalseMember)
        {
            if (conditionMember != null)
            {
                StringBuilder nameBuilder = new StringBuilder();
                nameBuilder.Append("(CASE WHEN ");
                if (conditionMember.MemberType == DataMemberType.None)
                {
                    nameBuilder.Append(conditionMember?.Name);
                }
                else
                {
                    nameBuilder.AppendFormat("{0}=1", conditionMember?.Name);
                }
                nameBuilder.AppendFormat(" THEN {0} ELSE {1})", ifTrueMember?.Name, ifFalseMember?.Name);
                AddParam(conditionMember, ifTrueMember, ifFalseMember);
                return new DataMember(nameBuilder.ToString(), DataMemberType.Key);
            }
            return null;
        }
        /// <summary>
        /// 解析 StartsWith
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodStartsWith(MethodCallExpression exp)
        {
            StringBuilder nameBuilder = new StringBuilder();
            if (exp.Object != null)
            {
                DataMember dataMember = this.ToParamMember(Resolve(exp.Object));
                nameBuilder.Append(dataMember.Name);
                this.AddParam(dataMember);
            }
            nameBuilder.Append(" LIKE '%'+ ");
            if (exp.Arguments != null)
            {
                foreach (var item in exp.Arguments)
                {
                    DataMember dataMember = this.ToParamMember(Resolve(item));
                    nameBuilder.Append(dataMember.Name);
                    this.AddParam(dataMember);
                }
            }

            return new DataMember(nameBuilder.ToString(), DataMemberType.None);
        }
        /// <summary>
        /// 解析 Contains
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodContains(MethodCallExpression exp)
        {
            var rightExpType = exp.Arguments[0].NodeType;
            Expression leftExp = exp.Object ?? exp.Arguments[1];
            Expression rightExp = null;
            if (new ExpressionType[] { ExpressionType.MemberAccess, ExpressionType.Constant }.Contains(rightExpType))
            {
                rightExp = exp.Arguments[0];
            }
            else
            {
                rightExp = Expression.Lambda(exp.Arguments[0]);
            }
            StringBuilder nameBuilder = new StringBuilder();
            DataMember leftSqlMember = this.ToParamMember(Resolve(leftExp));
            DataMember rightSqlMEember = this.ToParamMember(Resolve(rightExp));
            if (leftSqlMember.IsArrayOrCollection())//是个数组或者集合
            {
                nameBuilder.AppendFormat("{0} IN {1}", rightSqlMEember.Name, leftSqlMember.Name);
            }
            else if (rightSqlMEember.IsArrayOrCollection())
            {
                nameBuilder.AppendFormat("{0} IN {1}", leftSqlMember.Name, rightSqlMEember.Name);
            }
            else
            {
                nameBuilder.AppendFormat(" {0} LIKE '%'+{1}+'%' ", leftSqlMember.Name, rightSqlMEember.Name);
            }
            this.AddParam(leftSqlMember, rightSqlMEember);

            return new DataMember(nameBuilder.ToString(), DataMemberType.None);
        }
        /// <summary>
        /// 解析 Trim
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodTrim(MethodCallExpression exp)
        {
            StringBuilder nameBuilder = new StringBuilder("RTRIM(LTRIM(");
            List<T> paramList = new List<T>();
            if (exp.Object != null)
            {
                DataMember dataMember = this.ToParamMember(Resolve(exp.Object));
                nameBuilder.Append(dataMember.Name);
                this.AddParam(dataMember);
            }
            nameBuilder.Append(")");

            return new DataMember(nameBuilder.ToString(),DataMemberType.Key);

        }
        /// <summary>
        /// 解析 EndsWith
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodEndsWith(MethodCallExpression exp)
        {
            StringBuilder nameBuilder = new StringBuilder();
            if (exp.Object != null)
            {
                DataMember dataMember = this.ToParamMember(Resolve(exp.Object));
                nameBuilder.Append(dataMember.Name);
                this.AddParam(dataMember);
            }
            nameBuilder.Append(" LIKE ");
            if (exp.Arguments != null)
            {
                foreach (var item in exp.Arguments)
                {
                    DataMember dataMember = this.ToParamMember(Resolve(item));
                    nameBuilder.Append(dataMember.Name);
                    this.AddParam(dataMember);
                }
            }
            nameBuilder.Append(" +'%' ");

            return new DataMember(nameBuilder.ToString(), DataMemberType.None);
        }
        /// <summary>
        /// 解析 TrimEnd
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodTrimEnd(MethodCallExpression exp)
        {
            StringBuilder nameBuilder = new StringBuilder("RTRIM(");
            if (exp.Object != null)
            {
                DataMember dataMember = this.ToParamMember(Resolve(exp.Object));
                nameBuilder.Append(dataMember.Name);
                this.AddParam(dataMember);
            }
            nameBuilder.Append(")");

            return new DataMember(nameBuilder.ToString(), DataMemberType.Key);
        }
        /// <summary>
        /// 解析 TrimStart
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual DataMember ResolveMethodTrimStart(MethodCallExpression exp)
        {
            StringBuilder nameBuilder = new StringBuilder("LTRIM(");
            if (exp.Object != null)
            {
                DataMember dataMember = this.ToParamMember(Resolve(exp.Object));
                nameBuilder.Append(dataMember.Name);
                this.AddParam(dataMember);
            }
            nameBuilder.Append("))");

            return new DataMember(nameBuilder.ToString(), DataMemberType.Key);
        }
        #endregion

        private void AddParam(params DataMember[] members)
        {
            if (members != null)
            {
                foreach (var item in members)
                {
                    if (item.IsValue())
                    {
                        if (item.Value is T)
                        {
                            this.paramList.Add(item.Value as T);
                        }
                    }
                }
            }
        }

        private DataMember ToParamMember(DataMember dataMember)
        {
            if (null == dataMember || dataMember.MemberType != DataMemberType.Value)
            {
                return dataMember;
            }

            Tuple<string, T> parameter = this.GetParameter(dataMember.Value);
            dataMember.Name = parameter.Item1;
            dataMember.Value = parameter.Item2;

            return dataMember;
        }

        #region override method
        protected override DataMember ResolveBinary(BinaryExpression exp)
        {
            if (exp.NodeType == ExpressionType.ArrayIndex)
            {
                return this.Resolve(exp.ToConstantExpression());
            }

            Expression expLeft = exp.Left;
            Expression expRight = exp.Right;

            if (exp.NodeType == ExpressionType.Coalesce)//?? 解析
            {
                if (expLeft.IsConstantExpression()
                    && expRight.IsConstantExpression())
                {
                    return Resolve(exp.ToConstantExpression());
                }
            }

            DataMember left = this.Resolve(expLeft);
            DataMember right = this.Resolve(expRight);

            return this.Append(left, exp.NodeType, right);
        }

        protected override DataMember ResolveConditional(ConditionalExpression exp)
        {
            try
            {
                var lambda = Expression.Lambda(exp.Test).Compile().DynamicInvoke();
                if (null != lambda && bool.TryParse(lambda.ToString(), out bool isTrue))
                {
                    return this.Resolve(exp.IfTrue);
                }

                return this.Resolve(exp.IfFalse);
            }
            catch
            {
                DataMember member = this.Resolve(exp.Test);
                DataMember ifTrueMember = this.ToParamMember(this.Resolve(exp.IfTrue));
                DataMember ifFalseMember = this.ToParamMember(this.Resolve(exp.IfFalse));

                return this.ResolveConditionalMethod(member, ifTrueMember, ifFalseMember);
            }
        }

        protected override DataMember ResolveConstant(ConstantExpression exp)
        {
            object value = exp.Value;
            if (exp.Type == typeof(bool))
            {
                if (exp.Value != null)
                {
                    if (bool.TryParse(exp.Value.ToString(), out bool isTrue))
                    {
                        value = "1";
                    }
                    else
                    {
                        value = "0";
                    }
                }
            }

            return new DataMember(value, DataMemberType.Value);
        }

        protected override DataMember ResolveInvocation(InvocationExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveLambda(LambdaExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveListInit(ListInitExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveMember(MemberExpression exp)
        {
            if (exp.IsNotParameterExpression())
            {
                object memberValue = Expression.Lambda(exp).Compile().DynamicInvoke();
                Type memberValueType = memberValue.GetType();
                if (memberValueType.IsArrayOrCollection())
                {
                    List<Tuple<string, T>> tupleList = new List<Tuple<string, T>>();

                    foreach (var item in (IEnumerable)memberValue)
                    {
                        tupleList.Add(this.GetParameter(item));
                    }
                    string memberName = string.Concat("(", string.Join(",",
                        tupleList.Select(m => m.Item1)), ")");

                    this.paramList.AddRange(tupleList.Select(m => m.Item2));

                    return new DataMember(memberName, memberValue, DataMemberType.None);
                }
                else
                {
                    return this.Resolve(Expression.Constant(memberValue));
                }
            }
            else
            {
                if (exp.Member.Name == "Length")
                {
                    DataMember dataMember = this.Resolve(exp.Expression);
                    var memberName = $"LEN({dataMember.Name})";

                    return new DataMember(memberName, DataMemberType.Key);
                }
                if (exp.Member.Name == "Count")
                {
                    throw new NotSupportedException("Count");
                }
                else
                {
                    var memberName = string.Concat(this.prefix, exp.Member.Name);

                    return new DataMember(memberName, DataMemberType.Key);
                }
            }
        }

        protected override DataMember ResolveMemberInit(MemberInitExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveMethodCall(MethodCallExpression exp)
        {
            try
            {
                return Resolve(exp.ToConstantExpression());
            }
            catch
            {
                string methodName = exp.Method.Name;
                switch (methodName)
                {
                    case "StartsWith":
                        return ResolveMethodStartsWith(exp);
                    case "Contains":
                        return ResolveMethodContains(exp);
                    case "EndsWith":
                        return ResolveMethodEndsWith(exp);
                    case "TrimEnd":
                        return ResolveMethodTrimEnd(exp);
                    case "TrimStart":
                        return ResolveMethodTrimStart(exp);
                    case "Trim":
                        return ResolveMethodTrim(exp);
                    default:
                        throw new NotSupportedException(methodName);
                }
            }
        }

        protected override DataMember ResolveNew(NewExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveNewArray(NewArrayExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveParameter(ParameterExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveTypeBinary(TypeBinaryExpression exp)
        {
            throw new NotImplementedException();
        }

        protected override DataMember ResolveUnary(UnaryExpression exp)
        {
            var memberValue = Expression.Lambda(exp).Compile().DynamicInvoke();

            return this.Resolve(Expression.Constant(memberValue));
        }
        #endregion
    }
}
