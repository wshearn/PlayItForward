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
        public ActionResult DeleteAjaxEditing(int id)
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

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAjaxEditing()
        {
            // Create a new instance of the PiFGame class.
            //var game = new PiF();

            //// Perform model binding (fill the game properties and validate it).
            //if (TryUpdateModel(game))
            //{
            //    //// I can't figure out how to put the ID in ID field, keeps going into the game field. So for now, I convert the string to an int, then assign the correct game name to it.
            //    //game.ID = Convert.ToInt32(game.Name);
            //    // The model is valid - insert the game.
            //    var dbGame = new PiFDataContext().Threads.First(g => g.id == game.ID);

            //    game.PointWorth = dbGame.pif_points;
            //    game.SteamAppID = dbGame.steam_app_id;
            //    game.Game = dbGame.name;
            //    SessionGamesRepository.Insert(game);
            //}

            // Rebind the grid
            return this.View(new GridModel(SessionPiFRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            var game = SessionPiFRepository.One(p => p.ID == id);

            this.TryUpdateModel(game);

            var pif = new PiFDataContext().Threads;

            return View(new GridModel(SessionPiFRepository.All()));
        }

        [GridAction]
        public ActionResult SelectAjaxEditing()
        {
            //this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return View(new GridModel(SessionGamesRepository.All()));
        }

        #endregion
    }
}