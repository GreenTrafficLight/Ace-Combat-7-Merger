using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace7Localization.Utils
{
    public static class StringUtils
    {
        public static int GetCommonSubstringIndex(string str1, string str2, int startIndex = 0)
        {
            int index = -1;

            for (int i = startIndex; i < Math.Min(str1.Length, str2.Length); i++)
            {
                if (str1[i] != str2[i])
                {
                    return index;
                }
                index++;
            }
            return index;
        }
    }
}
