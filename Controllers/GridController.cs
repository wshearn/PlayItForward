// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridController.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
//   Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="Robert Baker">sevenalive</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of PiF.
//   PiF is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//   License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//   later version. PiF is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//   even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
//   License for more details. You should have received a copy of the GNU General Public License
//   along with PiF.  If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace PiF.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using PiF.Models;

    using Telerik.Web.Mvc;

    public class GridController : Controller
    {
        #region Public Methods

        [GridAction] 
        public ActionResult ClientEditTemplates()
        {
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return View();
        }

        [GridAction]
        public ActionResult _ClientEditTemplates()
        {
            this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return View(new GridModel(SessionGamesRepository.All()));
        }

        [GridAction]
        public ActionResult EditingAjax()
        {
            return this.View();
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
            SessionGamesRepository.Delete(id);

            // Rebind the grid
            //this.ViewData["games"] = new PiFDataContext().Games.ToList();
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAjaxEditing()
        {
            // Create a new instance of the PiFGame class.
            var game = new PiFGame();

            // Perform model binding (fill the game properties and validate it).
            if (TryUpdateModel(game))
            {
                //// I can't figure out how to put the ID in ID field, keeps going into the game field. So for now, I convert the string to an int, then assign the correct game name to it.
                //game.ID = Convert.ToInt32(game.Name);
                // The model is valid - insert the game.
                var dbGame = new PiFDataContext().Games.First(g => g.id == game.ID);

                game.PointWorth = dbGame.pif_points;
                game.SteamAppID = dbGame.steam_app_id;
                game.Name = dbGame.name;
                SessionGamesRepository.Insert(game);
            }

            // Rebind the grid
            return this.View(new GridModel(SessionGamesRepository.All()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAjaxEditing(int id)
        {
            var game = SessionGamesRepository.One(p => p.ID == id);

            this.TryUpdateModel(game);

            var dbGame = new PiFDataContext().Games.First(g => g.id == id);

            game.PointWorth = dbGame.pif_points * game.Count;
            game.Name = dbGame.name;
            game.ID = dbGame.id;
            return View(new GridModel(SessionGamesRepository.All()));
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