using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BSP.DynamicsGP.PowerHouse.Extensions
{
    public static class StringExtension
    {
        public static IEnumerable<string> Split(this string str, int chunkSize) => Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        public static string[] Wrap(this string text, int max)
        {
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
        public static string RemoveSpecialCharacters(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            return new String(text.Where(c => !Char.IsPunctuation(c)).ToArray());
        }

        /// <summary>
        /// Replace reserve XML characters with the valid character
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SanitizeXMLString(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            //removed temporarily
            //return System.Security.SecurityElement.Escape(text);
            return new String(text.Where(c => !Char.IsControl(c)).ToArray());
        }
    }
}
