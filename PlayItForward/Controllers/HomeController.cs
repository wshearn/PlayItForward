// <copyright file="HomeController.cs" project="PlayitForward">Robert Baker</copyright>
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
    public class HomeController : Controller
    {
        static Func<PiFDbDataContext, IQueryable<Thread>> openThreads;

        public ActionResult About()
        {
            return View();
        }

        [OutputCache(Duration = 60 * 60)]
        public ActionResult Exceptions()
        {
            return View();
        }

        public ActionResult Index(int page = 1)
        {
            const int pageSize = 15;
            using (var context = new PiFDbDataContext())
            {
                if (openThreads == null)
                {
                    openThreads =
                        CompiledQuery.Compile<PiFDbDataContext, IQueryable<Thread>>(
                            ctx => ctx.Threads.Where(c => c.EndDate.CompareTo(SqlDateTime.MinValue.Value) != 0));
                }

                int threadCount = openThreads.Invoke(context).Count();

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
        public ActionResult Points()
        {
            return View();
        }

        [OutputCache(Duration = 60 * 60)]
        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        static List<PiFListModel> GetThreads(IEnumerable<Thread> threads)
        {
            var details = new List<PiFListModel>();
            foreach (var thread in threads)
            {
                var model = new PiFListModel();

                var games = new List<Game>();

                foreach (var game in thread.ThreadGames)
                {
                    if (games.Any(x => x.Name == game.Game.Name))
                    {
                        games.Find(x => x.Name == game.Game.Name).Count += 1;
                    }
                    else
                    {
                        Game simpleGame = game.Game;
                        simpleGame.Count = 1;
                        games.Add(simpleGame);
                    }
                }

                model.Games = games;
                model.GameCount = thread.ThreadGames.Count;
                model.ThreadTitle = thread.Title;
                model.Username = thread.User.Username;
                model.CreatedDate = thread.CreatedDate;
                model.ThingID = thread.ThingID;

                details.Add(model);
            }

            return details;
        }
    }
}