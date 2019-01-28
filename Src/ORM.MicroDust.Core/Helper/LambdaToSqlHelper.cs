using MicroDust.Core.LambdaResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core.Helper
{
    public sealed class LambdaToSqlHelper
    {
        private static readonly object synObject = new object();
        private static SqlServerResolver instance = null;

        public static SqlServerResolver SqlServerResolver
        {
            get
            {
                lock (synObject)
                {
                    return instance ?? (instance = new LambdaResolver.SqlServerResolver());
                }
            }
        }
    }
}
