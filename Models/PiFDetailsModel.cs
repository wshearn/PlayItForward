using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiF.Models
{
    public class PiFDetailsModel
    {
        public HtmlString SelfText { get; set; }

        public string ThreadTitle { get; set; }

        public string ThreadID { get; set; }

        public int GameCount { get; set; }

        public IEnumerable<Game> Games { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Username { get; set; }
    }
}