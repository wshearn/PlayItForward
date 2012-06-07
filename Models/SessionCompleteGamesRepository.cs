// <copyright file="SessionGamesRepository.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiF.Models
{
    public static class SessionCompleteGamesRepository
    {
        public static IList<PiFGameComplete> All()
        {
            var result = HttpContext.Current.Session["CompletePiFGames"] as IList<PiFGameComplete>;
            if (result == null)
            {
                HttpContext.Current.Session["CompletePiFGames"] = result = new List<PiFGameComplete>();
            }
            return result;
        }

        public static void Clear()
        {
            HttpContext.Current.Session["CompletePiFGames"] = new List<ThreadGame>();
            return;
        }

        /// <summary>Deletes the game from the table.</summary>
        /// <param name="index">The row index</param>
        public static void Delete(int index)
        {
            PiFGameComplete target = One(p => p.ID == index);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        /// <summary>Insert a game into the table.</summary>
        /// <param name="game">The game object to insert.</param>
        public static void Insert(PiFGameComplete game)
        {
            All().Insert(0, game);
        }

        public static PiFGameComplete One(Func<PiFGameComplete, bool> predicate)
        {
            return All().Where(predicate).FirstOrDefault();
        }
    }
}