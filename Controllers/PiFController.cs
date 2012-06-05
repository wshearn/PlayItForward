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
    [Authorize]
    public class PiFController : Controller
    {
        #region Ajax
        [HttpPost]
        public ActionResult _AutoCompleteAjaxLoading(string text)
        {
            var data = GameHelper.GetGameList().Where(p => p.Name.StartsWith(text));
            return new JsonResult { Data = data.Select(n => n.Name).ToList() };
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region New
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
                this.ModelState.AddModelError(string.Empty, "At least 1 game is required to PiF");
                return View(model);
            }

            bool debug = false;
            string thingID = String.Empty;
            dynamic response;
#if DEBUG //We don't want to send an actual Self.PlayItForward thread to reddit if we are debugging, we will generate a random string instead
            debug = true;
#endif
            if (debug == false)
            {
                response = PostPiF(
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

            if (this.ModelState.IsValid)
            {
                // DataContext takes a connection string.
                var db = new PiFDataContext();

                // TODO: Handle errors such as rate limiting
                Random r = new Random();
                var thread = new Thread
                    {
                        CreatedDate = DateTime.UtcNow,
                        Title = model.ThreadTitle,
                        ThingID = debug == false ? thingID : String.Format("{0}{1}{2}{3}{4}", (char)r.Next(97, 123), (char)r.Next(97, 123), (char)r.Next(97, 123), (char)r.Next(97, 123), (char)r.Next(97, 123)),
                        UserID = AccountHelper.CurrentUser.id
                    };
                foreach (PiFGame pifgame in SessionNewGamesRepository.All())
                {
                    for (int i = 1; i <= pifgame.Count; i++)
                        thread.ThreadGames.Add(new ThreadGame { Thread = thread, Game = pifgame.Game });
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
        #endregion

        #region List
        public ActionResult List()
        {
            return View(AccountHelper.CurrentUser.Threads.OrderByDescending(t => t.CreatedDate));
        }
        #endregion

        #region View
        public ActionResult View(int id)
        {
            if (!AccountHelper.CurrentUser.Threads.Any(t => t.id == id))
                return RedirectToAction("List");
            return View(AccountHelper.CurrentUser.Threads.Single(t => t.id == id));
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            if (!AccountHelper.CurrentUser.Threads.Any(t => t.id == id))
                return RedirectToAction("List");

            if (AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThreadGames.Any(tg => tg.WinnerID == null))
            {
                SessionEditGamesRepository.Clear();
                foreach (ThreadGame tg in AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThreadGames)
                {
                    if (SessionEditGamesRepository.One(p => p.ID == tg.Game.id) != null)
                        SessionEditGamesRepository.One(p => p.ID == tg.Game.id).Count += 1;
                    else
                        SessionEditGamesRepository.Insert(new PiFGame { Count = 1, Game = tg.Game, Name = tg.Game.Name });
                }
                return View(new EditPiFModel());
            }
            else
                return RedirectToAction("View", "PiF", new { id = id });
        }

        [HttpPost]
        public ActionResult Edit(EditPiFModel model)
        {
            return View(model);
        }
        #endregion

        #region Complete
        public ActionResult Complete(int id)
        {
            if (!AccountHelper.CurrentUser.Threads.Any(t => t.id == id))
                return RedirectToAction("List");

            SessionCompleteGamesRepository.Clear();
            foreach (ThreadGame tg in AccountHelper.CurrentUser.Threads.Single(t => t.id == id).ThreadGames)
                SessionCompleteGamesRepository.Insert(tg);

            this.ViewData["Message"] = "Complete PiF";

            // TODO get the list of entries in the PiF from either reddit or the database.
            return View(new CompletePiFModel());
        }

        [HttpPost]
        public ActionResult Complete(CompletePiFModel model)
        {
            return View(model);
        }
        #endregion

        #region PostPiF - Create reddit self post to r/PlayItForward
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
        #endregion
    }
}