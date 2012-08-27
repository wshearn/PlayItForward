// <copyright file="EditPiFModel.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Properties containing data when completing a PiF</summary>
    public class EditPiFModel
    {
        public EditPiFModel()
        {
        }

        public EditPiFModel(Thread thread)
        {
            ThingID = thread.ThingID;
            ThreadTitle = thread.Title;
        }

        /// <summary>Gets or sets the date the PiF was made.</summary>
        [DisplayName("Created")]
        [UIHint("Date")]
        [Required]
        public string CreatedDate { get; set; }

        /// <summary>Gets the thread ThingID</summary>
        [Required]
        public string ThingID { get; set; }

        /// <summary>Gets or sets the thread title.</summary>
        [DisplayName("Thread title")]
        [Required]
        public string ThreadTitle { get; set; }
    }
}