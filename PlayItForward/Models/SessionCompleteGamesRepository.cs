// <copyright file="SessionCompleteGamesRepository.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiF.Models
{
    public static class SessionCompleteGamesRepository
    {
        public static IList<CompletePiFModel> All()
        {
            var result = HttpContext.Current.Session["CompletePiFGames"] as IList<CompletePiFModel>;
            if (result == null)
            {
                HttpContext.Current.Session["CompletePiFGames"] = result = new List<CompletePiFModel>();
            }

            return result;
        }

        public static void Clear()
        {
            HttpContext.Current.Session["CompletePiFGames"] = new List<ThreadGame>();
        }

        /// <summary>Deletes the game from the table.</summary>
        /// <param name="index">The row index</param>
        public static void Delete(int index)
        {
            CompletePiFModel target = One(p => p.ID == index);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public static void Delete(CompletePiFModel game)
        {
            CompletePiFModel target = One(p => p.ID == game.ID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        /// <summary>Insert a game into the table.</summary>
        /// <param name="game">The game object to insert.</param>
        public static void Insert(CompletePiFModel game)
        {
            All().Insert(0, game);
        }

        public static CompletePiFModel One(Func<CompletePiFModel, bool> predicate)
        {
            return All().Where(predicate).FirstOrDefault();
        }
    }
}