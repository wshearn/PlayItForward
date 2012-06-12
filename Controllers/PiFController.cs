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
using PiF.Models;

namespace PiF.Controllers
{
    [Authorize]
    public class PiFController : Controller
    {
        public ActionResult Complete(int id)
        {
            if (AccountHelper.CurrentUser.Threads.All(t => t.id != id))
            {
                return RedirectToAction("List");
            }

            var cpm = new CompletePiFModel
                {
                   ThingID = AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThingID 
                };

            Session["ThreadUsers"] = cpm.ThreadUserList(cpm.ThingID);

            var db = new PiFDbDataContext();
            SessionCompleteGamesRepository.Clear();
            foreach (var tg in AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThreadGames)
            {
                User user = db.Users.SingleOrDefault(u => u.id == tg.WinnerID);
                SessionCompleteGamesRepository.Insert(
                    new PiFGameComplete(tg, user != null ? user.Username : string.Empty));
            }

            ViewData["Message"] = "Complete PiF";

            // TODO get the list of entries in the PiF from either reddit or the database.
            return View(cpm);
        }

        [HttpPost]
        public ActionResult Complete(CompletePiFModel model)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.id == model.ID);

            if (thread == null)
            {
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                var db = new PiFDbDataContext();
                thread = db.Threads.Single(t => t.id == model.ID);
                db.ThreadGames.DeleteAllOnSubmit(thread.ThreadGames);
                var newUsers = new List<User>();
                foreach (var pifgame in SessionCompleteGamesRepository.All())
                {
                    for (int i = 1; i <= pifgame.Count; i++)
                    {
                        if (pifgame.WinnerUserName == string.Empty)
                        {
                            ModelState.AddModelError("Winner", "All entrys must have a winner selected");
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
                    Session["ThreadUsers"] = null;
                    return RedirectToAction("View", "PiF", new { thread.id });
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.id == id);

            if (thread == null)
            {
                return RedirectToAction("List");
            }

            if (thread.ThreadGames.Any(tg => tg.WinnerID == null))
            {
                SessionEditGamesRepository.Clear();
                foreach (var tg in AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThreadGames)
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

            return RedirectToAction("View", "PiF", new { id });
        }

        [HttpPost]
        public ActionResult Edit(EditPiFModel model)
        {
            Thread thread = AccountHelper.CurrentUser.Threads.SingleOrDefault(t => t.id == model.ID);

            if (thread == null)
            {
                return RedirectToAction("List");
            }

            var db = new PiFDbDataContext();
            thread = db.Threads.Single(t => t.id == model.ID);
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

        public ActionResult List()
        {
            return View(AccountHelper.CurrentUser.Threads.OrderByDescending(t => t.CreatedDate));
        }

        public ActionResult New()
        {
            ViewData["Message"] = "Create a new PiF";
            SessionNewGamesRepository.Clear();
            return View(new NewPiFModel());
        }

        // POST: /PiF/New
        [HttpPost]
        public ActionResult New(NewPiFModel model)
        {
            if (!SessionNewGamesRepository.All().Any())
            {
                ModelState.AddModelError(string.Empty, "At least 1 game is required to PiF");
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

                    return View(model);
                }

                thingID = response["json"]["data"]["id"];
            }

            if (ModelState.IsValid)
            {
                // DataContext takes a connection string.
                var db = new PiFDbDataContext();

                // TODO: Handle errors such as rate limiting
                var r = new Random();
                var thread = new Thread
                    {
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
                // TODO: Redirect to the PiF Edit/Complete page.
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult View(int id)
        {
            if (!AccountHelper.CurrentUser.Threads.Any(t => t.id == id))
            {
                return RedirectToAction("List");
            }

            return View(AccountHelper.CurrentUser.Threads.Single(t => t.id == id));
        }

        [HttpPost]
        public ActionResult _AutoCompleteGameNameAjaxLoading(string text)
        {
            IEnumerable<Game> data = GameHelper.GetGameList().Where(p => p.Name.StartsWith(text));
            return new JsonResult { Data = data.Select(n => n.Name).ToList() };
        }

        public ActionResult _AutoCompleteUserNameAjaxLoading(string text)
        {
            IEnumerable<User> data = AccountHelper.GetAllUsers().Where(p => p.Username.StartsWith(text));
            return new JsonResult { Data = data.Select(n => n.Username).ToList() };
        }

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
        dynamic SendPost(string data, string uri)
        {
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            connect.Headers["COOKIE"] = (Session["RedditCookie"] as HttpCookie).Value;
            connect.CookieContainer = new CookieContainer();
            Cookie cookie = Utilites.HttpCookieToCookie(Session["RedditCookie"] as HttpCookie);
            cookie.Domain = ".reddit.com";
            cookie.Name = "reddit_session";
            connect.CookieContainer.Add(cookie);
            connect.Method = "POST";
            connect.UserAgent = "r/playitforward site by /u/sevenalive";
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