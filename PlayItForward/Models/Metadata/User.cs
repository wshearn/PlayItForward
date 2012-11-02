// <copyright file="DBOverrides.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace PiF.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public int MessageCount { get; set; }
    }
    
    public class UserMetadata
    {
        [ScriptIgnore, JsonIgnore]
        public EntitySet<ThreadGame> ThreadGames { get; set; }
    }
}