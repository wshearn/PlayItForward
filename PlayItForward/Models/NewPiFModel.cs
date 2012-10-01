// <copyright file="NewPiFModel.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
    /// <summary>Properties containing data when creating a new PiF</summary>
    public class NewPiFModel
    {
        /// <summary>Gets or sets the captcha code</summary>
        [DisplayName("Captcha")]
        public string Captcha { get; set; }

        /// <summary>Gets or sets a value indicating whether a captcha code is needed.</summary>
        public bool CaptchaRequired { get; set; }

        /// <summary>Gets or sets the self text for the post.</summary>
        [DisplayName("Self text")]
        [Required]
        public string SelfText { get; set; }

        /// <summary>Gets the thread title.</summary>
        [DisplayName("Thread title")]
        [Required]
        public string ThreadTitle { get; private set; }
    }
}