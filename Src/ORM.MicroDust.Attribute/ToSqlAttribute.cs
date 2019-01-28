using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Attribute
{
    public class ToSqlAttribute :System.Attribute
    {
        public string Format { get; set; }

        public ToSqlAttribute(string str)
        {
            Format = str;
        }
    }
}
