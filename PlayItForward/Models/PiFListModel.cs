// <copyright file="PiFListModel.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Collections.Generic;

namespace PiF.Models
{
    public class PiFListModel
    {
        /// <summary>
        /// Gets or sets the date the thread was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date the thread was closed.
        /// </summary>
        public DateTime ClosedDate { get; set; }

        /// <summary>
        /// Gets or sets the number of games in the PiF.
        /// </summary>
        public int GameCount { get; set; }

        /// <summary>
        /// Gets or sets the Games for the PiF
        /// </summary>
        public IEnumerable<Game> Games { get; set; }

        /// <summary>
        /// Gets or sets the thing ID for the thread.
        /// </summary>
        public string ThingID { get; set; }

        /// <summary>
        /// Gets or sets the thread title.
        /// </summary>
        public string ThreadTitle { get; set; }

        /// <summary>
        /// Gets or sets the username for the giver.
        /// </summary>
        public string Username { get; set; }
    }
}