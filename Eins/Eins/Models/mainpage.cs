using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eins.Models
{
    public class mainpage
    {

        public static bool IsDebug()
        {
#if (DEBUG)
            return true;
#else
            return false;
#endif
        }
    }
}
