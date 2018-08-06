using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriFormat
{
    static class Helper
    {
        /// <summary>
        /// Convert chars to string
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string ToRealString(this char[] chars)
        {
            return new string(chars);
        }
    }
}
