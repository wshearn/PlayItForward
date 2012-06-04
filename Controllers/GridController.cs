// <copyright file="GridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class GridController : Controller
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
            SessionGamesRepository.Delete(id);

            // Rebind the grid
            // this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new GridModel(SessionGamesRepository.All()));
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
            // Create a new instance of the PiFGame class.
            var pifgame = new PiFGame();

            // Perform model binding (fill the game properties and validate it).
            if (this.TryUpdateModel(pifgame))
            {
                Game dbGame = new PiFDataContext().Games.FirstOrDefault(g => g.Name == pifgame.Name);
                if (dbGame == null)
                    ModelState.AddModelError("Name", "Invalid game name.");
                else if (SessionGamesRepository.One(p => p.Game.id == dbGame.id) != null)
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");

                if (ModelState.IsValid)
                {
                    pifgame.Game = dbGame;
                    SessionGamesRepository.Insert(pifgame);
                }
            }

            // Rebind the grid
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            // this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            PiFGame pifgame = SessionGamesRepository.One(p => p.Game.id == id);

            if (this.TryUpdateModel(pifgame))
            {
                Game dbGame = new PiFDataContext().Games.FirstOrDefault(g => g.Name == pifgame.Name);
                if (dbGame == null)
                    ModelState.AddModelError("Name", "Invalid game name.");
                else if (SessionGamesRepository.One(p => p.Game.id == dbGame.id && dbGame.id != id) != null)
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");

                if (pifgame.Count == 0)
                    SessionGamesRepository.Delete(id);
                else
                {
                    if (ModelState.IsValid)
                        pifgame.Game = dbGame;
                }
            }
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            return this.View(new GridModel(SessionGamesRepository.All()));
        }
    }
}