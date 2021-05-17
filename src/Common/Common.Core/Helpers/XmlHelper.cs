using System.Text.RegularExpressions;

namespace Bifrost.Common.Core.Helpers
{
    /// <summary>
    /// Helper class for handling xml data
    /// </summary>
    public static class XmlHelper
    {
        private static readonly Regex HexadecRegex = new Regex("[\x00-\x08\x0B\x0C\x0E-\x1F]", RegexOptions.Compiled);
        public static string ReplaceHexadecimalSymbols(string txt)
        {
            return HexadecRegex.Replace(txt, "");
        }

        private static readonly Regex ElementHexadecRegex = new Regex("[\x00-\x2f\xA4-\xBF]", RegexOptions.Compiled);

        public static string ReplaceInvalidElementNameChars(string elementName)
        {
            return ElementHexadecRegex.Replace(elementName, "");
        }

    }
}