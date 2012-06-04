// <copyright file="SelectGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Linq;
using System.Web.Mvc;
using PiF.Models;
using Telerik.Web.Mvc;

namespace PiF.Controllers
{
    public class SelectGridController : Controller
    {
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
            IQueryable<Thread> query = db.Threads.Where(u => u.id == id);
            Thread thread = query.First();

            // Delete the record
            db.Threads.DeleteOnSubmit(thread);

            // Rebind the grid
            return View(new GridModel(SessionPiFRepository.All));
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(SessionGamesRepository.All));
        }
    }
}