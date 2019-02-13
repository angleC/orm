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
        public bool IsIgnore { get; set; }

        public ColumnAttribute(bool isIgnore = false)
        {
            this.IsIgnore = isIgnore;
        }

        public ColumnAttribute(string columnName = "")
        {
            this.ColumnName = columnName;
        }

        public ColumnAttribute(string columnName = "", bool isIgnore = false)
        {
            this.ColumnName = columnName;
            this.IsIgnore = isIgnore;
        }
    }
}
