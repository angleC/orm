using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.LambdaResolver
{
    public class SqlServerResolver : BaseExpressionResolver<SqlParameter>
    {
        private int paramIndex = 0;

        public SqlServerResolver(string prefix = "")
            : base(prefix)
        {

        }

        protected override Tuple<string, SqlParameter> GetParameter(object value)
        {
            string parameterName = $"@_{this.paramIndex.ToString()}";
            this.paramIndex++;
            return Tuple.Create(parameterName, new SqlParameter
            {
                Value = value,
                DbType = TypeUtil.ToDbType(value.GetType()),
                ParameterName = parameterName
            });
        }

        protected override DataMember ResolveConditional(ConditionalExpression exp)
        {
            return base.ResolveConditional(exp);
        }
    }
}
