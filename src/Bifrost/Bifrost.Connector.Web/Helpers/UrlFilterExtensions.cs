using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bifrost.Core.Connectors.Configs.Web;

namespace Bifrost.Connector.Web.Helpers
{
    public static class UrlFilterExtensions
    {
        public static bool IsValid(this LinkFilter filter, Uri url, ref string excludeReason)
        {
            var isValid = !filter.ExcludeAsDefault;

            if (filter.ExcludePagesWithQueryParameters && !string.IsNullOrEmpty(url.Query))
            {
                excludeReason = $"ExcludePagesWithQueryParameters is enabled";
                return false;
            }
            if (filter.ExcludeExtensions.Any())
            {
                var extension= url.GetExtension();
                foreach (var excludeExtension in filter.ExcludeExtensions.Where(excludeExtension => excludeExtension == extension))
                {
                    excludeReason = $"Extension {excludeExtension} is configured to be exluced";
                    return false;
                }
            }
            foreach (var regex in filter.ExcludeRegex)
            {
                if (!Matches(url, regex)) continue;
                excludeReason = $"Matches exclude regex {regex}";
                return false;
            }
            if (filter.IncludeRegex.Any(regex => Matches(url, regex)))
            {
                return true;
            }
            if (isValid == false)
            {
                excludeReason = "Matches no include regex and ExcludeAsDefault is enabled";
            }
            return isValid;
        }

        private static bool Matches(Uri url, string regex)
        {
            return Regex.IsMatch(url.ToString(), regex);
        }
    }
}