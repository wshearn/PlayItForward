// <copyright file="PiFController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
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

    public class PiFController : Controller
    {
        // GET: /PiF/

        // GET: /NewPiF/Create

        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        public ActionResult Complete()
        {
            if (this.Request.Cookies["ModHash"].Value != null && this.Session["ModHash"] == null)
            {
                this.Session["ModHash"] = this.Request.Cookies["ModHash"].Value;
            }
            if (this.Request.Cookies["RedditCookie"].Value != null && this.Session["RedditCookie"] == null)
            {
                this.Session["RedditCookie"] = this.Request.Cookies["RedditCookie"];
            }
            if (this.Request.Cookies["Username"].Value != null && this.Session["Username"] == null)
            {
                this.Session["Username"] = this.Request.Cookies["Username"].Value;
            }

            this.ViewData["Message"] = "Complete PiF";


            // TODO get the list of entries in the PiF from either reddit or the database.
            //   this.ViewData["users"] = new PiFDataContext().Threads.
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

        [Authorize]
        public ActionResult New()
        {
            if (this.Request.Cookies["ModHash"].Value != null && this.Session["ModHash"] == null)
            {
                this.Session["ModHash"] = this.Request.Cookies["ModHash"].Value;
            }
            if (this.Request.Cookies["RedditCookie"].Value != null && this.Session["RedditCookie"] == null)
            {
                this.Session["RedditCookie"] = this.Request.Cookies["RedditCookie"];
            }
            if (this.Request.Cookies["Username"].Value != null && this.Session["Username"] == null)
            {
                this.Session["Username"] = this.Request.Cookies["Username"].Value;
            }

            this.ViewData["Message"] = "Create a new PiF";
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new NewPiFModel());
        }

        // POST: /NewPiF/Create
        [HttpPost]
        public ActionResult New(NewPiFModel model)
        {
            if (!SessionGamesRepository.All().Any())
            {
                this.ModelState.AddModelError(string.Empty, "At least 1 game is required to PiF");
                return this.View(model);
            }

            dynamic response = this.PostPiF(
                model.ThreadTitle,
                model.SelfText,
                (string)this.Session["ModHash"],
                model.Captcha,
                (string)this.Session["CaptchaID"]);

            if (response["json"]["errors"].Length > 0)
            {
                if (response["json"]["errors"][0][0] == "BAD_CAPTCHA")
                {
                    this.ModelState.AddModelError(string.Empty, "Reddit is requesting you enter a captcha code");
                    this.Session["CaptchaID"] = response["json"]["captcha"];
                    model.CaptchaRequired = true;
                }

                this.ViewData["games"] = new PiFDataContext().Games.ToList();

                return this.View(model);
            }

            string url = "http://reddit.com";

            if (this.ModelState.IsValid)
            {
                // DataContext takes a connection string.
                var db = new PiFDataContext();
                IQueryable<User> query = db.Users.Where(u => u.Username == (string)this.Session["Username"]);

                // TODO: Handle errors such as rate limiting
                var thread = new Thread
                    { CreatedDate = DateTime.Now.Date, Title = model.ThreadTitle, Url = url, User = query.First() };

                foreach (
                    var threadGame in
                        SessionGamesRepository.All().Select(
                            game => new ThreadGame { Thread = thread, Game = db.Games.First(u => u.id == game.ID), }))
                {
                    thread.ThreadGames.Add(threadGame);
                }

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

        public ActionResult ViewPiFs()
        {
            return this.View();
        }

        private ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            this.Session.Clear();
            this.Session.Abandon();
            this.Response.Cookies.Clear();
            return this.RedirectToAction("Index", "Home");
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
            {
                data += string.Format("&iden={0}&captcha={1}", iden, captcha);
            }

            return this.SendPost(data, "http://www.reddit.com/api/submit");
        }

        /// <summary>
        /// Sends data in POST to the specified URI
        /// </summary>
        /// <param name="data">POST data</param>
        /// <param name="uri">URI to POST data to</param>
        /// <returns>True/false based on success (NYI)</returns>
        private dynamic SendPost(string data, string uri)
        {
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            connect.Headers["COOKIE"] = ((HttpCookie)this.Session["RedditCookie"]).Value;
            connect.CookieContainer = new CookieContainer();
            Cookie cookie = Utilites.HttpCookieToCookie((HttpCookie)this.Session["RedditCookie"]);
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
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
    }
}