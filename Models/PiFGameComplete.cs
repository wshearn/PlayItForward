// <copyright file="CompletePiFModel.cs" project="PiF">Robert Baker</copyright>
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
            this.ID = tg.id;
            this.Name = tg.Game.Name;
            this.Count = 1;
            this.SteamID = tg.Game.SteamID;
            this.PointWorth = tg.Game.PointWorth;
            this.WinnerUserName = winnerUserName;
        }

        [Required]
        [UIHint("UserList")]
        [DisplayName("Winner")]
        public string WinnerUserName { get; set; }
    }
}