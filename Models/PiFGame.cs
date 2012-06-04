// <copyright file="PiFGame.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [KnownType(typeof(PiFGame))]
    public class PiFGame
    {
        public PiFGame()
        {
            if (this.Count == 0)
            {
                this.Count = 1;
            }
        }

        /// <summary>Gets all the games available.</summary>
        /// <returns></returns>
        public IEnumerable<Game> All()
        {
            return new PiFDataContext().Games;
        }

        /// <summary>Gets or sets the number of copies being given.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Quanity")]
        [DefaultValue(1)]
        public int Count { get; set; }

        /// <summary>Gets or sets the row ID of the game from the database.</summary>
        [DisplayName("Game")]
        public int ID { get; set; }

        /// <summary>Gets or sets the name of the game.</summary>
        [DisplayName("Game")]
        [UIHint("GameList")]
        [Required]
        public string Name { get; set; }

        /// <summary>Gets or sets the number of copies being given.</summary>
        [DataType("Integer")]
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
        [Required]
        public int PointWorth { get; set; }

        /// <summary>Gets or sets the steam ID of the game.</summary>
        [ReadOnly(true)]
        [DisplayName("SteamID")]
        public int? SteamAppID { get; set; }
    }
}