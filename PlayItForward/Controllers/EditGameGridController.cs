﻿// <copyright file="EditGameGridController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using PiF.Models;

    public class EditGameGridController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
            // Perform model binding (fill the game properties and validate it).
            if (pifgame != null && ModelState.IsValid)
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
                    pifgame = new PiFGame(pifgame.Count, game);
                    SessionEditGamesRepository.Insert(pifgame);
                }
            }

            // Rebind the grid
            return Json(new[] { pifgame }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id)
        {
            // Delete the record
            SessionEditGamesRepository.Delete(id);

            // Rebind the grid
            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(SessionEditGamesRepository.All().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
            // PiFGame pifgame = SessionEditGamesRepository.One(p => p.ID == id);
            if (pifgame != null && ModelState.IsValid)
            {
                Game game = GameHelper.GetGameList().FirstOrDefault(g => g.Name == pifgame.Name);
                if (game == null)
                {
                    ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionEditGamesRepository.One(p => p.ID == game.id && game.id != pifgame.ID) != null)
                {
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionEditGamesRepository.Delete(pifgame);
                    if (pifgame.Count > 0)
                    {
                        SessionEditGamesRepository.Insert(new PiFGame(pifgame.Count, game));
                    }
                }
            }

            return Json(ModelState.ToDataSourceResult());
        }
    }
}