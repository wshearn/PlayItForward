// <copyright file="UsersController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Linq;
using System.Web.Http;
using PiF.Models;

namespace PiF.Controllers
{
    public class UsersController : ApiController
    {
        public User GetUserById(int id)
        {
            return new PiFDbDataContext().Users.SingleOrDefault(x => x.id == id);
        }

        public User GetUserByName(string username)
        {
            return new PiFDbDataContext().Users.SingleOrDefault(x => x.Username == username);
        }
    }
}