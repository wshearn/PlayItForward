// <copyright file="SessionPiFRepository.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class SessionPiFRepository
    {
        public static List<PiF> All()
        {
            var result = HttpContext.Current.Session["PiF"] as List<PiF>;

            if (result == null)
            {
                HttpContext.Current.Session["PiF"] = result = new List<PiF>();
            }

            return result;
        }

        /// <summary>Deletes the game from the table.</summary>
        /// <param name="index">The row index</param>
        public static void Delete(int index)
        {
            PiF target = One(p => p.ID == index);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        /// <summary>Insert a game into the table.</summary>
        /// <param name="pif">The game object to insert.</param>
        public static void Insert(PiF pif)
        {
            All().Insert(0, pif);
        }

        public static PiF One(Func<PiF, bool> predicate)
        {
            return All().Where(predicate).FirstOrDefault();
        }
    }
}