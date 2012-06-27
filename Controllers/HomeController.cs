// <copyright file="HomeController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PiF.Models;

namespace PiF.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Title = "About Play It Forward";
            return View();
        }

        public static PagedList<PiFDetailsModel> GetPagedThreads(int skip, int take)
        {
            using (var context = new PiFDbDataContext())
            {
                // refactor consideration...make this a pre-compiled query
                var threads = context.Threads
                  .OrderBy(c => c.CreatedDate).Skip(skip).Take(take).ToList();

                var threadsCount = threads.Count();

                var details = new List<PiFDetailsModel>();
                foreach (Thread thread in threads)
                {
                    var model = new PiFDetailsModel();

                    var games = new List<Game>();
                    // Make this AJAX instead.
                    //string text;
                    //try
                    //{
                    //    text = Utilities.GetThreadInfo(thread.ThingID)[0].data.children[0].data.selftext_html;
                    //    text = text.Replace("\n\n", "<br /><br />").Replace("\n", "<br />");
                    //}
                    //catch
                    //{
                    //    // TODO Handle exceptions better.
                    //    text = "Reddit is currently down or too busy, cannot retrieve information at this time";
                    //}

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

                  //  model.SelfText = new HtmlString(text);

                    details.Add(model);
                }

                return new PagedList<PiFDetailsModel>
                {
                    Entities = details,
                    HasNext = skip + 10 < threadsCount,
                    HasPrevious = skip > 0
                };
            }
        }

        [OutputCache(Duration = 60 * 10)]
        public ActionResult Index(int? page)
        {
            ViewBag.Title = "Recent Giveaways";
            var threads = GetPagedThreads((page ?? 0) * 15, 15);
            ViewBag.HasPrevious = threads.HasPrevious;
            ViewBag.HasMore = threads.HasNext;
            ViewBag.CurrentPage = page ?? 0;
            return View(threads.Entities);

            //var threads = new PiFDbDataContext().Threads.Skip(startIndex).Take(15); //.Where(x => x.EndDate.CompareTo(SqlDateTime.MinValue.Value) == 0);

            //var details = new List<PiFDetailsModel>();
            //foreach (Thread thread in threads)
            //{
            //    var model = new PiFDetailsModel();

            //    var games = new List<Game>();
            //    string text;
            //    try
            //    {
            //        text = Utilities.GetThreadInfo(thread.ThingID)[0]["data"]["children"][0]["data"]["selftext"];
            //        text = text.Replace("\n\n", "<br /><br />").Replace("\n", "<br />");
            //    }
            //    catch
            //    {
            //        // TODO Handle exceptions better.
            //        text = "Reddit is currently down or too busy, cannot retrieve information at this time";
            //    }

            //    foreach (ThreadGame game in thread.ThreadGames)
            //    {
            //        if (games.Any(x => x.Name == game.Game.Name))
            //        {
            //            games.Find(x => x.Name == game.Game.Name).Count += 1;
            //        }
            //        else
            //        {
            //            var simpleGame = game.Game;
            //            simpleGame.Count = 1;
            //            games.Add(simpleGame);
            //        }
            //    }

            //    model.Games = games;
            //    model.GameCount = thread.ThreadGames.Count;
            //    model.ThreadTitle = thread.Title;
            //    model.Username = thread.User.Username;
            //    model.CreatedDate = thread.CreatedDate;
            //    model.ThreadID = thread.ThingID;

            //    model.SelfText = new HtmlString(text);

            //    details.Add(model);
            //}

            //return View(details);
        }

        [OutputCache(Duration = 60 * 60)]
        public ActionResult Rules()
        {
            ViewBag.Title = "Rules & Guidelines";
            return View();
        }
    }
}