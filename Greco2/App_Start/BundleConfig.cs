using System.Web;
using System.Web.Optimization;

namespace Greco2
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/bootbox.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/fullcalendar/fullcalendar.js",
                      "~/Scripts/fullcalendar/gcal.js",
                      "~/Scripts/fullcalendar/locale-all.js",
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/dropzone/dropzone.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/fileinput.js",
                      "~/Scripts/locales/es.js")); 

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/toastr.css",
                      "~/Content/fullcalendar.css",
                      "~/Content/fullcalendar.print.css",
                      "~/Scripts/dropzone/dropzone.css",
                      "~/Scripts/dropzone/basic.css",
                      "~/Content/bootstrap-fileinput/css/fileinput-rtl.css",
                      "~/Content/bootstrap-fileinput/css/fileinput.css")); 

            //BundleTable.EnableOptimizations = true;
        }
    }
}
