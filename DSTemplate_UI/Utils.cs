using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace DSTemplate_UI
{
    internal static class Utils
    {
        public static List<object> ToList(this Array array)
        {
            var list = new List<object>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            return list;
        }

        public static string ToLocalizedString<T>(this T obj, IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(obj.GetType());
            return localizer[obj.ToString()].Value;
        }
    }
}
