using System;
using MicroDust.Core;

namespace ORM.MicroDust.Test.Console
{
    public class ORMDbContext : DbContext
    {
        private static readonly object synObject = new object();
        private static ORMDbContext instance = null;

        public static ORMDbContext Singleton
        {
            get
            {
                lock (synObject)
                {
                    return instance ?? (instance = new ORMDbContext());
                }
            }
        }

        public ORMDbContext()
            : base("OwlDbContext")
        {

        }
    }
}
