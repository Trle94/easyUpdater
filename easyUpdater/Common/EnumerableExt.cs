using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyUpdater.Common
{
    public static class EnumerableExt
    {
        public static string[] TrimAll(this string[] strings)
        {
            string[] trimmedStrings = new string[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                var s = strings[i].Trim();
                trimmedStrings[i] = s;
            }

            return trimmedStrings;
        }
    }
}
