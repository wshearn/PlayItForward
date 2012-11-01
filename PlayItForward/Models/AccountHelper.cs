// <copyright file="AccountHelper.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PiF.Models
{
    public static class AccountHelper
    {
        public static User CurrentUser
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated
                    && HttpContext.Current.Session["Username"] == null)
                {
                    return null;
                }

                string username = HttpContext.Current.User.Identity.Name != string.Empty
                                      ? HttpContext.Current.User.Identity.Name
                                      : HttpContext.Current.Session["Username"].ToString();
                var db = new PiFDbDataContext();

                User user = db.Users.SingleOrDefault(u => u.Username == username);

                // TODO: Move this code to something that is called less often. Needs done at login
                // string userIP = Utilities.GetHash(HttpContext.Current.Request.UserHostAddress);
                // if (user == null)
                // {
                // // new user - this is mostly here for debugging so we can reset the database when needed
                // var ip = new UserIP { CreatedDate = DateTime.UtcNow, HashedIP = userIP };
                // user = new User { Username = username, RecordCreatedDate = DateTime.UtcNow };

                // user.UserIPs.Add(ip);
                // db.Users.InsertOnSubmit(user);
                // db.SubmitChanges();
                // }
                // else
                // {
                // // existing user
                // if (user.UserIPs.All(ips => ips.HashedIP != userIP))
                // {
                // var ip = new UserIP { CreatedDate = DateTime.UtcNow, HashedIP = userIP };
                // user.UserIPs.Add(ip);
                // db.SubmitChanges();
                // }
                // }
                return user;
            }
        }

        [OutputCache(Duration = 60)]
        public static IEnumerable<User> GetAllUsers()
        {
            return new PiFDbDataContext().Users.ToList();
        }

        [OutputCache(Duration = 60)]
        public static User GetUser(string username)
        {
            return new PiFDbDataContext().Users.SingleOrDefault(e => e.Username == username);
        }
    }
}