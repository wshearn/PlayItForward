// <copyright file="AccountController.cs" project="PlayitForward">Robert Baker</copyright>
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
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PiF.Models;

namespace PiF.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Activity(string username)
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            dynamic response;
            try
            {
                response = Login(model.UserName, model.Password);
            }
            catch (WebException)
            {
                ModelState.AddModelError("SignIn", "Reddit is currently down.");
                return View(model);
            }

            if (response["errors"].Length > 0)
            {
                string error = response["errors"][0][0];

                switch (error)
                {
                    case "BAD_USERNAME":
                    case "WRONG_PASSWORD":
                        ModelState.AddModelError("Password", Utilities.RedditError(response));
                        break;
                    default:
                        ModelState.AddModelError("SignIn", Utilities.RedditError(response));
                        break;
                }

                return View(model);
            }

            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

            // Set Session vars in case user doesn't use cookies.
            Session["Username"] = model.UserName;
            Session["ModHash"] = response["data"]["modhash"];

            var redditCookie = new HttpCookie("reddit_session")
                                   {
                                       Value = Server.UrlEncode(response["data"]["cookie"]), 
                                       Expires = DateTime.Now.AddYears(1)
                                   };
            Session["RedditCookie"] = redditCookie;

            if (model.RememberMe)
            {
                Response.Cookies["Username"].Value = model.UserName;
                Response.Cookies["Username"].Expires = DateTime.Now.AddYears(1);
                Response.Cookies["RedditCookie"].Value = Server.UrlEncode(response["data"]["cookie"]);
                Response.Cookies["RedditCookie"].Expires = DateTime.Now.AddYears(1);
                Response.Cookies["ModHash"].Value = response["data"]["modhash"];
                Response.Cookies["ModHash"].Expires = DateTime.Now.AddYears(1);
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
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

            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Messages(string username)
        {
            return View();
        }

        public ActionResult Preferences()
        {
            return View();
        }

        public ActionResult Profile(string username)
        {
            User user = AccountHelper.GetUser(username);
            if (user != null)
            {
                ViewBag.User = user;
                return View(Utilities.GetThreads(user.Threads.OrderByDescending(t => t.CreatedDate)));
            }

            return new HttpNotFoundResult();
        }

        //public ActionResult LinkSteam()
        //{
        //    var openid = new OpenIdRelyingParty();
        //    IAuthenticationResponse response = openid.GetResponse();

        //    if (response != null)
        //    {
        //        switch (response.Status)
        //        {
        //            case AuthenticationStatus.Authenticated:
        //                FormsAuthentication.RedirectFromLoginPage(
        //                    response.ClaimedIdentifier, false);
        //                break;
        //            case AuthenticationStatus.Canceled:
        //                ModelState.AddModelError("loginIdentifier",
        //                    "Login was cancelled at the provider");
        //                break;
        //            case AuthenticationStatus.Failed:
        //                ModelState.AddModelError("loginIdentifier",
        //                    "Login failed using the provided OpenID identifier");
        //                break;
        //        }
        //    }

        //    return View();
        //}

        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LinkSteam()
        {
                var openid = new OpenIdRelyingParty();
                IAuthenticationRequest request = openid.CreateRequest(
                    Identifier.Parse("http://steamcommunity.com/openid"));

                //// Require some additional data
                //request.AddExtension(new ClaimsRequest
                //{
                //    BirthDate = DemandLevel.NoRequest,
                //    Email = DemandLevel.Require,
                //    FullName = DemandLevel.Require
                //});

            var steamID = openid.GetResponse().ClaimedIdentifier;

                return request.RedirectingResponse.AsActionResult();
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
            login.Timeout = 5000;
            Stream postStream = login.GetRequestStream();

            postStream.Write(dataBytes, 0, dataBytes.Length);
            postStream.Close();

            // Do the actual login
            WebResponse response = login.GetResponse();

            string json;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                json = reader.ReadToEnd();
            }

            return JObject.Parse(json)["json"];
        }
    }
}