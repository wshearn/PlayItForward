// <copyright file="PiFController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
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
    public class PiFController : Controller
    {
        [HttpPost]
        public ActionResult _AutoCompleteAjaxLoading(string text)
        {
            var data = new PiFDataContext().Games.Where(p => p.Name.StartsWith(text));
            return new JsonResult { Data = data.Select(n => n.Name).ToList() };
        }

        [Authorize]
        public ActionResult Complete()
        {
            this.ViewData["Message"] = "Complete PiF";

            // TODO get the list of entries in the PiF from either reddit or the database.
            // this.ViewData["users"] = new PiFDataContext().Threads.
            var list = new List<User>();
            var user = new User { Username = "TestUser1" };
            list.Add(user);
            user.Username = "TestUser2";
            list.Add(user);
            user.Username = "TestUser3";
            list.Add(user);
            user.Username = "TestUser4";
            list.Add(user);
            user.Username = "TestUser5";
            list.Add(user);
            this.ViewData["users"] = list;
            return this.View(new CompletePiFModel());
        }

        [HttpPost]
        public ActionResult Complete(CompletePiFModel model)
        {
            return this.View(model);
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        public ActionResult New()
        {
            var httpCookie = Request.Cookies.Get("ModHash");
            if (httpCookie != null && (httpCookie.Value != null && Session["ModHash"] == null))
                Session["ModHash"] = Request.Cookies.Get("ModHash").Value;
            
            httpCookie = Request.Cookies.Get("RedditCookie");
            if (httpCookie != null && (httpCookie.Value != null && Session["RedditCookie"] == null))
                Session["RedditCookie"] = Request.Cookies.Get("RedditCookie");

            httpCookie = Request.Cookies.Get("Username");
            if (httpCookie != null && (httpCookie.Value != null && Session["Username"] == null))
                Session["Username"] = Request.Cookies.Get("Username").Value;

            ViewData["Message"] = "Create a new PiF";
            return this.View(new NewPiFModel());
        }

        // POST: /PiF/New
        [HttpPost]
        public ActionResult New(NewPiFModel model)
        {
            if (!SessionGamesRepository.All.Any())
            {
                this.ModelState.AddModelError(string.Empty, "At least 1 game is required to PiF");
                return this.View(model);
            }

            dynamic response = PostPiF(
                model.ThreadTitle,
                model.SelfText,
                Session["ModHash"].ToString(),
                model.Captcha,
                Session["CaptchaID"].ToString());

            if (response["json"]["errors"].Length > 0)
            {
                if (response["json"]["errors"][0][0] == "BAD_CAPTCHA")
                {
                    ModelState.AddModelError(string.Empty, "Reddit is requesting you enter a captcha code");
                    Session["CaptchaID"] = response["json"]["captcha"];
                    model.CaptchaRequired = true;
                }

                ViewData["games"] = new PiFDataContext().Games.ToList();

                return View(model);
            }

            if (this.ModelState.IsValid)
            {
                // DataContext takes a connection string.
                var db = new PiFDataContext();
                IQueryable<User> query = db.Users.Where(u => u.Username == Session["Username"].ToString());

                // TODO: Handle errors such as rate limiting
                var thread = new Thread
                    {
                        CreatedDate = DateTime.Now.Date,
                        Title = model.ThreadTitle,
                        ThingID = response["json"]["data"]["id"],
                        User = query.First()
                    };

                foreach (var threadGame in SessionGamesRepository.All.Select(game => new ThreadGame { Thread = thread, Game = db.Games.First(u => u.id == game.Game.id), }))
                    thread.ThreadGames.Add(threadGame);

                // Get a typed table to run queries and insert the data into the table.
                db.Threads.InsertOnSubmit(thread);
                db.SubmitChanges();

                // TODO: Handle errors.
                // TODO: Redirect to the PiF Edit/Complete page.
                return this.RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        [HttpPost]
        public ActionResult Select(SelectPiFModel model)
        {
            return this.View(model);
        }

        public ActionResult ViewPiFs()
        {
            return this.View();
        }

        private ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();
            return RedirectToAction("Index", "Home");
        }

        private dynamic PostPiF(string title, string text, string modhash, string captcha = null, string iden = null)
        {
            string data =
                string.Format(
                    "api_type=json&uh={0}&kind=self&text={1}&sr=playitforward&title={2}&r=playitforward",
                    modhash,
                    text,
                    title);

            if (!string.IsNullOrWhiteSpace(iden) && !string.IsNullOrWhiteSpace(captcha))
                data += string.Format("&iden={0}&captcha={1}", iden, captcha);

            return SendPost(data, "http://www.reddit.com/api/submit");
        }

        /// <summary>Sends data in POST to the specified URI</summary>
        /// <param name="data">POST data</param>
        /// <param name="uri">URI to POST data to</param>
        /// <returns>True/false based on success (NYI)</returns>
        private dynamic SendPost(string data, string uri)
        {
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            connect.Headers["COOKIE"] = (Session["RedditCookie"] as HttpCookie).Value;
            connect.CookieContainer = new CookieContainer();
            Cookie cookie = Utilites.HttpCookieToCookie(Session["RedditCookie"] as HttpCookie);
            cookie.Domain = ".reddit.com";
            cookie.Name = "reddit_session";
            connect.CookieContainer.Add(cookie);
            connect.Method = "POST";
            connect.ContentType = "application/x-www-form-urlencoded";

            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            connect.ContentLength = dataBytes.Length;
            Stream postStream = connect.GetRequestStream();

            postStream.Write(dataBytes, 0, dataBytes.Length);
            postStream.Close();

            // Do the actual connection
            WebResponse response = connect.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
                resp = reader.ReadToEnd();

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
    }
}