using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssignmentCore
{
    public static class StringHelper
    {
        public static bool IsEmpty(this string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }

        public static bool Eq(this string target, string s)
        {
            return target.Equals(s, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool Starts(this string target, string s)
        {
            return target.StartsWith(s, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool HasSubstring(this string target, string s)
        {
            return target.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1;
        }

        public static string[] SplitTrim(this string target, params string[] separators)
        {
            string[] fields = target.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim();
            }
            return fields;
        }

        public static string Combine(this string target, string path)
        {
            return Path.Combine(target, path);
        }
    }
}
