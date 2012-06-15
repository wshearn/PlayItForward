// <copyright file="CompletePiFModel.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PiF.Models
{
    /// <summary>Properties containing data when completing a PiF</summary>
    public class CompletePiFModel
    {
        /// <summary>Gets or sets the PiF ID.</summary>
        public int ID { get; set; }

        /// <summary>Gets or sets the ThingID.</summary>
        public string ThingID { get; set; }

        /// <summary>Gets or sets the thread title.</summary>
        [DisplayName("Thread Title")]
        public string ThreadTitle { get; set; }

        [OutputCache(Duration = 60 * 5)]
        public IList<SelectListItem> ThreadUserList(string thingID)
        {
            var users = new List<SelectListItem>();
            dynamic thread = GetPostComments(thingID);

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
            catch (Exception ex)
            {
                // Lets test and see if any exceptions do get thrown then handle the ones that do gracefully.
                throw;
            }

            users.Add(new SelectListItem { Value = string.Empty, Text = string.Empty });
            return users.OrderBy(u => u.Text.ToLower()).ToList();
        }

        Dictionary<string, string> GetAllUsers(dynamic data)
        {
            var userDictionary = new Dictionary<string, string>();
            if (!userDictionary.ContainsKey(data["author"]))
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
                    foreach (var kvp in subUsers.Where(kvp => !userDictionary.ContainsKey(kvp.Value)))
                    {
                        userDictionary.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return userDictionary;
        }

        [OutputCache(Duration = 60 * 5)]
        dynamic GetPostComments(string thingID)
        {
            string uri = string.Format("http://www.reddit.com/{0}/.json", thingID);
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;

            connect.UserAgent = "r/playitforward site by /u/sevenalive";

            // Do the actual connection
            WebResponse response = connect.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                resp = reader.ReadToEnd();
            }

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
    }
}