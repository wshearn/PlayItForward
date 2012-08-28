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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
            // Perform model binding (fill the game properties and validate it).
            if (pifgame != null && ModelState.IsValid)
            {
                if (pifgame.Count < 1)
                {
                    pifgame.Count = 1;
                }

                Game game = new PiFDbDataContext().Games.FirstOrDefault(g => g.Name == pifgame.Name);
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
                    SessionNewGamesRepository.Insert(pifgame);
                }
            }

            // Rebind the grid
            return Json(new[] { pifgame }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, PiFGame game)
        {
            // Delete the record
            SessionNewGamesRepository.Delete(game);

            // Rebind the grid
            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(SessionNewGamesRepository.All().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, PiFGame pifgame)
        {
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