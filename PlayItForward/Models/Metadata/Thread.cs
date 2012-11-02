// <copyright file="Thread.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace PiF.Models
{
    [MetadataType(typeof(ThreadMetaData))]
    public partial class Thread
    {
    }

    public class ThreadMetaData
    {
        [ScriptIgnore, JsonIgnore]
        public User User { get; set; }
    }

    [MetadataType(typeof(ThreadGameMetaData))]
    public partial class ThreadGame
    {

    }

    public class ThreadGameMetaData
    {
        [ScriptIgnore, JsonIgnore]
        public User User { get; set; }

        [ScriptIgnore, JsonIgnore]
        public Thread Thread { get; set; }
    }
}