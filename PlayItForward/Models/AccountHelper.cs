﻿// <copyright file="AccountHelper.cs" project="PlayitForward">Robert Baker</copyright>
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
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }

                string username = HttpContext.Current.User.Identity.Name != string.Empty
                                      ? HttpContext.Current.User.Identity.Name
                                      : HttpContext.Current.Session["Username"].ToString();
                return new PiFDbDataContext().Users.SingleOrDefault(u => u.Username == username) ?? AddUser(username);
            }
        }

        /// <summary>
        /// Adds a user to the database and returns the user object.
        /// </summary>
        /// <param name="username">The username for the user to add.</param>
        /// <returns>The added user object.</returns>
        private static User AddUser(string username)
        {
            var ip = new UserIP { CreatedDate = DateTime.UtcNow, HashedIP = Utilities.GetHash(HttpContext.Current.Request.UserHostAddress) };
            var user = new User { Username = username, RecordCreatedDate = DateTime.UtcNow };
            user.UserIPs.Add(ip);
            var db = new PiFDbDataContext();
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();
            return user;
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