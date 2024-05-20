using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Ofqual.Common.RegisterFrontend.Extensions
{
    public static partial class StringExtensions
    {
        [GeneratedRegex(@"[\[\\""\]]+")]
        private static partial Regex SubStringRegex();

        public static string[]? GetSubStrings(this string? value)
        {
            if (value == null) return null;
            if (value == "null") return null;
            if (value.Length == 0) return null;
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            //remove [], quotes and spaces from the db value
            var str = SubStringRegex().Replace(value, "");

            var arr = str.Replace(", ", ",").Split(",");
            return arr;
        }

        public static bool IsNotNull(this string? value)
        {
            return value != null;
        }

        public static bool IsNotNull(this string[]? value)
        {
            return value != null;
        }

        public static string GetCompareValue(this string? value)
        {
            return value ?? "-";
        }

        public static string ToURL(this string? value)
        {
            return HttpUtility.UrlEncode(value) ?? "";
        }

        public static string Hiphenate(this string? value)
        {
            return value != null ? value.Replace(" ", "-") : "";
        }

        public static string EscapeSpaces(this string? value)
        {
            return value != null ? value.Replace(" ", string.Empty) : "";
        }

        public static string StripLeadingZeros(this string? value)
        {
            return value != null ? value.StartsWith('0') ? value.Remove(0, 1) : value : "";
        }
    }
}
