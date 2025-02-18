using Microsoft.Extensions.Localization;
using System.Globalization;

namespace TestUI.Utils
{
    public static class AppExtentions
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static List<T> AsNotNull<T>(this List<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }

        public static string RemoveController(this string str)
        {
            return str.Replace("Controller", "");
        }

        public static string ToLocalizedString<T>(this T enume, IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(enume.GetType());
            return localizer[enume.ToString()].Value;
        }

        public static string ToPersianLongDateTimeString(this DateTime dateTime)
        {
            var pc = new PersianCalendar();
            return pc.GetYear(dateTime) + "/" + pc.GetMonth(dateTime) + "/" + pc.GetDayOfMonth(dateTime) + " " +
                pc.GetHour(dateTime) + ":" + pc.GetMinute(dateTime) + ":" + pc.GetSecond(dateTime);
        }

        public static void SaveFile(this IFormFile file,string path)
        {
            var outStream = System.IO.File.OpenWrite(path);
            file.CopyTo(outStream);
            outStream.Close();
        }

        public static List<object> ToList(this Array array)
        {
            List<object> list = new List<object>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
