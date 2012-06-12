// <copyright file="EditGameGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System.Linq;
using System.Web.Mvc;
using PiF.Models;
using Telerik.Web.Mvc;

namespace PiF.Controllers
{
    public class EditGameGridController : Controller
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
            SessionEditGamesRepository.Delete(id);

            // Rebind the grid
            return View(new GridModel(SessionEditGamesRepository.All()));
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
            // Create a new instance of the PiFGame class.
            var pifgame = new PiFGame();

            // Perform model binding (fill the game properties and validate it).
            if (TryUpdateModel(pifgame) && ModelState.IsValid)
            {
                Game game = new PiFDbDataContext().Games.FirstOrDefault(g => g.Name == pifgame.Name);
                if (game == null)
                {
                    ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionEditGamesRepository.One(p => p.ID == game.id) != null)
                {
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionEditGamesRepository.Insert(new PiFGame(pifgame.Count, game));
                }
            }

            // Rebind the grid
            return View(new GridModel(SessionEditGamesRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            return View(new GridModel(SessionEditGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            PiFGame pifgame = SessionEditGamesRepository.One(p => p.ID == id);

            if (TryUpdateModel(pifgame) && ModelState.IsValid)
            {
                Game game = GameHelper.GetGameList().FirstOrDefault(g => g.Name == pifgame.Name);
                if (game == null)
                {
                    ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionEditGamesRepository.One(p => p.ID == game.id && game.id != id) != null)
                {
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionEditGamesRepository.Delete(id);
                    if (pifgame.Count > 0)
                    {
                        SessionEditGamesRepository.Insert(new PiFGame(pifgame.Count, game));
                    }
                }
            }

            return View(new GridModel(SessionEditGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            return View(new GridModel(SessionEditGamesRepository.All()));
        }
    }
}