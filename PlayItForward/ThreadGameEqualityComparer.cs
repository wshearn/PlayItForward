// <copyright file="ThreadGameEqualityComparer.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF
{
    using System.Collections.Generic;

    using PiF.Models;

    public class ThreadGameEqualityComparer : IEqualityComparer<ThreadGame>
    {
        public bool Equals(ThreadGame b1, ThreadGame b2)
        {
            return b1.GameID == b2.GameID;
        }

        public int GetHashCode(ThreadGame obj)
        {
            return obj.GameID.GetHashCode();
        }
    }
}