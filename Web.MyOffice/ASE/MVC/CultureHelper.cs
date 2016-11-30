using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    public static class CultureHelper
    {
        public static readonly Dictionary<string, string> Cultures = new Dictionary<string, string> {
            { "ru-RU", "Русский" },
            { "uk-UA", "Українська" },
            { "en-US", "English" },
        };

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </summary>
        /// <param name="name">Culture's name (e.g. en-US)</param>
        public static string GetValidCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return GetDefaultCulture(); // return Default culture

            if (Cultures.Keys.Contains(name))
                return name;

            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (ie English)            
            foreach (var c in Cultures.Keys)
                if (c.StartsWith(name.Substring(0, 2)))
                    return c;

            return GetDefaultCulture(); // return Default culture as no match found
        }

        public static string GetDefaultCulture()
        {
            return Cultures.Keys.ElementAt(0); // return Default culture

        }

        public static string GetCultureNameFromCookies(HttpRequestBase request)
        {
            string cul = GetCultureFromCookies(request);
            return Cultures[cul];
        }

        public static string GetCultureFromCookies(HttpRequestBase request)
        {
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = request.Cookies["_culture"];
            if (cultureCookie != null)
            {
                cultureName = cultureCookie.Value;
            }
            else if (request.UserLanguages != null)
            {
                cultureName = request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages
            }

            // Validate culture name
            return GetValidCulture(cultureName); // This is safe
        }

        public static string AcceptLanguage()
        {
            return HttpUtility.HtmlAttributeEncode(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
        }

        public static IHtmlString MetaAcceptLanguage<T>(this HtmlHelper<T> html)
        {
            return new HtmlString(String.Format(@"<meta name=""accept-language"" content=""{0}"" />", AcceptLanguage()));
        }

        public static IHtmlString GlobalizationLink<T>(this HtmlHelper<T> html)
        {
            return new HtmlString(String.Format(@"<script src=""../../Scripts/globalization/cultures/globalize.culture.{0}.js"" type=""text/javascript""></script>",
                AcceptLanguage()));
        }
    }
}