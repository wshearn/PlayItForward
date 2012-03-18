// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
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

namespace PiF.Controllers
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

    using PiF.Models;

    public class AccountController : Controller
    {
        #region Public Methods

        public ActionResult LogOff()
        {
            this.Session.Clear();
            FormsAuthentication.SignOut();
            Request.Cookies["RedditCookie"].Expires = DateTime.Now.AddDays(-1d);
            Request.Cookies["Username"].Expires = DateTime.Now.AddDays(-1d);
            Request.Cookies["ModHash"].Expires = DateTime.Now.AddDays(-1d);

            this.Session.Abandon();
            return this.RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return this.View();
        }

        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="username">Reddit account username</param>
        /// <param name="password">Reddit account password</param>
        /// <returns>True/False depending on success of login (NYI)</returns>
        private dynamic Login(string username, string password)
        {
            var login = WebRequest.Create("https://ssl.reddit.com/api/login/" + username) as HttpWebRequest;
            login.CookieContainer = new CookieContainer();
            login.Method = "POST";
            login.ContentType = "application/x-www-form-urlencoded";

            var postData = string.Format("api_type=json&user={0}&passwd={1}", username, password);
            var dataBytes = Encoding.ASCII.GetBytes(postData);
            login.ContentLength = dataBytes.Length;
            var postStream = login.GetRequestStream();

            postStream.Write(dataBytes, 0, dataBytes.Length);
            postStream.Close();

            // Do the actual login
            var response = login.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                var response = this.Login(model.UserName, model.Password);

                if (response["json"]["errors"].Length > 0)
                {
                    if (response["json"]["errors"][0][0] == "INVALID_PASSWORD" || "RATELIMIT")
                    {
                        this.ModelState.AddModelError(string.Empty, response["json"]["errors"][0][1]);
                    }
                }
                else
                {
                    var userIP = Utilites.GetHash(HttpContext.Request.UserHostAddress);
                    var db = new PiFDataContext();

                    var query = db.Users.Where(u => u.username == model.UserName);

                    User user;
                    if (!query.Any())
                    {
                        var ip = new User_IP
                            {
                                created_date = DateTime.Now.Date,
                                hashed_ip = userIP
                            };

                        user = new User { username = model.UserName, record_created_date = DateTime.Now.Date };

                        user.User_IPs.Add(ip);
                        db.Users.InsertOnSubmit(user);
                    }
                    else
                    {
                        user = query.First();
                        if (user.User_IPs.All(ips => ips.hashed_ip != userIP))
                        {
                            user = new User { username = model.UserName, record_created_date = DateTime.Now.Date };

                            var ip = new User_IP
                            {
                                created_date = DateTime.Now.Date,
                                hashed_ip = userIP
                            };
                            user.User_IPs.Add(ip);
                        }
                    }

                    // Set Session vars in case user doesn't use cookies.

                    this.Session["ModHash"] = response["json"]["data"]["modhash"];

                    var redditCookie = new HttpCookie("reddit_session");
                    redditCookie.Value = Server.UrlEncode(response["json"]["data"]["cookie"]);
                    redditCookie.Expires = DateTime.Now.AddYears(1);
                    this.Session["RedditCookie"] = redditCookie;
                    this.Session["Username"] = model.UserName;

                    if (model.RememberMe)
                    {
                        this.Response.Cookies["Username"].Value = model.UserName;
                        this.Response.Cookies["Username"].Expires = DateTime.Now.AddYears(1);
                        this.Response.Cookies["RedditCookie"].Value = Server.UrlEncode(response["json"]["data"]["cookie"]);
                        this.Response.Cookies["RedditCookie"].Expires = DateTime.Now.AddYears(1);
                        this.Response.Cookies["ModHash"].Value = response["json"]["data"]["modhash"];
                        this.Response.Cookies["ModHash"].Expires = DateTime.Now.AddYears(1);
                    }
                    db.SubmitChanges();


                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return this.RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}