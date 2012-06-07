// <copyright file="HomeController.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Web.Mvc;

namespace PiF.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to the Play It Forward App!";

            return View();
        }
    }
}