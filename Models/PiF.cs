// <copyright file="PiF.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary></summary>
    [KnownType(typeof(PiF))]
    public class PiF
    {
        /// <summary>Gets or sets the name of the game.</summary>
        [ReadOnly(true)]
        [DisplayName("Game")]
        public string Game { get; set; }

        /// <summary>Gets or sets the ID.</summary>
        [ReadOnly(true)]
        [DisplayName("ID")]
        [Required]
        public int ID { get; set; }

        /// <summary>Gets or sets the number of points the game is worth.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
        public int PointWorth { get; set; }

        /// <summary>Gets or sets the steam ID of the game.</summary>
        [ReadOnly(true)]
        [DisplayName("SteamID")]
        public int SteamAppID { get; set; }

        /// <summary>Gets or sets the username who received the game.</summary>
        [UIHint("UserList")]
        [DisplayName("User")]
        public string Username { get; set; }
    }
}