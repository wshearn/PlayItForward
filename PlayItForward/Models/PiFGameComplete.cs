// <copyright file="PiFGameComplete.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PiFGameComplete : PiFGame
    {
        public PiFGameComplete()
        {
        }

        public PiFGameComplete(ThreadGame tg, string winnerUserName)
        {
            ID = tg.id;
            Name = tg.Game.Name;
            Count = 1;
            SteamID = tg.Game.SteamID;
            PointWorth = tg.Game.PointWorth;
            WinnerUserName = winnerUserName;
        }

        /// <summary>Gets or sets the game name.</summary>
        [Required]
        [ReadOnly(true)]
        public new string Name { get; set; }

        [Required]
        [DisplayName("Winner")]
        public string WinnerUserName { get; set; }
    }
}