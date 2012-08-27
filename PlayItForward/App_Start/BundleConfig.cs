// <copyright file="BundleConfig.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />
namespace PiF
{
    using System.Web.Optimization;

    /// <summary>
    /// The bundle config.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// The register bundles.
        /// </summary>
        /// <param name="bundles">
        /// The bundles.
        /// </param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui*"));

            bundles.Add(
                new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.unobtrusive*", "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(
                new StyleBundle("~/Content/themes/base/css").Include(
                    "~/Content/themes/base/jquery.ui.core.css", 
                    "~/Content/themes/base/jquery.ui.resizable.css", 
                    "~/Content/themes/base/jquery.ui.selectable.css", 
                    "~/Content/themes/base/jquery.ui.accordion.css", 
                    "~/Content/themes/base/jquery.ui.autocomplete.css", 
                    "~/Content/themes/base/jquery.ui.button.css", 
                    "~/Content/themes/base/jquery.ui.dialog.css", 
                    "~/Content/themes/base/jquery.ui.slider.css", 
                    "~/Content/themes/base/jquery.ui.tabs.css", 
                    "~/Content/themes/base/jquery.ui.datepicker.css", 
                    "~/Content/themes/base/jquery.ui.progressbar.css", 
                    "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}