// <copyright file="CompletePiFModel.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace PiF.Models
{
    /// <summary>Properties containing data when completing a PiF</summary>
    public class CompletePiFModel : PiFGame
    {
        public CompletePiFModel()
        {
        }

        public CompletePiFModel(ThreadGame tg, string winnerUserName)
        {
            ID = tg.id;
            Name = tg.Game.Name;
            Count = 1;
            SteamID = tg.Game.SteamID;
            PointWorth = tg.Game.PointWorth;
            WinnerUserName = winnerUserName;
        }

        /// <summary>Gets or sets the game name.</summary>
        [Required]
        [ReadOnly(true)]
        public new string Name { get; set; }

        /// <summary>Gets or sets the ThingID.</summary>
        public string ThingID { get; set; }

        /// <summary>Gets or sets the username who won the game.</summary>
        [Required]
        [DisplayName("Winner")]
        public string WinnerUserName { get; set; }

        [OutputCache(Duration = 60 * 5)]
        public IList<SelectListItem> ThreadUserList(string thingID)
        {
            var users = new List<SelectListItem>();
            dynamic thread = GetThreadInfo(thingID);

            try
            {
                foreach (var post in thread[1]["data"]["children"])
                {
                    foreach (KeyValuePair<string, string> kvp in GetAllUsers(post["data"]))
                    {
                        if (!users.Exists(s => s.Value == kvp.Value))
                        {
                            users.Add(new SelectListItem { Value = kvp.Key, Text = kvp.Key });
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Lets test and see if any exceptions do get thrown then handle the ones that do gracefully.
                throw;
            }

            // users.Add(new SelectListItem { Value = string.Empty, Text = string.Empty });
            return users.ToList();
        }

        Dictionary<string, string> GetAllUsers(dynamic data)
        {
            var userDictionary = new Dictionary<string, string>();
            if (!userDictionary.ContainsKey(data["author"]) && data["author"] != "[deleted]"
                                                            && data["author"] != string.Empty)
            {
                userDictionary.Add(data["author"], data["author"]);
            }

            if (data["replies"] is Dictionary<string, object>)
            {
                foreach (var d in data["replies"]["data"]["children"])
                {
                    if (d["data"] == null)
                    {
                        continue;
                    }

                    Dictionary<string, string> subUsers = GetAllUsers(d["data"]);
                    foreach (var kvp in
                        subUsers.Where(kvp => !userDictionary.ContainsKey(kvp.Value))
                                .Where(kvp => data["author"] != "[deleted]" && data["author"] != string.Empty))
                    {
                        userDictionary.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return userDictionary;
        }

        [OutputCache(Duration = 60 * 5)]
        dynamic GetThreadInfo(string thingID)
        {
            string uri = string.Format("http://www.reddit.com/{0}/.json", thingID);
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;

            connect.UserAgent = "r/playitforward site by /u/sevenalive";

            // Do the actual connection
            WebResponse response = connect.GetResponse();

            string json;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                json = reader.ReadToEnd();
            }

            return JObject.Parse(json)["json"];
        }
    }
}