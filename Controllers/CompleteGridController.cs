// <copyright file="CompleteGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Data.Linq;
using System.Web.Mvc;
using PiF.Models;
using Telerik.Web.Mvc;

namespace PiF.Controllers
{
    public class CompleteGridController : Controller
    {
        [GridAction]
        public ActionResult ClientEditTemplates()
        {
            return View();
        }

        [GridAction]
        public ActionResult ClientSideEvents()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAjaxEditing(int id)
        {
            // Delete the record
            SessionCompleteGamesRepository.Delete(id);

            // Rebind the grid
            return View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult EditingAjax()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAjaxEditing()
        {
            //FIXME: add code to verify user exists on reddit if they are not in our DB
            // Rebind the grid
            return View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            return View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            PiFGameComplete game = SessionCompleteGamesRepository.One(p => p.ID == id);
            TryUpdateModel(game);
            return View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            return View(new GridModel(SessionCompleteGamesRepository.All()));
        }
    }
}