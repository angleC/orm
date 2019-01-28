using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : System.Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
