// <copyright file="NewGameGridController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Linq;
using System.Web.Mvc;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using PiF.Models;

namespace PiF.Controllers
{
    public class NewGameGridController : Controller
    {
        [HttpPost]
        public ActionResult NggCreate([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
            System.Diagnostics.Debug.WriteLine("Create Method Fired");
            // Perform model binding (fill the game properties and validate it).
            if (pifgame != null)
            {
                //if (pifgame.Count < 1)
                //{
                //    pifgame.Count = 1;
                //}

                Game game = new PiFDbDataContext().Games.SingleOrDefault(g => g.Name == pifgame.Name);

                if (game == null)
                {
                    ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionNewGamesRepository.One(p => p.ID == game.id) != null)
                {
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    pifgame.PointWorth = game.PointWorth;
                    pifgame.SteamID = game.SteamID;
                    pifgame.ID = game.id;
                    SessionNewGamesRepository.Insert(pifgame);
                }
            }

            // Rebind the grid
            return Json(new[] { pifgame }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult NggDelete([DataSourceRequest] DataSourceRequest request, PiFGame game)
        {
            // Delete the record
            SessionNewGamesRepository.Delete(game);

            // Rebind the grid
            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult NggRead([DataSourceRequest] DataSourceRequest request)
        {
            System.Diagnostics.Debug.WriteLine("Read Method Fired");
            return Json(SessionNewGamesRepository.All().ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult NggUpdate([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
            System.Diagnostics.Debug.WriteLine("Update Method Fired");
            // PiFGame pifgame = SessionNewGamesRepository.One(p => p.ID == id);
            if (pifgame != null && ModelState.IsValid)
            {
                Game game = GameHelper.GetGameList().FirstOrDefault(g => g.Name == pifgame.Name);
                if (game == null)
                {
                    ModelState.AddModelError("Name", "Invalid game name.");
                }
                else if (SessionNewGamesRepository.One(p => p.ID == game.id && game.id != pifgame.ID) != null)
                {
                    ModelState.AddModelError("Name", "Duplicate game, please edit existing row.");
                }
                else
                {
                    SessionNewGamesRepository.Delete(pifgame);
                    if (pifgame.Count > 0)
                    {
                        SessionNewGamesRepository.Insert(new PiFGame(pifgame.Count, game));
                    }
                }
            }

            return Json(ModelState.ToDataSourceResult());
        }
    }
}