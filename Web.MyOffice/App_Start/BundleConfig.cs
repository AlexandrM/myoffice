using System.Web;
using System.Web.Optimization;

namespace Web.MyOffice
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/JS/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        , "~/Scripts/jquery.unobtrusive-ajax*"
                        , "~/Scripts/jquery.numeric.js"
                        ));

            bundles.Add(new ScriptBundle("~/JS/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/JS/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/CSS/css").Include(
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/JS/bootstrap").Include(
                "~/Content/bootstrap/js/*.js",
                "~/Content/bootstrap/js/locales/*.js"
                ));

            bundles.Add(new StyleBundle("~/CSS/bootstrap").Include(
                "~/Content/bootstrap/css/*.css"
                ));

            bundles.Add(new StyleBundle("~/CSS/tree").Include("~/Content/tree/*.css"));
            bundles.Add(new ScriptBundle("~/JS/tree").Include("~/Content/tree/*.js"));

            bundles.Add(new ScriptBundle("~/JS/ASE").Include(
                        "~/Scripts/ASE.js",
                        "~/Scripts/tools.js",
                        "~/Scripts/date.js",
                        "~/Scripts/DateFormat.js",
                        "~/Scripts/clearableinput.js"
                        ));

            bundles.Add(new ScriptBundle("~/JS/moment").Include(
                "~/Scripts/moment*"));

            bundles.Add(new ScriptBundle("~/JS/typehead").Include("~/Content/typeahead/js/*.js"));
            bundles.Add(new StyleBundle("~/CSS/typehead").Include("~/Content/typeahead/css/*.css"));

            bundles.Add(new ScriptBundle("~/JS/PDF").Include(
                        "~/Scripts/libs/html2canvas.min.js",
                        "~/Scripts/libs/jspdf.js",
                        "~/Scripts/libs/addimage.js",
                        "~/Scripts/libs/png.js",
                        "~/Scripts/libs/png_support.js",
                        "~/Scripts/libs/zlib.js",
                        "~/Scripts/libs/FileSaver.min.js"
                        ));
        }
    }
}
