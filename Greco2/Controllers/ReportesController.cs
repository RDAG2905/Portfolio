using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Reportes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    
    [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
    public class ReportesController : Controller
    {
        // GET: Reportess
        public ActionResult Index()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View(new ReporteEstadosModelView());
        }

        [NonAction]
        private List<ListaSabanaSP> filtrarSinFechas(ReporteEstadosModelView filtroModel, List<ListaSabanaSP> lista) {

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }
            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }
            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            if (filtroModel.responsableSeleccionado > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableSeleccionado).ToList();
            }
            if (filtroModel.estadoInicialSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Anterior == filtroModel.estadoInicialSeleccionado).ToList();
            }
            if (filtroModel.estadoFinalSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoFinalSeleccionado).ToList();
            }
            return lista;

        }

        [NonAction]
        private List<ListaSabanaSP> filtrarSinOrganismo(ReporteEstadosModelView filtroModel, List<ListaSabanaSP> lista)
        {
            
            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }
            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            if (filtroModel.responsableSeleccionado > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableSeleccionado).ToList();
            }
            if (filtroModel.estadoInicialSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Anterior == filtroModel.estadoInicialSeleccionado).ToList();
            }
            if (filtroModel.estadoFinalSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoFinalSeleccionado).ToList();
            }
            return lista;

        }

        [NonAction]
        private List<ListaSabanaSP> filtrarSinProvincia(ReporteEstadosModelView filtroModel, List<ListaSabanaSP> lista)
        {
            
            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            if (filtroModel.responsableSeleccionado > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableSeleccionado).ToList();
            }
            if (filtroModel.estadoInicialSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Anterior == filtroModel.estadoInicialSeleccionado).ToList();
            }
            if (filtroModel.estadoFinalSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoFinalSeleccionado).ToList();
            }
            return lista;

        }

        [NonAction]
        private List<ListaSabanaSP> filtrarSinEstudio(ReporteEstadosModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableSeleccionado > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableSeleccionado).ToList();
            }
            if (filtroModel.estadoInicialSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Anterior == filtroModel.estadoInicialSeleccionado).ToList();
            }
            if (filtroModel.estadoFinalSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoFinalSeleccionado).ToList();
            }
            return lista;

        }

        [NonAction]
        private List<ListaSabanaSP> filtrarSinResponsable(ReporteEstadosModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.estadoInicialSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Anterior == filtroModel.estadoInicialSeleccionado).ToList();
            }
            if (filtroModel.estadoFinalSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoFinalSeleccionado).ToList();
            }
            return lista;

        }

        [NonAction]
        private List<ListaSabanaSP> aplicarFiltrosReporte(ReporteEstadosModelView filtroModel)
        {

            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde)
                                                                                                                 , new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))
                            .OrderBy(x => x.Fecha_Cambio)

                            .ToList();
                    
                }
               return  filtrarSinFechas(filtroModel, lista);
            }
            else
                if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
                {
                    using (NuevoDbContext context = new NuevoDbContext())

                    {
                        lista = context.Database
                               .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", DateTime.Parse(filtroModel.fechaNotifDesde))
                                                                                                                    , new SqlParameter("@fechaHasta", DateTime.Parse(filtroModel.fechaNotifHasta)))
                               .OrderBy(x => x.Fecha_Cambio)
                               .ToList();                    
                    }
                 return  filtrarSinFechas(filtroModel, lista);
                }
            else
             if (filtroModel.fechaNotifDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacionDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde))
                            .OrderBy(x => x.Fecha_Cambio)
                            .ToList();

                }
                return filtrarSinFechas(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacionHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .OrderBy(x => x.Fecha_Cambio)
                            .ToList();

                }
                return filtrarSinFechas(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacionGciaDesde @fechaGciaDesde", new SqlParameter("@fechaGciaDesde", filtroModel.fechaNotifGciaDesde))
                            .OrderBy(x => x.Fecha_Cambio)
                            .ToList();

                }
                return filtrarSinFechas(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("EstadosPorFechasDeNotificacionGciaHasta @fechaGciaHasta", new SqlParameter("@fechaGciaHasta", filtroModel.fechaNotifGciaHasta))
                            .OrderBy(x => x.Fecha_Cambio)
                            .ToList();

                }
                return filtrarSinFechas(filtroModel, lista);
            }
            else
                if (filtroModel.organismoSeleccionado != null)
                {
                    using (NuevoDbContext context = new NuevoDbContext())
                    {
                        lista = context.Database
                               .SqlQuery<ListaSabanaSP>("EstadosPorOrganismo @organismoId", new SqlParameter("@organismoId", filtroModel.organismoSeleccionado))
                               .OrderBy(x => x.Fecha_Cambio)
                               .ToList();
                    }
                  return  filtrarSinOrganismo(filtroModel, lista);
                }


                else
                    if (filtroModel.provinciaSeleccionada != null)
                    {
                        using (NuevoDbContext context = new NuevoDbContext())
                        {
                            lista = context.Database
                                   .SqlQuery<ListaSabanaSP>("EstadosPorProvincia @provinciaId", new SqlParameter("@provinciaId", filtroModel.provinciaSeleccionada))
                                   .OrderBy(x => x.Fecha_Cambio)
                                   .ToList();
                        }
                        return filtrarSinProvincia(filtroModel, lista);
                    }
                    else
                        if (filtroModel.estudioSeleccionado != null)
                        {
                            using (NuevoDbContext context = new NuevoDbContext())
                            {
                                lista = context.Database
                                       .SqlQuery<ListaSabanaSP>("EstadosPorEstudio @estudioId", new SqlParameter("@estudioId", filtroModel.estudioSeleccionado))
                                       .OrderBy(x => x.Fecha_Cambio)
                                       .ToList();
                            }
                         return filtrarSinEstudio(filtroModel, lista);
                        }
                    else
                        if (filtroModel.responsableSeleccionado > 0)
                    {
                        using (NuevoDbContext context = new NuevoDbContext())
                        {
                            lista = context.Database
                                    .SqlQuery<ListaSabanaSP>("EstadosPorResponsable @responsableId", new SqlParameter("@responsableId", filtroModel.responsableSeleccionado))
                                    .OrderBy(x => x.Fecha_Cambio)
                                    .ToList();
                        }
                         return filtrarSinResponsable(filtroModel, lista);
                    }

            else
                    if (filtroModel.estadoInicialSeleccionado != null && filtroModel.estadoFinalSeleccionado != null)
                        {
                            using (NuevoDbContext context = new NuevoDbContext())
                            {
                                return lista =  context.Database
                                                .SqlQuery<ListaSabanaSP>("EstadosPorInicialyFinal @estadoInicial,@estadoFinal",
                                                new SqlParameter("@estadoInicial", filtroModel.estadoInicialSeleccionado),
                                                new SqlParameter("@estadoFinal", filtroModel.estadoFinalSeleccionado))
                                                .OrderBy(x => x.Fecha_Cambio)
                                                .ToList();
                    
                            }

                    }
                    else
                        if (filtroModel.estadoInicialSeleccionado != null && filtroModel.estadoFinalSeleccionado == null)
                        {
                            using (NuevoDbContext context = new NuevoDbContext())
                            {
                                return lista = context.Database
                                                .SqlQuery<ListaSabanaSP>("EstadosPorInicial @estadoInicial",
                                                new SqlParameter("@estadoInicial", filtroModel.estadoInicialSeleccionado))
                                                .OrderBy(x => x.Fecha_Cambio)
                                                .ToList();

                            }

                        }
                        else
                        if (filtroModel.estadoInicialSeleccionado == null && filtroModel.estadoFinalSeleccionado != null)
                        {
                            using (NuevoDbContext context = new NuevoDbContext())
                            {
                                return lista = context.Database
                                                .SqlQuery<ListaSabanaSP>("EstadosPorFinal @estadoFinal",
                                                new SqlParameter("@estadoFinal", filtroModel.estadoFinalSeleccionado))
                                                .OrderBy(x => x.Fecha_Cambio)
                                                .ToList();

                            }

                        }

            return lista;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GenerarReporte([Bind(Include = "fechaNotifDesde,fechaNotifHasta,fechaNotifGciaDesde,fechaNotifGciaHasta,organismoSeleccionado,provinciaSeleccionada,estudioSeleccionado,responsableSeleccionado,estadoInicialSeleccionado,estadoFinalSeleccionado")] ReporteEstadosModelView model) {
            var lista = aplicarFiltrosReporte(model);
            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                   //  e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                   //  e.Nro_Documento,
                                   //  e.Nro_Linea,
                                   //  e.Tramite_CRM,
                                   //  e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                    // e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Anterior,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Fecha_Cambio
                                    // e.Denuncia_Preventiva,
                                    // e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteEstados"
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

        private static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
        {
            //We really want to know if there is any data at all
            var firstRecord = v.FirstOrDefault();
            if (firstRecord == null)
                return null;

            /*Okay, we have some data. Time to work.*/

            //So dear record, what do you have?
            PropertyInfo[] infos = firstRecord.GetType().GetProperties();

            //Our table should have the columns to support the properties
            DataTable table = new DataTable();

            //Add, add, add the columns
            foreach (var info in infos)
            {

                Type propType = info.PropertyType;

                if (propType.IsGenericType
                    && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) //Nullable types should be handled too
                {
                    table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }

            //Hmm... we are done with the columns. Let's begin with rows now.
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

            //Table is ready to serve.
            table.AcceptChanges();

            return table;
        }

        [HttpGet]
        [NoCache]
        public ActionResult DownloadSpreadsheet(string file)
        {
            if (Path.IsPathRooted(file))
            {
                throw new ArgumentException("La ruta " + file + " no está permitida");
            }
            else {
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