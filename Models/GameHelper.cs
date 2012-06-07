// <copyright file="GameHelper.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PiF.Models
{
    public static class GameHelper
    {
        [OutputCache(Duration = 60 * 5)] // cache the list for 5 minutes
        public static IEnumerable<Game> GetGameList()
        {
            return new PiFDbDataContext().Games.ToList();
        }
    }
}