// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewPiFModel.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
//   Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="Robert Baker">sevenalive</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of PiF.
//   PiF is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. PiF is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
//    License for more details. You should have received a copy of the GNU General Public License
//    along with PiF.  If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Properties containing data when creating a new PiF
    /// </summary>
    public class NewPiFModel
    {
        public NewPiFModel()
        {
            if (ThreadTitle == null)
                ThreadTitle = "[PiF] ";
        }

        /// <summary>
        /// Gets or sets the thread title.
        /// </summary>
        [DisplayName("Thread title")]
        [Required]
        public string ThreadTitle { get; set; }

        /// <summary>
        /// Gets or sets the self text for the post.
        /// </summary>
        [DisplayName("Self text")]
        [Required]
        public string SelfText { get; set; }

        /// <summary>
        /// Gets or sets the captcha code
        /// </summary>
        [DisplayName("Captcha")]
        [Required]
        public string Captcha { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a captcah code is needed.
        /// </summary>
        public bool CaptchaRequired { get; set; }
    }
}