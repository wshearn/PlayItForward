// <copyright file="PiFGame.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
    public class PiFGame
    {
        private int count = 1;

        public PiFGame()
        {
        }

        public PiFGame(int count, Game game)
        {
            ID = game.id;
            Count = count;
            Name = game.Name;
            SteamID = game.SteamID;
            PointWorth = game.PointWorth * count;
        }

        // Note: The properties below cannot be set to protected set while used with a parameterless construct, which is used in Ajax, and not caught by static analysis
        // ReSharper disable MemberCanBeProtected.Global

        /// <summary>Gets or sets the number of copies being given.</summary>
        [Required]
        [UIHint("Integer")]
        [DisplayName("Quantity")]
        public int Count
        {
            get { return count; }


            set { count = Math.Max(1, value); }
        }

        /// <summary>Gets or sets the game id.</summary>
        [ReadOnly(true)]
        [DisplayName("ID")]
        [DefaultValue(0)]
        public int ID { get; set; }


        /// <summary>Gets or sets the game name.</summary>
        [Required]
        public string Name { get; set; }


        /// <summary>Gets or sets the number of copies being given.</summary>
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
        public int PointWorth { get; set; }


        /// <summary>Gets or sets the game Steam ID.</summary>
        [ReadOnly(true)]
        [DisplayName("Steam ID")]
        [DefaultValue(0)]
        public int? SteamID { get; set; }

        // ReSharper restore MemberCanBeProtected.Global
    }
}