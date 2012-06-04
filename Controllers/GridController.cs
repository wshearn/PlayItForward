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
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
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
            var game = new PiFGame();

            // Perform model binding (fill the game properties and validate it).

            if (this.TryUpdateModel(game))
            {
                Game dbGame = new PiFDataContext().Games.First(g => g.Name == game.Name);

                game.SteamAppID = dbGame.SteamID;
                game.PointWorth = dbGame.PointWorth;
                SessionGamesRepository.Insert(game);
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
            PiFGame game = SessionGamesRepository.One(p => p.ID == id);

            this.TryUpdateModel(game);

            Game dbGame = new PiFDataContext().Games.First(g => g.id == id);

            game.PointWorth = dbGame.PointWorth * game.Count;
            game.Name = dbGame.Name;
            game.ID = dbGame.id;
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new GridModel(SessionGamesRepository.All()));
        }
    }
}