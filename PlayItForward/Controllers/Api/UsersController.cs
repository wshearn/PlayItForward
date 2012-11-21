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
        public User GetUserById(int id)
        {
            return new PiFDbDataContext().Users.SingleOrDefault(x => x.id == id);
        }

        public User GetUserByName(string username)
        {
            return new PiFDbDataContext().Users.SingleOrDefault(x => x.Username == username);
        }

        /// <summary>
        /// Gets the current user that is currently logged in.
        /// </summary>
        /// <returns>The user logged in.</returns>
        public User GetCurrentUser()
        {
            var user = AccountHelper.CurrentUser;

            if (user.SteamID != null)
            {
                string uri =
                    string.Format(
                        "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=B82FD1C7BC17932B62F8453A2CB2CCE2&steamids={0}",
                        user.SteamID);
                var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;

                connect.UserAgent = "playitforward site";

                // Do the actual connection
                WebResponse response = connect.GetResponse();

                string json;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                }

                var steam = JObject.Parse(json)["response"]["players"][0]["avatarmedium"].Value<string>();
                user.SteamAvatar = steam;
            }
            
            return user;
        }
    }
}