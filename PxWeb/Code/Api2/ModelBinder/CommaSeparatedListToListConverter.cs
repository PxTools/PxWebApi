using System.Text.RegularExpressions;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class CommaSeparatedListToListConverter
    {
        public static List<T> ToList<T>(string values, Func<string, T> converter)
        {
            var list = new List<T>();
            var items = Regex.Split(values, ",(?=[^\\]]*(?:\\[|$))", RegexOptions.None,
                    TimeSpan.FromMilliseconds(100));

            foreach (var item in items)
            {
                var cleanItem = CleanValue(item);
                if (!string.IsNullOrWhiteSpace(cleanItem))
                {
                    list.Add(converter(cleanItem));
                }
            }

            return list;
        }

        private static string CleanValue(string value)
        {
            var item2 = value.Trim();
            if (item2.StartsWith('[') && item2.EndsWith(']'))
            {
                return value.Substring(1, item2.Length - 2).Trim();
            }
            return item2;

        }
    }
}
