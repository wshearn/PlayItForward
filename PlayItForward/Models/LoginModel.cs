// <copyright file="LoginModel.cs" project="PiF">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiF.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Stay signed in")]
        [DefaultValue(true)]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}