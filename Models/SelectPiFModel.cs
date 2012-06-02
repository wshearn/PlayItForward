﻿// <copyright file="SelectPiFModel.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Properties containing data when completing a PiF</summary>
    public class SelectPiFModel
    {
        public string ID { get; set; }

        /// <summary>Gets or sets the thread title.</summary>
        [DisplayName("Thread title")]
        [Required]
        public string ThreadTitle { get; set; }
    }
}