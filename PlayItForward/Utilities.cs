// <copyright file="Utilities.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PiF
{
    public static class Utilities
    {
        public static HttpCookie CookieToHttpCookie(Cookie cookie)
        {
            var httpCookie = new HttpCookie(cookie.Name);

            /*Copy keys and values*/
            foreach (var val in cookie.Value.Split('&').Select(value => value.Split('=')))
            {
                httpCookie.Values.Add(val[0], val[1]); /* or httpCookie[val[0]] = val[1];  */
            }

            httpCookie.Domain = cookie.Domain;
            httpCookie.Expires = cookie.Expires;
            httpCookie.HttpOnly = cookie.HttpOnly;
            httpCookie.Path = cookie.Path;
            httpCookie.Secure = cookie.Secure;

            return httpCookie;
        }

        /// <summary>Gets the SHA-2 Hash of a file.</summary>
        /// <param name="str">The string to calculate hash.</param>
        /// <returns>The SHA-2 Hash.</returns>
        public static string GetHash(string str)
        {
            var buff = new StringBuilder(10);
            using (var sha2 = new SHA256Cng())
            {
                sha2.ComputeHash(Encoding.Unicode.GetBytes(str));
                foreach (byte hashByte in sha2.Hash)
                {
                    buff.Append(string.Format(CultureInfo.InvariantCulture, "{0:X1}", hashByte));
                }
            }

            return buff.ToString();
        }

        [OutputCache(Duration = 60 * 5)]
        public static dynamic GetThreadInfo(string thingID)
        {
            string uri = string.Format("http://www.reddit.com/{0}/.json", thingID);
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;

            connect.UserAgent = "r/playitforward site by /u/sevenalive";

            // Do the actual connection
            WebResponse response = connect.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }

        public static Cookie HttpCookieToCookie(HttpCookie httpCookie)
        {
            var cookie = new Cookie
                {
                    Domain = httpCookie.Domain, 
                    Expires = httpCookie.Expires, 
                    HttpOnly = httpCookie.HttpOnly, 
                    Path = httpCookie.Path, 
                    Secure = httpCookie.Secure, 
                    Value = httpCookie.Value
                };

            return cookie;
        }

        public static string RedditError(dynamic response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            else
            {
                string error = response["errors"][0][0];
                switch (error)
                {
                    case "BAD_CAPTCHA":
                        return "Enter the captcha above.";
                    case "BAD_USERNAME":
                    case "WRONG_PASSWORD":
                        return "The username or password you entered is incorrect.";
                    case "RATELIMIT":
                        error = response["errors"][0][1];
                        error = string.Format(
                            "{0} before attempting again.", 
                            error.Replace("you are doing that too much. try again in", "You need to wait").Replace(
                                ".", string.Empty));
                        return error;
                    default:
                        return error;
                }
            }
        }

        public static HtmlString TimeAgo(this HtmlHelper helper, DateTime dateTime)
        {
            var tag = new TagBuilder("time");
            tag.AddCssClass("timeago");
            tag.Attributes.Add("datetime", dateTime.ToString("s") + "Z");
            tag.SetInnerText(dateTime.ToString());
            return new HtmlString(tag.ToString());
        }
    }
}