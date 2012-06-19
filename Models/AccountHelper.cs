// <copyright file="AccountHelper.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System;
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
                string userIP = Utilities.GetHash(HttpContext.Current.Request.UserHostAddress);
                User user = db.Users.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    // new user - this is mostly here for debugging so we can reset the database when needed
                    var ip = new UserIP { CreatedDate = DateTime.UtcNow, HashedIP = userIP };
                    user = new User { Username = username, RecordCreatedDate = DateTime.UtcNow };

                    user.UserIPs.Add(ip);
                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges();
                }
                else
                {
                    // existing user
                    if (user.UserIPs.All(ips => ips.HashedIP != userIP))
                    {
                        var ip = new UserIP { CreatedDate = DateTime.UtcNow, HashedIP = userIP };
                        user.UserIPs.Add(ip);
                        db.SubmitChanges();
                    }
                }

                return user;
            }
        }

        [OutputCache(Duration = 60)]
        public static IEnumerable<User> GetAllUsers()
        {
            return new PiFDbDataContext().Users.ToList();
        }

        [OutputCache(Duration = 60)]
        public static IList<User> GetAllUsersWithBlank()
        {
            List<User> users = new PiFDbDataContext().Users.ToList();
            users.Insert(0, new User());
            return users;
        }
    }
}