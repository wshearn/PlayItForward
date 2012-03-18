// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PiFController.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
//   Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="Robert Baker">sevenalive</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
// This file is part of PiF.
//   PiF is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. PiF is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//   even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
//  License for more details. You should have received a copy of the GNU General Public License
//    along with PiF.  If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace PiF.Models
{
    using System;
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

        #region Public Methods

        public ActionResult CompletePiF()
        {
            return this.View();
        }

        public ActionResult Create()
        {
            return this.View();
        }

        public ActionResult Index()
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

        [Authorize]
        public ActionResult NewPiF()
        {

            if (this.Request.Cookies["ModHash"].Value != null && Session["ModHash"] == null)
                Session["ModHash"] = this.Request.Cookies["ModHash"].Value;
            if (this.Request.Cookies["RedditCookie"].Value != null && Session["RedditCookie"] == null)
                Session["RedditCookie"] = this.Request.Cookies["RedditCookie"];
            if (this.Request.Cookies["Username"].Value != null && Session["Username"] == null)
                Session["Username"] = this.Request.Cookies["Username"].Value;


            this.ViewData["Message"] = "Create a new PiF";
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new NewPiFModel());
        }

        // POST: /NewPiF/Create
        [HttpPost]
        public ActionResult NewPiF(NewPiFModel model)
        {
            if (!SessionGamesRepository.All().Any())
            {
                this.ModelState.AddModelError(string.Empty, "At least 1 game is required to PiF");
                return this.View(model);
            }

            var response = this.PostPiF(model.ThreadTitle, model.SelfText, (string)this.Session["ModHash"], model.Captcha, (string)this.Session["CaptchaID"]);

            if (response["json"]["errors"].Length > 0)
            {
                if (response["json"]["errors"][0][0] == "BAD_CAPTCHA")
                {
                    this.ModelState.AddModelError(string.Empty, "Reddit is requesting you enter a captcha code");
                    Session["CaptchaID"] = response["json"]["captcha"];
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
                var query = db.Users.Where(u => u.username == (string)this.Session["Username"]);

                // TODO: Handle errors such as rate limiting
                var thread = new Thread
                    {
                        created_date = DateTime.Now.Date,
                        title = model.ThreadTitle,
                        url = url,
                        User = query.First()
                    };

                foreach (var threadGame in SessionGamesRepository.All().Select(game => new Thread_Game
                    {
                        Thread = thread,
                        Game = db.Games.First(u => u.id == game.ID),
                    }))
                {
                    thread.Thread_Games.Add(threadGame);
                }

                // Get a typed table to run queries and insert the data into the table.
                db.Threads.InsertOnSubmit(thread);

                db.SubmitChanges();

                // TODO: Handle errors.
                // TODO: Redirect to the PiF Edit/Complete page.
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        public ActionResult ViewPiFs()
        {
            return this.View();
        }

        #endregion

        private dynamic PostPiF(string title, string text, string modhash, string captcha = null, string iden = null)
        {
            var data =
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
            var cookie = Utilites.HttpCookieToCookie((HttpCookie)this.Session["RedditCookie"]);
            cookie.Domain = ".reddit.com";
            cookie.Name = "reddit_session";
            connect.CookieContainer.Add(cookie);
            connect.Method = "POST";
            connect.ContentType = "application/x-www-form-urlencoded";

            var dataBytes = Encoding.ASCII.GetBytes(data);
            connect.ContentLength = dataBytes.Length;
            var postStream = connect.GetRequestStream();

            postStream.Write(dataBytes, 0, dataBytes.Length);
            postStream.Close();

            // Do the actual connection
            var response = connect.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
    }
}