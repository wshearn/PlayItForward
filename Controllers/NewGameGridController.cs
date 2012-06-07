// <copyright file="NewGameGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class NewGameGridController : Controller
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
            SessionNewGamesRepository.Delete(id);

            // Rebind the grid
            return this.View(new GridModel(SessionNewGamesRepository.All()));
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
            if (this.TryUpdateModel(pifgame) && this.ModelState.IsValid)
            {
                Game dbGame = new PiFDbDataContext().Games.FirstOrDefault(g => g.Name == pifgame.Name);
                if (dbGame == null)
                {
                    this.ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionNewGamesRepository.One(p => p.ID == dbGame.id) != null)
                {
                    this.ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionNewGamesRepository.Insert(new PiFGame(pifgame.Count, dbGame));
                }
            }

            // Rebind the grid
            return this.View(new GridModel(SessionNewGamesRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            return this.View(new GridModel(SessionNewGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            PiFGame pifgame = SessionNewGamesRepository.One(p => p.ID == id);

            if (this.TryUpdateModel(pifgame) && this.ModelState.IsValid)
            {
                Game dbGame = GameHelper.GetGameList().FirstOrDefault(g => g.Name == pifgame.Name);
                if (dbGame == null)
                {
                    this.ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionNewGamesRepository.One(p => p.ID == dbGame.id && dbGame.id != id) != null)
                {
                    this.ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionNewGamesRepository.Delete(id);
                    if (pifgame.Count > 0)
                    {
                        SessionNewGamesRepository.Insert(new PiFGame(pifgame.Count, dbGame));
                    }
                }
            }
            return this.View(new GridModel(SessionNewGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            return this.View(new GridModel(SessionNewGamesRepository.All()));
        }
    }
}