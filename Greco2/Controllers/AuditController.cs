using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
    public class AuditController : Controller
    {
        // GET: Audit
        public ActionResult Index()
        {            
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View();
        }

        //Queda para más adelante
        //[HttpPost]
        //public ActionResult GetListAuditEvents(string fechaInicio,string fechaFin)
        //{
        //    List<SPChangeLogger> lista = new List<SPChangeLogger>();
        //    using (var context = new NuevoDbContext())
        //    {
        //        if (!String.IsNullOrEmpty(fechaInicio) && !String.IsNullOrEmpty(fechaFin)) {
        //            var fechaDesde = DateTime.Parse(fechaInicio);
        //            var fechaHasta = DateTime.Parse(fechaFin);
        //            lista = context.Database
        //                        .SqlQuery<SPChangeLogger>("AuditoriaPorFechaInicioyFechaFin @fechaInicio,@fechaFin", new SqlParameter("@fechaInicio", fechaDesde)
        //                                                                                                             , new SqlParameter("@fechaFin", fechaHasta))
        //                        .ToList();                    
        //        }
        //        else
        //        if (!String.IsNullOrEmpty(fechaInicio))
        //        {
        //            var fechaDesde = DateTime.Parse(fechaInicio);
        //            lista = context.Database
        //                       .SqlQuery<SPChangeLogger>("AuditoriaPorFechaInicio @fechaInicio", new SqlParameter("@fechaInicio", fechaDesde))
        //                       .ToList();
        //        }
        //        else
        //        if (!String.IsNullOrEmpty(fechaFin))
        //        {
        //            var fechaHasta = DateTime.Parse(fechaFin);
        //            lista = context.Database
        //                       .SqlQuery<SPChangeLogger>("AuditoriaPorFechaFin @fechaFin", new SqlParameter("@fechaFin", fechaHasta))
        //                       .ToList();

        //        }
               
        //        return PartialView("ListSystemEvents", lista);
        //    }
            
        //}
        private List<SPChangeLogger> aplicarFiltroReportesAuditoria(string fechaInicio, string fechaFin)
        {

            List<SPChangeLogger> lista = new List<SPChangeLogger>();
            using (var context = new NuevoDbContext())
            {
                if (!String.IsNullOrEmpty(fechaInicio) && !String.IsNullOrEmpty(fechaFin))
                {
                    var fechaDesde = DateTime.Parse(fechaInicio);
                    var fechaHasta = DateTime.Parse(fechaFin);
                    lista = context.Database
                                .SqlQuery<SPChangeLogger>("AuditoriaPorFechaInicioyFechaFin @fechaInicio,@fechaFin", new SqlParameter("@fechaInicio", fechaDesde)
                                                                                                                     , new SqlParameter("@fechaFin", fechaHasta))
                                .ToList();
                }
                else
                if (!String.IsNullOrEmpty(fechaInicio))
                {
                    var fechaDesde = DateTime.Parse(fechaInicio);
                    lista = context.Database
                               .SqlQuery<SPChangeLogger>("AuditoriaPorFechaInicio @fechaInicio", new SqlParameter("@fechaInicio", fechaDesde))
                               .ToList();
                }
                else
                if (!String.IsNullOrEmpty(fechaFin))
                {
                    var fechaHasta = DateTime.Parse(fechaFin);
                    lista = context.Database
                               .SqlQuery<SPChangeLogger>("AuditoriaPorFechaFin @fechaFin", new SqlParameter("@fechaFin", fechaHasta))
                               .ToList();

                }
                lista = lista.OrderBy(x=>x.FechaCambio).ToList();
                return lista;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GenerarReporteAuditoria(string fechaInicio,string fechaFin)
        {
            var lista = aplicarFiltroReportesAuditoria(fechaInicio,fechaFin);
            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.FechaCambio,
                                     e.ObjetoModificado,
                                     e.ObjetoId,
                                     e.Descripcion,
                                     e.ValorAnterior,
                                     e.ValorActual,
                                     e.Usuario
                                    
                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteAuditoria"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("EstadosDenuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);

                Excel.CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

                return Json(new { fileName = fileName, errorMessage = "" });
            }
            else
            {
                return Json(new { fileName = "isEmpty", errorMessage = "" });
            }

        }

        public static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
        {
            
            var firstRecord = v.FirstOrDefault();
            if (firstRecord == null)
                return null;

           
            PropertyInfo[] infos = firstRecord.GetType().GetProperties();

           
            DataTable table = new DataTable();

            
            foreach (var info in infos)
            {

                Type propType = info.PropertyType;

                if (propType.IsGenericType
                    && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) 
                {
                    table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }

            
            DataRow row;

            foreach (var record in v)
            {
                row = table.NewRow();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row[i] = infos[i].GetValue(record) != null ? infos[i].GetValue(record) : DBNull.Value;
                }

                table.Rows.Add(row);
            }

            
            table.AcceptChanges();

            return table;
        }

        [HttpGet]
        [NoCache]
        public ActionResult DownloadSpreadsheet(string file)
        {
            if (Path.IsPathRooted(file))
            {
                throw new ArgumentNullException("El Path contiene una ruta Absoluta : " + file);
            }
            else
            {
                string fullPath = Path.Combine(Server.MapPath("~/temp"), file);
                return File(fullPath, "application/vnd.ms-excel", file);
            }

        }



        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuted(ActionExecutedContext context)
            {
                context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

    }
}