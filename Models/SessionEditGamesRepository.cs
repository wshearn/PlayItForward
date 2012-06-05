// <copyright file="SessionGamesRepository.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiF.Models
{
    public static class SessionEditGamesRepository
    {
        public static IList<PiFGame> All()
        {
            var result = HttpContext.Current.Session["EditPiFGames"] as IList<PiFGame>;
            if (result == null)
                HttpContext.Current.Session["EditPiFGames"] = result = new List<PiFGame>();
            return result;
        }

        /// <summary>Deletes the game from the table.</summary>
        /// <param name="index">The row index</param>
        public static void Delete(int index)
        {
            PiFGame target = One(p => p.Game.id == index);
            if (target != null)
                All().Remove(target);
        }

        /// <summary>Insert a game into the table.</summary>
        /// <param name="game">The game object to insert.</param>
        public static void Insert(PiFGame game)
        {
            All().Insert(0, game);
        }

        public static PiFGame One(Func<PiFGame, bool> predicate)
        {
            return All().Where(predicate).FirstOrDefault();
        }

        public static void Clear()
        {
            HttpContext.Current.Session["EditPiFGames"] = new List<PiFGame>();
            return;
        }
    }
}