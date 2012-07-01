// <copyright file="HomeController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Mvc;
using PiF.Models;

namespace PiF.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        static Func<PiFDbDataContext, IQueryable<Thread>> openThreads;

        public ActionResult About()
        {
            ViewBag.Title = "About Play It Forward";
            return View();
        }

        static List<PiFDetailsModel> GetThreads(IEnumerable<Thread> threads)
        {
            var details = new List<PiFDetailsModel>();
                foreach (Thread thread in threads)
                {
                    var model = new PiFDetailsModel();

                    var games = new List<Game>();

                    // Make this AJAX instead.
                    // string text;
                    // try
                    // {
                    // text = Utilities.GetThreadInfo(thread.ThingID)[0].data.children[0].data.selftext_html;
                    // text = text.Replace("\n\n", "<br /><br />").Replace("\n", "<br />");
                    // }
                    // catch
                    // {
                    // // TODO Handle exceptions better.
                    // text = "Reddit is currently down or too busy, cannot retrieve information at this time";
                    // }
                    foreach (ThreadGame game in thread.ThreadGames)
                    {
                        if (games.Any(x => x.Name == game.Game.Name))
                        {
                            games.Find(x => x.Name == game.Game.Name).Count += 1;
                        }
                        else
                        {
                            var simpleGame = game.Game;
                            simpleGame.Count = 1;
                            games.Add(simpleGame);
                        }
                    }

                    model.Games = games;
                    model.GameCount = thread.ThreadGames.Count;
                    model.ThreadTitle = thread.Title;
                    model.Username = thread.User.Username;
                    model.CreatedDate = thread.CreatedDate;
                    model.ThreadID = thread.ThingID;

                    // model.SelfText = new HtmlString(text);
                    details.Add(model);
                }

                return details;
        }

        [OutputCache(Duration = 60 * 10)]
        public ActionResult Index(int page = 1)
        {
            ViewBag.Title = "Recent Giveaways";
            const int pageSize = 15;
            using (var context = new PiFDbDataContext())
            {
                if (openThreads == null)
                {
                    openThreads =
                        CompiledQuery.Compile<PiFDbDataContext, IQueryable<Thread>>(
                            ctx => ctx.Threads.Where(c => c.EndDate.CompareTo(SqlDateTime.MinValue.Value) != 0));
                }
                
                var threadCount = openThreads.Invoke(context).Count();
                
                double totalPages = (double)threadCount / pageSize;
                List<Thread> pifs = openThreads.Invoke(context).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(totalPages);
                ViewBag.ThreadCount = threadCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;

                return View(GetThreads(pifs));
            }
        }

        [OutputCache(Duration = 60 * 60)]
        public ActionResult Rules()
        {
            ViewBag.Title = "Rules & Guidelines";
            return View();
        }
    }
}