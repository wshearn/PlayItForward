// <copyright file="NewPiFModel.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Properties containing data when creating a new PiF</summary>
    public class NewPiFModel
    {
        public NewPiFModel()
        {
            if (this.ThreadTitle == null)
            {
                this.ThreadTitle = "[PiF] ";
            }
        }

        /// <summary>Gets or sets the captcha code</summary>
        [DisplayName("Captcha")]
        [Required]
        public string Captcha { get; set; }

        /// <summary>Gets or sets a value indicating whether a captcah code is needed.</summary>
        public bool CaptchaRequired { get; set; }

        /// <summary>Gets or sets the self text for the post.</summary>
        [DisplayName("Self text")]
        [Required]
        public string SelfText { get; set; }

        /// <summary>Gets or sets the thread title.</summary>
        [DisplayName("Thread title")]
        [Required]
        public string ThreadTitle { get; set; }
    }
}