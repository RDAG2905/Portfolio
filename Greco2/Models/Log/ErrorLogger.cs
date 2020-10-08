using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class ErrorLogger 
    {
        public ErrorLogger(Exception ex,string url,string user)
        {

            string strLogText = "";
            //Exception ex = filterContext.Exception;
            //filterContext.ExceptionHandled = true;

            //var objClass = filterContext;
            strLogText += "Message ---\n{0}" + ex.Message;

            if (ex.Source == ".Net SqlClient Data Provider")
            {
                strLogText += Environment.NewLine + "SqlClient Error ---\n{0}" + "Check Sql Error";
            }
            else if (ex.Source == "System.Web.Mvc")
            {
                strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Code For Error";
            }
            //else if (filterContext.HttpContext.Request.IsAjaxRequest() == true)
            //else if (System.Web.HttpContext.Current.Request.IsAjaxRequest() == true)
            //{
            //    strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Ajax Code For Error";
            //}
            strLogText += Environment.NewLine + "Source ---\n{0}" + ex.Source;
            strLogText += Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace;
            strLogText += Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite;
            if (ex.InnerException != null)
            {
                strLogText += Environment.NewLine + "Inner Exception is {0}" + ex.InnerException;//error prone
            }
            if (ex.HelpLink != null)
            {
                strLogText += Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink;//error prone
            }

            StreamWriter log;

            //string timestamp = DateTime.Now.ToString("d-MMMM-yyyy", new CultureInfo("en-GB"));
            string timestamp = "";

            string error_folder = ConfigurationManager.AppSettings["ErrorLogPath"].ToString();

            if (!System.IO.Directory.Exists(error_folder))
            {
                System.IO.Directory.CreateDirectory(error_folder);
            }

            if (!File.Exists(String.Format(@"{0}\Log_{1}.txt", error_folder, timestamp)))
            {
                //log = new StreamWriter(String.Format(@"{0}\Log_{1}.txt", error_folder, timestamp));
                var anio = DateTime.Now.Year;
                var mes = DateTime.Now.Month;
                var dia = DateTime.Now.Day;
                var hora = DateTime.Now.Hour;
                var minutos = DateTime.Now.Minute;
                var segundos = DateTime.Now.Second;
                timestamp = dia + mes + anio + "_" + hora + "-" + minutos + segundos;
                var ruta = error_folder + "Log" + timestamp + ".txt";
                log = new StreamWriter(@ruta);
            }
            else
            {
                log = File.AppendText(String.Format(@"{0}\Log_{1}.txt", error_folder, timestamp));
            }

            //var controllerName = (string)filterContext.RouteData.Values["controller"];
            //var actionName = (string)filterContext.RouteData.Values["action"];

            // Write to the file:
            log.WriteLine(Environment.NewLine + DateTime.Now);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            //log.WriteLine("Controller Name :- " + controllerName);
            //log.WriteLine("Action Method Name :- " + actionName);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            //log.WriteLine(objClass);
            log.WriteLine(strLogText);
            log.WriteLine("Url Request : " + url);
            log.WriteLine("User : " + user);
            log.WriteLine();

            // Close the stream:
            log.Close();
            //filterContext.HttpContext.Session.Abandon();
            //filterContext.Result = new RedirectToRouteResult
            // (new RouteValueDictionary
            // {
            //     {"controller", "Errorview"}, {"action", "Error"}
            // });

        }

    }
}