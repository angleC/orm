using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : System.Attribute
    {
        public string ColumnName { get; set; }

        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
