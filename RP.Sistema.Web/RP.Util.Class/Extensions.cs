using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Util
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static string[] NSplit(this string str, params char[] separator)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var _source = str.Trim();
                if (string.IsNullOrEmpty(_source.Trim()))
                    return new string[0];
                return str.Split(separator);
            }
            return new string[0];
        }
    }
}
