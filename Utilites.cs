// <copyright file="Utilites.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PiF
{
    public class Utilites
    {
        public static HttpCookie CookieToHttpCookie(Cookie cookie)
        {
            var httpCookie = new HttpCookie(cookie.Name);

            /*Copy keys and values*/
            foreach (var val in cookie.Value.Split('&').Select(value => value.Split('=')))
                httpCookie.Values.Add(val[0], val[1]); /* or httpCookie[val[0]] = val[1];  */

            httpCookie.Domain = cookie.Domain;
            httpCookie.Expires = cookie.Expires;
            httpCookie.HttpOnly = cookie.HttpOnly;
            httpCookie.Path = cookie.Path;
            httpCookie.Secure = cookie.Secure;

            return httpCookie;
        }

        /// <summary>Gets the SHA-2 Hash of a file.</summary>
        /// <param name="str">The string to calulate hash.</param>
        /// <returns>The SHA-2 Hash.</returns>
        public static string GetHash(string str)
        {
            var buff = new StringBuilder(10);
            using (var sha2 = new SHA256Cng())
            {
                sha2.ComputeHash(Encoding.Unicode.GetBytes(str));
                foreach (byte hashByte in sha2.Hash)
                    buff.Append(string.Format(CultureInfo.InvariantCulture, "{0:X1}", hashByte));
            }

            return buff.ToString();
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
    }
}