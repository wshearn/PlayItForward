using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PiF.Models
{
    public static class SessionGamesRepository
    {
        public static List<PiFGame> All()
        {
            var result = HttpContext.Current.Session["PiFGames"] as List<PiFGame>;

            if (result == null)
            {
                HttpContext.Current.Session["PiFGames"] = result = new List<PiFGame>();
            }
            return result;
        }

        public static PiFGame One(Func<PiFGame, bool> predicate)
        {
            return All().Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Insert a game into the table.
        /// </summary>
        /// <param name="game">The game object to insert.</param>
        public static void Insert(PiFGame game)
        {
            if (game.Count < 1)
            {
                game.Count = 1;
            }

            All().Insert(0, game);
        }


        /// <summary>
        /// Deletes the game from the table.
        /// </summary>
        /// <param name="index">The row index</param>
        public static void Delete(int index)
        {
            var target = One(p => p.ID == index);
            if (target != null)
            {
                All().Remove(target);
            }
        }

    }
}
