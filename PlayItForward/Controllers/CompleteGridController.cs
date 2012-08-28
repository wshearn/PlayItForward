// <copyright file="CompleteGridController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Web.Mvc;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using PiF.Models;

namespace PiF.Controllers
{
    public class CompleteGridController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, CompletePiFModel game)
        {
            // FIXME: add code to verify user exists on reddit if they are not in our DB
            // Rebind the grid
            if (game != null && ModelState.IsValid)
            {
                SessionCompleteGamesRepository.Insert(game);
            }

            return Json(new[] { game }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, CompletePiFModel game)
        {
            // Delete the record
            SessionCompleteGamesRepository.Delete(game);

            // Rebind the grid
            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(SessionCompleteGamesRepository.All().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, CompletePiFModel game)
        {
            // CompletePiFModel game = SessionCompleteGamesRepository.One(p => p.ID == id);
            TryUpdateModel(game);
            return Json(ModelState.ToDataSourceResult());
        }
    }
}