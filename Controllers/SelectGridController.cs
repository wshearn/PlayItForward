// <copyright file="SelectGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class SelectGridController : Controller
    {
        #region Public Methods

        [GridAction]
        public ActionResult ClientSideEvents()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult Delete(int id)
        {
            var db = new PiFDataContext();
            var query = db.Threads.Where(u => u.id == id);
            var thread = query.First();

            // Delete the record
            db.Threads.DeleteOnSubmit(thread);


            // Rebind the grid

            //this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new GridModel(SessionPiFRepository.All()));
        }

        [GridAction]
        public ActionResult Select()
        {
            //this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return View(new GridModel(SessionGamesRepository.All()));
        }

        #endregion
    }
}