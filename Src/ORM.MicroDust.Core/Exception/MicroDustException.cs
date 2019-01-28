using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDust.Core
{
    public class MicroDustException : Exception
    {
        public MicroDustException(string message) 
            : base(message)
        {

        }
    }
}
