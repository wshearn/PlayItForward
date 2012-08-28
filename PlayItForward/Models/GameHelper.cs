// <copyright file="GameHelper.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PiF.Models
{
    public static class GameHelper
    {
        [OutputCache(Duration = 60 * 60)] // cache the list for 60 minutes
        public static IEnumerable<Game> GetGameList()
        {
            return new PiFDbDataContext().Games.ToList();
        }

        [OutputCache(Duration = 60 * 60)] // cache the list for 60 minutes
        public static List<string> GetGameNameList()
        {
            return new PiFDbDataContext().Games.Select(x => x.Name).ToList();
        }
    }
}