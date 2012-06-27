using System.Collections.Generic;

namespace PiF
{
    public class PagedList<T>
    {
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public List<T> Entities { get; set; }
    }
}