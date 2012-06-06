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
        #region Properties
        /// <summary>Gets or sets the PiF ID.</summary>
        public int ID { get; set; }

        /// <summary>Gets or sets the ThingID.</summary>
        public string ThingID { get; set; }

        /// <summary>Gets or sets the thread title.</summary>
        [DisplayName("Thread Title")]
        public string ThreadTitle { get; set; }
        #endregion

        #region Public Functions
        [OutputCache(Duration = 60 * 5)]
        public IList<SelectListItem> ThreadUserList(string thingID)
        {
            List<SelectListItem> users = new List<SelectListItem>();
            dynamic thread = GetPostComments(thingID);

            try
            {
                foreach (dynamic post in thread[1]["data"]["children"])
                    foreach (KeyValuePair<string, string> kvp in GetAllUsers(post["data"]))
                    {
                        if (!users.Exists(s => s.Value == kvp.Value))
                            users.Add(new SelectListItem { Value = kvp.Key, Text = kvp.Key });
                    }
            }
            catch (Exception ex) { }

            return users.OrderBy(u => u.Text.ToLower()).ToList();
        }
        #endregion

        #region Private Functions
        private Dictionary<string, string> GetAllUsers(dynamic data)
        {
            Dictionary<string, string> userDictionary = new Dictionary<string, string>();
            if (!userDictionary.ContainsKey(data["author"]))
                userDictionary.Add(data["author"], data["author"]);

            if (data["replies"] is Dictionary<string, object>)
            {
                foreach (dynamic d in data["replies"]["data"]["children"])
                {
                    if (d["data"] != null)
                    {
                        Dictionary<string, string> subUsers = GetAllUsers(d["data"]);
                        foreach (KeyValuePair<string, string> kvp in subUsers)
                        {
                            if (!userDictionary.ContainsKey(kvp.Value))
                                userDictionary.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            return userDictionary;
        }

        [OutputCache(Duration = 60 * 5)]
        private dynamic GetPostComments(string thingID)
        {
            string uri = String.Format("http://www.reddit.com/r/PlayItForward/comments/{0}/.json", thingID);
            var connect = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            // Do the actual connection
            WebResponse response = connect.GetResponse();

            string resp;
            using (var reader = new StreamReader(response.GetResponseStream()))
                resp = reader.ReadToEnd();

            return new JavaScriptSerializer().Deserialize<dynamic>(resp);
        }
        #endregion
    }
}