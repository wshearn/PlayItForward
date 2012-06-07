// <copyright file="CompleteGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Controllers
{
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class CompleteGridController : Controller
    {
        [GridAction]
        public ActionResult ClientEditTemplates()
        {
            return this.View();
        }

        [GridAction]
        public ActionResult ClientSideEvents()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAjaxEditing(int id)
        {
            // Delete the record
            SessionCompleteGamesRepository.Delete(id);

            // Rebind the grid
            return this.View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult EditingAjax()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAjaxEditing()
        {
            //FIXME: add code to verify user exists on reddit if they are not in our DB
            // Rebind the grid
            return this.View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            return this.View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            PiFGameComplete game = SessionCompleteGamesRepository.One(p => p.ID == id);
            this.TryUpdateModel(game);
            return this.View(new GridModel(SessionCompleteGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            return this.View(new GridModel(SessionCompleteGamesRepository.All()));
        }
    }
}