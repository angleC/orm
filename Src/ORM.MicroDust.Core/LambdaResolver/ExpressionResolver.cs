using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public abstract class ExpressionResolver
    {
        protected ExpressionResolver()
        {
        }

        protected abstract DataMember ResolveUnary(UnaryExpression exp);

        protected abstract DataMember ResolveBinary(BinaryExpression exp);

        protected abstract DataMember ResolveTypeBinary(TypeBinaryExpression exp);

        protected abstract DataMember ResolveConditional(ConditionalExpression exp);

        protected abstract DataMember ResolveConstant(ConstantExpression exp);

        protected abstract DataMember ResolveParameter(ParameterExpression exp);

        protected abstract DataMember ResolveMember(MemberExpression exp);

        protected abstract DataMember ResolveMethodCall(MethodCallExpression exp);

        protected abstract DataMember ResolveLambda(LambdaExpression exp);

        protected abstract DataMember ResolveNew(NewExpression exp);

        protected abstract DataMember ResolveNewArray(NewArrayExpression exp);

        protected abstract DataMember ResolveInvocation(InvocationExpression exp);

        protected abstract DataMember ResolveMemberInit(MemberInitExpression exp);

        protected abstract DataMember ResolveListInit(ListInitExpression exp);
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public DataMember Resolve(Expression exp)
        {
            DataMember dataMember = null;
            if (exp == null)
                return dataMember;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    dataMember = this.ResolveUnary((UnaryExpression)exp);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    dataMember = this.ResolveBinary((BinaryExpression)exp);
                    break;
                case ExpressionType.TypeIs:
                    dataMember = this.ResolveTypeBinary((TypeBinaryExpression)exp);
                    break;
                case ExpressionType.Conditional:
                    dataMember = this.ResolveConditional((ConditionalExpression)exp);
                    break;
                case ExpressionType.Constant:
                    dataMember = this.ResolveConstant((ConstantExpression)exp);
                    break;
                case ExpressionType.Parameter:
                    dataMember = this.ResolveParameter((ParameterExpression)exp);
                    break;
                case ExpressionType.MemberAccess:
                    dataMember = this.ResolveMember((MemberExpression)exp);
                    break;
                case ExpressionType.Call:
                    dataMember = this.ResolveMethodCall((MethodCallExpression)exp);
                    break;
                case ExpressionType.Lambda:
                    dataMember = this.ResolveLambda((LambdaExpression)exp);
                    break;
                case ExpressionType.New:
                    dataMember = this.ResolveNew((NewExpression)exp);
                    break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    dataMember = this.ResolveNewArray((NewArrayExpression)exp);
                    break;
                case ExpressionType.Invoke:
                    dataMember = this.ResolveInvocation((InvocationExpression)exp);
                    break;
                case ExpressionType.MemberInit:
                    dataMember = this.ResolveMemberInit((MemberInitExpression)exp);
                    break;
                case ExpressionType.ListInit:
                    dataMember = this.ResolveListInit((ListInitExpression)exp);
                    break;
                default:
                    throw new NotSupportedException(exp.NodeType.ToString());
            }

            return dataMember;
        }
    }
}
