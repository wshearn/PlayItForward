// <copyright file="Game.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace PiF.Models
{
    [MetadataType(typeof(GameMetaData))]
    public partial class Game
    {
        public int Count { get; set; }

        public string ImageUrl
        {
            get
            {
                if (SteamID > 0)
                {
                    return string.Format(
                        "http://cdn.steampowered.com/v/gfx/{0}/{1}/capsule_184x69.jpg", 
                        IsSteamSubscription ? "subs" : "apps", 
                        SteamID);
                }

                return "/Images/capsules/" + ImageName + ".jpg";
            }
        }
    }

    public class GameMetaData
    {
        [ScriptIgnore, JsonIgnore]
        public EntitySet<ThreadGame> ThreadGames { get; set; }
    }
}