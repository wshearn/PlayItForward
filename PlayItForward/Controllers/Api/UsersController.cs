// <copyright file="UsersController.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using PiF.Models;

namespace PiF.Controllers.Api
{
    public class UsersController : ApiController
    {
       

        /// <summary>
        /// Gets the current user that is currently logged in.
        /// </summary>
        /// <returns>The user logged in.</returns>
        public User GetCurrentUser()
        {
            User user = AccountHelper.CurrentUser;

            user.SteamAvatar = user.SteamID > 0
                                   ? AccountHelper.GetSteamPlayerSummary(user.SteamID)["avatarmedium"].Value<string>()
                                   : "http://media.steampowered.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_medium.jpg";

            return user;
        }

        public User GetUserById(int id)
        {
            var user = new PiFDbDataContext().Users.SingleOrDefault(x => x.id == id);
            if (user != null)
            {
                user.SteamAvatar = user.SteamID > 0
                                       ? AccountHelper.GetSteamPlayerSummary(user.SteamID)["avatarmedium"].Value<string>()
                                       : "http://media.steampowered.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_medium.jpg";
            }
            else
            {
                user = new User
                           {
                               SteamAvatar =
                                   "http://media.steampowered.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_medium.jpg"
                           };
            }

            return user;
        }

        public User GetUserByName(string username)
        {
            var user = new PiFDbDataContext().Users.SingleOrDefault(x => x.Username == username);
            if (user != null)
            {
                user.SteamAvatar = user.SteamID > 0
                                       ? AccountHelper.GetSteamPlayerSummary(user.SteamID)["avatarmedium"].Value<string>()
                                       : "http://media.steampowered.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_medium.jpg";
            }
            else
            {
                user = new User
                {
                    SteamAvatar =
                        "http://media.steampowered.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_medium.jpg"
                };
            }

            return user;
        }
    }
}