// <copyright file="CompletePiFModel.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
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

        [Required]
        [UIHint("UserList")]
        [DisplayName("Winner")]
        public string WinnerUserName { get; set; }
    }
}