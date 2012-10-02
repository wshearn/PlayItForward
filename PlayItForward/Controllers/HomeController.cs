// <copyright file="HomeController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Mvc;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

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

        [OutputCache(Duration = 60 * 60)]
        public JsonResult GetGames([DataSourceRequest] DataSourceRequest request)
        {
            var games = new PiFDbDataContext().Games.ToDataSourceResult(request, o => new { o.Name, ID = o.id }).Data;
            return Json(games, JsonRequestBehavior.AllowGet);
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

                return View(Utilities.GetThreads(pifs));
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
    }
}