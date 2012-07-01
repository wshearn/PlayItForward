// <copyright file="PiFGame.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
    public class PiFGame
    {
        int count = 1;

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

        // The properties below cannot be set to protected set while used with a parameterless construct, which is used in Ajax, and not caught by static analysis

        /// <summary>Gets or sets the number of copies being given.</summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Quantity")]
        [DefaultValue(1)]
        public int Count
        {
            get { return count; }
// ReSharper disable MemberCanBeProtected.Global
            set { count = Math.Max(0, value); }
// ReSharper restore MemberCanBeProtected.Global
        }

        /// <summary>Gets or sets the game id.</summary>
        [ReadOnly(true)]
        [DataType("Integer")]
        [DisplayName("ID")]
        //[ScaffoldColumn(false)]
        [DefaultValue(0)]
// ReSharper disable MemberCanBeProtected.Global
        public int ID { get; set; }
// ReSharper restore MemberCanBeProtected.Global

        /// <summary>Gets or sets the game name.</summary>
        [Required]
        [DataType("String")]
        [UIHint("GameList")]
// ReSharper disable MemberCanBeProtected.Global
        public string Name { get; set; }
// ReSharper restore MemberCanBeProtected.Global

        /// <summary>Gets or sets the number of copies being given.</summary>
        [DataType("Integer")]
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
// ReSharper disable MemberCanBeProtected.Global
        public int PointWorth { get; set; }
// ReSharper restore MemberCanBeProtected.Global

        /// <summary>Gets or sets the game Steam ID.</summary>
        [ReadOnly(true)]
        [DisplayName("Steam ID")]
        [DefaultValue(0)]
// ReSharper disable MemberCanBeProtected.Global
        public int? SteamID { get; set; }
// ReSharper restore MemberCanBeProtected.Global
    }
}