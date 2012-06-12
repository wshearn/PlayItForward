// <copyright file="AccountController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using PiF.Models;

namespace PiF.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult LogOff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            HttpCookie httpCookie = Request.Cookies.Get("RedditCookie");
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1d);
            }

            httpCookie = Request.Cookies.Get("Username");
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1d);
            }

            httpCookie = Request.Cookies.Get("ModHash");
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1d);
            }

            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            ViewBag.Title = "Sign in";
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                dynamic response = Login(model.UserName, model.Password);

                if (response["json"]["errors"].Length > 0)
                {
                    string error = response["json"]["errors"][0][0];
                    if (error == "INVALID_PASSWORD" || error == "WRONG_PASSWORD" || error == "RATELIMIT")
                    {
                        ModelState.AddModelError(string.Empty, response["json"]["errors"][0][1]);
                    }
                }
                else
                {
                    // Set Session vars in case user doesn't use cookies.
                    Session["Username"] = model.UserName;

                    // Session is required reguardless of cookie state as Auth cookie won't be sent until next request
                    if (AccountHelper.CurrentUser == null)
                    {
                        // Referencing the helper will add the user and/or update the IP if necessary
                        throw new Exception("Account error!");
                    }

                    Session["ModHash"] = response["json"]["data"]["modhash"];

                    var redditCookie = new HttpCookie("reddit_session")
                        {
                            Value = Server.UrlEncode(response["json"]["data"]["cookie"]), 
                            Expires = DateTime.Now.AddYears(1)
                        };
                    Session["RedditCookie"] = redditCookie;

                    if (model.RememberMe)
                    {
                        Response.Cookies["Username"].Value = model.UserName;
                        Response.Cookies["Username"].Expires = DateTime.Now.AddYears(1);
                        Response.Cookies["RedditCookie"].Value = Server.UrlEncode(response["json"]["data"]["cookie"]);
                        Response.Cookies["RedditCookie"].Expires = DateTime.Now.AddYears(1);
                        Response.Cookies["ModHash"].Value = response["json"]["data"]["modhash"];
                        Response.Cookies["ModHash"].Expires = DateTime.Now.AddYears(1);
                    }

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Me()
        {
            ViewBag.Title = User.Identity.Name + "'s PiFs";
            return View(AccountHelper.CurrentUser.Threads.OrderByDescending(t => t.CreatedDate));
        }

        /// <summary>Logs the user in</summary>
        /// <param name="username">Reddit account username</param>
        /// <param name="password">Reddit account password</param>
        /// <returns>True/False depending on success of login (NYI)</returns>
        dynamic Login(string username, string password)
        {
            var login = WebRequest.Create("https://ssl.reddit.com/api/login/" + username) as HttpWebRequest;
            login.CookieContainer = new CookieContainer();
            login.Method = "POST";
            login.ContentType = "application/x-www-form-urlencoded";

            string postData = string.Format("api_type=json&user={0}&passwd={1}", username, password);
            byte[] dataBytes = Encoding.ASCII.GetBytes(postData);
            login.ContentLength = dataBytes.Length;
            Stream postStream = login.GetRequestStream();

            postStream.Write(dataBytes, 0, dataBytes.Length);
            postStream.Close();

            // Do the actual login
            WebResponse response = login.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
    }
}