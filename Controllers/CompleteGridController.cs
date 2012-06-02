// <copyright file="CompleteGridController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Controllers
{
    using System.Data.Linq;
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class CompleteGridController : Controller
    {
        [GridAction]
        public ActionResult ClientEditTemplates()
        {
            // this.ViewData["users"] = new PiFDataContext().Users.ToList();
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
            SessionPiFRepository.Delete(id);

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
            // var game = new PiF();

            //// Perform model binding (fill the game properties and validate it).
            // if (TryUpdateModel(game))
            // {
            // //// I can't figure out how to put the ID in ID field, keeps going into the game field. So for now, I convert the string to an int, then assign the correct game name to it.
            // //game.ID = Convert.ToInt32(game.Name);
            // // The model is valid - insert the game.
            // var dbGame = new PiFDataContext().Threads.First(g => g.id == game.ID);

            // game.PointWorth = dbGame.pif_points;
            // game.SteamAppID = dbGame.steam_app_id;
            // game.Game = dbGame.name;
            // SessionGamesRepository.Insert(game);
            // }

            // Rebind the grid
            return this.View(new GridModel(SessionPiFRepository.All()));
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
            PiF game = SessionPiFRepository.One(p => p.ID == id);

            this.TryUpdateModel(game);

            Table<Thread> pif = new PiFDataContext().Threads;

            return this.View(new GridModel(SessionPiFRepository.All()));
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            // this.ViewData["users"] = new PiFDataContext().Users.ToList();
            return this.View(new GridModel(SessionPiFRepository.All()));
        }
    }
}