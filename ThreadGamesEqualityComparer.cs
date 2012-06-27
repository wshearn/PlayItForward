using System.Collections.Generic;
using PiF.Models;
namespace PiF
{
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