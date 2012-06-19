// <copyright file="HomeController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
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

        [OutputCache(Duration = 60 * 10)]
        public ActionResult Index()
        {
            ViewBag.Title = "Recent Giveaways";
            var threads = new PiFDbDataContext().Threads.Where(x => x.EndDate.CompareTo(SqlDateTime.MinValue.Value) == 0);

            var details = new List<PiFDetailsModel>();
            foreach (Thread thread in threads)
            {
                var model = new PiFDetailsModel();
                string text;
                try
                {
                    text = Utilities.GetThreadInfo(thread.ThingID)[0]["data"]["children"][0]["data"]["selftext"];
                    text = text.Replace("\n\n", "<br /><br />").Replace("\n", "<br />");
                }
                catch
                {
                    // TODO Handle exceptions better.
                    text = "Reddit is currently down or too busy, cannot retrieve information at this time";
                }

                model.Thread = thread;
                model.SelfText = new HtmlString(text);

                details.Add(model);
            }

            return View(details);
        }

        public ActionResult Rules()
        {
            ViewBag.Title = "Rules & Guidelines";
            return View();
        }
    }
}