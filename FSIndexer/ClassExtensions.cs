using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FSIndexer
{
    public static class ClassExtensions
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(oldValue))
                return str;

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);

                if (sb.Length > 255)
                {
                    return str;
                }
            }

            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static bool Contains(this string source, string toCheck)
        {
            return source.Contains(toCheck, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool Contains(this List<string> list, string item, StringComparison comparison)
        {
            foreach (var i in list)
            {
                if (i.Equals(item, comparison))
                    return true;
            }

            return false;
        }

        public static bool FileExists(this FileInfo fi)
        {
            return File.Exists(fi.FullName);
        }

        public static bool DirExists(this DirectoryInfo di)
        {
            return Directory.Exists(di.FullName);
        }
    }
}
