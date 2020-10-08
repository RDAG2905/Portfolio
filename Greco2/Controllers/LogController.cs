using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Configuration;

namespace Greco2.Controllers
{
    
    [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            //return this.GetListErrors();
            return View();
        }

        //Probar con este , es el actual

        public FileResult GetErrorById(string errorId)
        {
            LogErrorDto log = new LogErrorDto();
            var id = int.Parse(errorId);
            using (var context = new NuevoDbContext())
            {
                log = context.ErrorLogs.Where(x => x.Id == id).FirstOrDefault();
                string Nombre = "Error" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + ".txt";

                string line1 = log.Fecha.ToString();
                string line2 = log.Error.ToString();
                string line3 = log.UrlRequest.ToString();
                string line4 = log.UserId.ToString();
                string line5 = log.ErrorDetallado.ToString();
                string line6 = log.Id.ToString();

                string error_folder = ConfigurationManager.AppSettings["ErrorLogPath"].ToString();
                var ruta = error_folder + Nombre;
                var logFinal = new StreamWriter(@ruta);
                logFinal.WriteLine(line1);
                logFinal.WriteLine(line2);
                logFinal.WriteLine(line3);
                logFinal.WriteLine(line4);
                logFinal.WriteLine(line5);
                logFinal.WriteLine(line6);
                logFinal.WriteLine();
                logFinal.Close();


                return File(Nombre, "application/txt", ruta);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListErrors(string fechaInicio, string fechaFin)
        {
            List<LogErrorDto> lista = new List<LogErrorDto>();
            using (var context = new NuevoDbContext())
            {
                if (!String.IsNullOrEmpty(fechaInicio) && !String.IsNullOrEmpty(fechaFin))
                {
                    var fechaDesde = DateTime.Parse(fechaInicio);
                    var fechaHasta = DateTime.Parse(fechaFin);
                    lista = context.ErrorLogs.Where(e => e.Fecha > DbFunctions.AddDays(fechaDesde,0) && e.Fecha < DbFunctions.AddDays(fechaHasta, 1)).ToList();
                   
                }
                else
                if (!String.IsNullOrEmpty(fechaInicio))
                {
                    var fechaDesde = DateTime.Parse(fechaInicio);
                    lista = context.ErrorLogs.Where(e => e.Fecha > DbFunctions.AddDays(fechaDesde,0)).ToList();
                    
                }
                else
                if (!String.IsNullOrEmpty(fechaFin))
                {
                    var fechaHasta = DateTime.Parse(fechaFin);
                    lista = context.ErrorLogs.Where(e => e.Fecha < DbFunctions.AddDays(fechaHasta, 1)).ToList();
                    
                }
               

                return PartialView("ErrorList", lista);
            }
        }

        //public ActionResult GetErrorById(string errorId)
        //{
        //    StringWriter sw = new StringWriter();
        //    LogErrorDto log = new LogErrorDto();
        //    var id = int.Parse(errorId);
        //    using (var context = new NuevoDbContext())
        //    {
        //        log = context.ErrorLogs.Where(x => x.Id == id).FirstOrDefault();
        //        string Nombre = "Error" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + ".txt";

        //        string line1 = log.Fecha.ToString();
        //        string line2 = log.Error.ToString();
        //        string line3 = log.UrlRequest.ToString();
        //        string line4 = log.UserId.ToString();
        //        string line5 = log.ErrorDetallado.ToString();
        //        string line6 = log.Id.ToString();
        //        //string[] lines = { line1, line2, line3, line4, line5, line6 };

        //        string error_folder = ConfigurationManager.AppSettings["ErrorLogPath"].ToString();
        //        var ruta = error_folder + Nombre;
        //        var logFinal = new StreamWriter(@ruta);
        //        logFinal.WriteLine(line1);
        //        logFinal.WriteLine(line2);
        //        logFinal.WriteLine(line3);
        //        logFinal.WriteLine(line4);
        //        logFinal.WriteLine(line5);
        //        logFinal.WriteLine(line6);
        //        logFinal.WriteLine();
        //        string[] lines = { line1, line2, line3, line4, line5, line6 };
        //        logFinal.Close();

               
        //        Response.ClearContent();
        //        Response.AddHeader("content-disposition", "attachment;filename=Exported_Users.csv");
        //        Response.HeaderEncoding = System.Text.Encoding.Default;
        //        Response.ContentType = "text/csv;charset=utf-8";

                
        //        foreach (var line in lines)
        //        {
        //            //sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
        //            sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}",
        //                                       line1,
        //                                       line2,
        //                                       line3,
        //                                       line4,
        //                                       line5,
        //                                       line6
        //                                       ), System.Text.Encoding.UTF8);

        //        }

        //        Response.Write(sw.ToString());

        //        Response.End();


        //        //return File(Nombre, "application/txt", ruta);
        //    }

        //}

        

        //public FileResult GetErrorById(string errorId) {
        //    LogErrorDto log = new LogErrorDto();
        //    var id = int.Parse(errorId);
        //    using (var context = new NuevoDbContext())
        //    {
        //        log = context.ErrorLogs.Where(x => x.Id == id).FirstOrDefault();
        //        string Nombre = "Error" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + ".txt"; 

        //        string line1 = log.Fecha.ToString();
        //        string line2 = log.Error.ToString();
        //        string line3 = log.UrlRequest.ToString();
        //        string line4 = log.UserId.ToString();
        //        string line5 = log.ErrorDetallado.ToString();   
        //        string line6 = log.Id.ToString();
        //        string[] lines = { line1, line2 , line3 , line4 , line5 , line6 };
        //        var ruta = Server.MapPath("~/LogErrors");

        //        var path = Path.Combine(ruta,Nombre);
        //        System.IO.File.WriteAllLines(path,lines);


        //        return File(ruta,"application/txt",path);
        //    }

        //}
    }
}