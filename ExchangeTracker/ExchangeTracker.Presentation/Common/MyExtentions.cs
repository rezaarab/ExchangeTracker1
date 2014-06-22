using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeTracker.Presentation.Common
{
    public static class MyExtentions
    {
        public static T ToType<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default (T);
            try
            {
                return (T)Convert.ChangeType(str, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static void SetStringTo<T>(ref T field, string str)
        {
            field = str.ToType<T>();
        }

        public static T GetSafe<T>(this IList<T> array, int index)
        {
            if (array != null && array.Count > index)
                return array[index];
            return default(T);
        }

    }
}
