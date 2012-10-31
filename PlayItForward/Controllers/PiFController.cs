// <copyright file="PiFController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using PiF.Models;

namespace PiF.Controllers
{
    public class PiFController : Controller
    {
        [Authorize]
        public ActionResult Complete(string thingID)
        {
            if (AccountHelper.CurrentUser.Threads.All(t => t.ThingID != thingID))
            {
                return RedirectToAction("Me", "Account");
            }

            var cpm = new CompletePiFModel
                {
                    ThingID =
                        AccountHelper.CurrentUser.Threads.Single(t => t.ThingID == thingID)
                        .ThingID
                };

            ViewData["ThreadUsers"] = cpm.ThreadUserList(cpm.ThingID);

            var db = new PiFDbDataContext();
            SessionCompleteGamesRepository.Clear();
            foreach (var tg in AccountHelper.CurrentUser.Threads.Single(t => t.ThingID == thingID).ThreadGames)
            {
                User user = db.Users.SingleOrDefault(u => u.id == tg.WinnerID);
                SessionCompleteGamesRepository.Insert(
                    new CompletePiFModel(tg, user != null ? user.Username : string.Empty));
            }

            return View(cpm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Complete(CompletePiFModel model)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.ThingID == model.ThingID);

            if (thread == null)
            {
                return RedirectToAction("Me", "Account");
            }

            if (ModelState.IsValid)
            {
                var db = new PiFDbDataContext();
                thread = db.Threads.Single(t => t.ThingID == model.ThingID);
                db.ThreadGames.DeleteAllOnSubmit(thread.ThreadGames);
                var newUsers = new List<User>();
                foreach (var pifgame in SessionCompleteGamesRepository.All())
                {
                    for (int i = 1; i <= pifgame.Count; i++)
                    {
                        if (pifgame.WinnerUserName == string.Empty)
                        {
                            ModelState.AddModelError("Winner", "All entries must have a winner selected");
                            break;
                        }

                        User user = db.Users.SingleOrDefault(u => u.Username == pifgame.WinnerUserName);
                        if (user == null && newUsers.Exists(u => u.Username == pifgame.WinnerUserName))
                        {
                            user = newUsers.Single(u => u.Username == pifgame.WinnerUserName);
                        }

                        if (user == null)
                        {
                            user = new User { Username = pifgame.WinnerUserName, RecordCreatedDate = DateTime.UtcNow };
                            db.Users.InsertOnSubmit(user);
                            newUsers.Add(user);
                        }

                        var tg = new ThreadGame { ThreadID = thread.id, GameID = pifgame.ID, User = user };
                        db.ThreadGames.InsertOnSubmit(tg);
                    }

                    if (!ModelState.IsValid)
                    {
                        break;
                    }
                }

                if (ModelState.IsValid)
                {
                    db.SubmitChanges();
                    return RedirectToAction("View", "PiF", new { thread.ThingID });
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(string thingID)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.ThingID == thingID);

            if (thread == null)
            {
                return RedirectToAction("Me", "Account");
            }

            ViewData["Games"] = GameHelper.GetGameNameList();

            if (thread.ThreadGames.Any(tg => tg.WinnerID == null))
            {
                SessionEditGamesRepository.Clear();
                foreach (var tg in AccountHelper.CurrentUser.Threads.Single(t => t.ThingID == thingID).ThreadGames)
                {
                    int count = 1;
                    PiFGame egame = SessionEditGamesRepository.One(p => p.ID == tg.Game.id);
                    if (egame != null)
                    {
                        count += egame.Count;
                        SessionEditGamesRepository.Delete(egame.ID);
                    }

                    SessionEditGamesRepository.Insert(new PiFGame(count, tg.Game));
                }

                return View(new EditPiFModel(thread));
            }

            return RedirectToAction("View", "PiF", new { thread.ThingID });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(EditPiFModel model)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.ThingID == model.ThingID);

            if (thread == null)
            {
                return RedirectToAction("Me", "Account");
            }

            var db = new PiFDbDataContext();
            thread = db.Threads.Single(t => t.ThingID == model.ThingID);
            db.ThreadGames.DeleteAllOnSubmit(thread.ThreadGames);
            foreach (var pifgame in SessionEditGamesRepository.All())
            {
                for (int i = 1; i <= pifgame.Count; i++)
                {
                    var tg = new ThreadGame { Thread = thread, GameID = pifgame.ID };
                    db.ThreadGames.InsertOnSubmit(tg);
                }
            }

            db.SubmitChanges();

            return View(model);
        }

        [Authorize]
        public ActionResult New()
        {
            ViewData["Games"] = GameHelper.GetGameNameList();
            return View(new NewPiFModel());
        }

        // POST: /PiF/New
        [HttpPost]
        [Authorize]
        public ActionResult New(NewPiFModel model)
        {
            if (!SessionNewGamesRepository.All().Any())
            {
                ModelState.AddModelError("NewGameGrid", "At least 1 game is required to PiF");
                return View(model);
            }

            string thingID = string.Empty;
            bool debug;
#if DEBUG

            // We don't want to send an actual Self.PlayItForward thread to reddit if we are debugging, we will generate a random string instead
            debug = true;
#endif
            if (debug == false)
            {
                dynamic response;
                try
                {
                    response = PostPiF(
                        model.ThreadTitle,
                        model.SelfText,
                        Session["ModHash"].ToString(),
                        model.Captcha,
                        Session["CaptchaID"].ToString());
                }
                catch (WebException)
                {
                    ModelState.AddModelError("Submit", "Reddit is currently down.");
                    return View(model);
                }

                if (response["errors"].Length > 0)
                {
                    Session["CaptchaID"] = response["captcha"];
                    model.CaptchaRequired = !string.IsNullOrWhiteSpace(Session["CaptchaID"].ToString());
                    ModelState.AddModelError("Submit", Utilities.RedditError(response["errors"][0][0]));
                    return View(model);
                }

                Session["CaptchaID"] = null;
                thingID = response["data"]["id"];
            }

            if (ModelState.IsValid)
            {
                var db = new PiFDbDataContext();

                var r = new Random();
                var thread = new Thread
                    {
                        EndDate = SqlDateTime.MinValue.Value,
                        CreatedDate = DateTime.UtcNow,
                        Title = model.ThreadTitle,
                        ThingID =
                            debug == false
                                ? thingID
                                : string.Format(
                                    "{0}{1}{2}{3}{4}",
                                    (char)r.Next(97, 123),
                                    (char)r.Next(97, 123),
                                    (char)r.Next(97, 123),
                                    (char)r.Next(97, 123),
                                    (char)r.Next(97, 123)),
                        UserID = AccountHelper.CurrentUser.id
                    };
                foreach (var pifgame in SessionNewGamesRepository.All())
                {
                    for (int i = 1; i <= pifgame.Count; i++)
                    {
                        thread.ThreadGames.Add(new ThreadGame { Thread = thread, GameID = pifgame.ID });
                    }
                }

                // Get a typed table to run queries and insert the data into the table.
                db.Threads.InsertOnSubmit(thread);
                db.SubmitChanges();
                SessionNewGamesRepository.Clear();

                // TODO: Handle errors.
                return RedirectToAction("Edit", "PiF", new { thread.ThingID });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Stats(string thingID)
        {
            return View();
        }

        public ActionResult Entries(string thingID)
        {
            return View();
        }

        public new ActionResult View(string thingID)
        {
            if (string.IsNullOrWhiteSpace(thingID))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(Utilities.GetPiFDetails(new PiFDbDataContext().Threads.Single(t => t.ThingID == thingID)));
        }

        [Authorize]
        dynamic PostPiF(string title, string text, string modhash, string captcha = null, string iden = null)
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

            return SendPost(data, "http://www.reddit.com/api/submit");
        }

        /// <summary>Sends data in POST to the specified URI</summary>
        /// <param name="data">POST data</param>
        /// <param name="uri">URI to POST data to</param>
        /// <returns>True/false based on success (NYI)</returns>
        [Authorize]
        dynamic SendPost(string data, string uri)
        {
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            connect.Headers["COOKIE"] = (Session["RedditCookie"] as HttpCookie).Value;
            connect.CookieContainer = new CookieContainer();
            Cookie cookie = Utilities.HttpCookieToCookie(Session["RedditCookie"] as HttpCookie);
            cookie.Domain = ".reddit.com";
            cookie.Name = "reddit_session";
            connect.CookieContainer.Add(cookie);
            connect.Method = "POST";
            connect.UserAgent = "r/playitforward site by /u/sevenalive";
            connect.ContentType = "application/x-www-form-urlencoded";
            connect.Timeout = 5000;
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

            return new JavaScriptSerializer().Deserialize<dynamic>(resp)["json"];
        }
    }
}