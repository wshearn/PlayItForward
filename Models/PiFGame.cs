// <copyright file="PiFGame.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
    public class PiFGame
    {
        private Game game = new Game();
        private int count = 1;

        public PiFGame() { }

        /// <summary>Gets the unique ID of the Game, or 0 if one is not yet selected.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("ID")]
        [DefaultValue(0)]
        public int ID { get { return Game == null ? 0 : Game.id; } }

        /// <summary>Gets or sets the number of copies being given.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Quanity")]
        [DefaultValue(1)]
        public int Count { get { return count; } set { count = Math.Max(0, value); } }

        /// <summary>Gets the Game class that represents the Name and SteamAppID</summary>
        [Required]
        public Game Game { get { return game; } set { game = value; } }

        /// <summary>Gets or sets the game name.</summary>
        [Required]
        [DataType("String")]
        [UIHint("GameList")]
        public string Name { get; set; }

        /// <summary>Gets or sets the number of copies being given.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
        public int PointWorth { get { return Game == null ? 0 : Game.PointWorth * Count; } }
    }
}