// <copyright file="BundleConfig.cs" project="PlayitForward">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System.Web.Optimization;

namespace PiF
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.7.*"));

            bundles.Add(
                new ScriptBundle("~/bundles/index").Include(
                    "~/Scripts/jquery.expander*", 
                    "~/Scripts/jquery.timeago*", 
                    "~/Scripts/jquery-ui.*", 
                    "~/Scripts/index.*"));

            // The Kendo JavaScript bundle
            bundles.Add(
                new ScriptBundle("~/bundles/kendo").Include("~/Scripts/kendo.web.*", "~/Scripts/kendo.aspnetmvc.*"));

            // The Kendo CSS bundle
            bundles.Add(
                new StyleBundle("~/Content/kendo").Include("~/Content/kendo.common.*", "~/Content/kendo.metro.*"));

            // Clear all items from the default ignore list to allow minified CSS and JavaScript files to be included in debug mode
            bundles.IgnoreList.Clear();

            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);

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