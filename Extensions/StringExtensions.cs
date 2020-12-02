using System;
using System.Text.RegularExpressions;
using Discord;

namespace TaricSharp.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsNullOrUri(this string url) =>
            string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute);

        public static bool IsValidHexString(this string str) =>
            new Regex(@"^#?[A-Fa-f0-9]{6}$").IsMatch(str);

        /// <summary>
        /// Creates a color based on the hex value of a string
        /// </summary>
        /// <param name="hexStr">The hex value</param>
        /// <returns>Color based on RGB value of hex string, or default if invalid</returns>
        public static Color ToColor(this string hexStr)
        {
            if (!hexStr.IsValidHexString())
            {
                return Color.Default;
            }

            if (hexStr.StartsWith("#"))
                hexStr = hexStr.Substring(1);

            return new Color(
                Convert.ToInt32(hexStr.Substring(0, 2), 16),
                Convert.ToInt32(hexStr.Substring(2, 2), 16),
                Convert.ToInt32(hexStr.Substring(4, 2), 16)
            );
        }

        /// <summary>
        /// Strips a string value from the start of a string
        /// </summary>
        /// <param name="str">The original string</param>
        /// <param name="trimStr">The text to be trimmed from start</param>
        /// <returns>String with trimStr removed</returns>
        public static string TrimStart(this string str, string trimStr)
        {
            return  str.StartsWith(trimStr) ? str.Substring(trimStr.Length) : str;
        }
        
    }
}