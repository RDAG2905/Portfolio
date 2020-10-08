using Greco2.Excel;
using Greco2.Model;
using Greco2.Models;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Evento;
using Greco2.Models.Organismo;
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
using System.Web.UI;

namespace Greco2.Controllers
{
    public class EstudioExternoController : Controller
    {
        // GET: EstudioExterno
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult Index()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View(new AgendaModelView());
        }

        [ActionAuthorize(Roles = "ESTUDIO")]
        public ActionResult AgendaExternos()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View("AgendaExternos", new AgendaModelView());
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetListAgendaToday([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {
            List<AgendaSP> lista = new List<AgendaSP>();
            var fechaDesde = DateTime.Parse(model.fechaDesde);
            var fechaHasta = DateTime.Parse(model.fechaHasta);
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var fechaDesde = model.fechaDesde;
            //var fechaHasta = model.fechaHasta;
            //var fechaDesdex = DateTime.Parse(model.fechaDesde);
            //var fechaHasta = DateTime.Parse(model.fechaHasta);
            //var horaInicio = "00:00:00";
            //var horaFin = "00:00:00";
            //var fechaDesde = DateTime.Parse(model.fechaDesde.Date.ToString() + ' '+ "00:00:00");
            //var fechaHasta = DateTime.Parse(model.fechaHasta.Date.ToString() + " " + "23:59:59");
            using (var context = new NuevoDbContext())
            {
                int responsableId = context.Responsables.Where(x => x.UmeId == usuario).FirstOrDefault().Id;
                lista = await context.Database
                         .SqlQuery<AgendaSP>("GetListAgenda @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                         .ToListAsync();
                lista = lista.Where(e => e.Agendable & (e.Deleted != true) & e.ResIntId == responsableId)
                             .OrderBy(e => e.Fecha)
                             .ToList();
            }
            return PartialView("ListAgendaSP", lista);
        }

        [ActionAuthorize(Roles = "ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetListAgendaTodayExternos([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {
            List<AgendaSP> lista = new List<AgendaSP>();
            var fechaDesde = DateTime.Parse(model.fechaDesde);
            //var fechaHasta = DateTime.Parse(model.fechaHasta);
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;

            using (var context = new NuevoDbContext())
            {
                int? estudioId = context.Responsables.Where(x => x.UmeId == usuario).FirstOrDefault().Estudio_Id;
                Session["estudioExternoId"] = estudioId;

                lista = await context.Database
                         .SqlQuery<AgendaSP>("GetListAgenda @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaDesde))
                         .ToListAsync();
                lista = lista.Where(e => e.Agendable & (e.Deleted != true) & e.estudioId == estudioId)
                             .OrderBy(e => e.Fecha)
                             .ToList();
            }
            return PartialView("ListAgendaSPExternos", lista);
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuscarListAgenda([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {
            int maxRows = 30;
            List<AgendaSP> lista = new List<AgendaSP>();
            var fechaDesde = model.fechaDesde;
            var fechaHasta = model.fechaHasta;
            PaginadorAgenda paginadorAgenda = new PaginadorAgenda();
            using (var context = new NuevoDbContext())
            {
                if (model.filtrarPorFechaVencimiento)
                {
                    lista = await context.Database
                             .SqlQuery<AgendaSP>("GetListAgenda @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                             .ToListAsync();
                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.Fecha).ToList();
                }

                else
                 if (model.filtrarPorFechaNotificacion)
                {
                    lista = await context.Database
                                .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToListAsync();
                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                }
                else
                 if (model.filtrarPorFechaNotificacionGcia)
                {
                    lista = await context.Database
                                .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToListAsync();
                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                }
                if (model.tipoEventoSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.TipoEventoId == model.tipoEventoSeleccionado).ToList();
                }
                if (model.organismoSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.OrganismoId == model.organismoSeleccionado).ToList();
                }
                if (model.reqInformeSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.ReqInformeId == model.reqInformeSeleccionado).ToList();
                }
                if (model.solucionadoSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.SolucionadoId == model.solucionadoSeleccionado).ToList();
                }
                if (model.estudioSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.estudioId == model.estudioSeleccionado).ToList();
                }
                if (model.provinciaSeleccionada > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.ProvinciaId == model.provinciaSeleccionada).ToList();
                }
                if (model.responsableSeleccionado > 0)
                {
                    lista = lista.Where(e => e.ResIntId == model.responsableSeleccionado).ToList();
                }
                if (model.idDenuncia != null)
                {
                    lista = lista.Where(e => e.DenunciaId == model.idDenuncia).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.dniDenunciante))
                {
                    //ICollection<object> bools= new List<object>();
                    //foreach (var item in lista) {
                    //    var x = (item.dniDenunciante.Equals(model.dniDenunciante));
                    //    bools.Add(x);
                    //} 
                    //lista = lista.Where(e => e.dniDenunciante == (Convert.ToInt32(model.dniDenunciante))).ToList();
                    lista = lista.Where(e => String.Equals(e.dniDenunciante, model.dniDenunciante)).ToList();
                }
                if (model.contestado)
                {
                    //lista = lista.Where(e => e.Contestado == model.contestado).ToList();
                    lista = lista.Where(e => e.Contestado == 1).ToList();
                }
                if (model.esUnCambioMasivo)
                {
                    return PartialView("ListAgendaMM", lista);
                }

               

                paginadorAgenda.listadoAgenda = lista
                             .Skip((model.CurrentPageIndex - 1) * maxRows)
                             .Take(maxRows).ToList();
                paginadorAgenda.totalDeRegistros = lista.Count();
            }
            double pageCount = (double)((decimal)lista.Count() / Convert.ToDecimal(maxRows));

            paginadorAgenda.PageCount = (int)Math.Ceiling(pageCount);

            paginadorAgenda.CurrentPageIndex = model.CurrentPageIndex;


            return PartialView("ListAgendaPaginada", paginadorAgenda);

        }

        [ActionAuthorize(Roles = "ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuscarListAgendaExternos([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {
            int maxRows = 30;
            List<AgendaSP> lista = new List<AgendaSP>();
            var fechaDesde = model.fechaDesde;
            var fechaHasta = model.fechaHasta;
            PaginadorAgenda paginadorAgenda = new PaginadorAgenda();
            using (var context = new NuevoDbContext())
            {
                if (model.filtrarPorFechaVencimiento)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = await context.Database
                             .SqlQuery<AgendaSP>("GetListAgendaDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                             .ToListAsync();

                    }
                    else
                   if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = await context.Database
                            .SqlQuery<AgendaSP>("GetListAgendaHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                            .ToListAsync();

                    }
                    else
                    {
                        lista = await context.Database
                             .SqlQuery<AgendaSP>("GetListAgenda @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                             .ToListAsync();

                    }
                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.Fecha).ToList();
                    //lista = await context.Database
                    //         .SqlQuery<AgendaSP>("GetListAgenda @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //         .ToListAsync();
                    //lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.Fecha).ToList();
                }

                else
                 if (model.filtrarPorFechaNotificacion)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = await context.Database
                               .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                               .ToListAsync();
                    }
                    else
                   if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = await context.Database
                                  .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                                  .ToListAsync();
                    }
                    else
                    {
                        lista = await context.Database
                               .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                               .ToListAsync();
                    }


                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                    //lista = await context.Database
                    //            .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //            .ToListAsync();
                    //lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                }
                else
                 if (model.filtrarPorFechaNotificacionGcia)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = await context.Database
                                .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionGciaDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                                .ToListAsync();
                    }
                    else
                   if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = await context.Database
                                .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionGciaHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                                .ToListAsync();
                    }
                    else
                    {
                        lista = await context.Database
                                .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToListAsync();
                    }
                    lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                    //lista = await context.Database
                    //            .SqlQuery<AgendaSP>("GetEventosPorFechasNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //            .ToListAsync();
                    //lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.DenunciaId).ToList();
                }
                if (model.tipoEventoSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.TipoEventoId == model.tipoEventoSeleccionado).ToList();
                }
                //if (model.organismoSeleccionado > 0/*!= null*/)
                //{
                //    lista = lista.Where(e => e.OrganismoId == model.organismoSeleccionado).ToList();
                //}
                if (model.reqInformeSeleccionado > 0/*!= null*/)
                {
                    lista = lista.Where(e => e.ReqInformeId == model.reqInformeSeleccionado).ToList();
                }
                //if (model.solucionadoSeleccionado > 0/*!= null*/)
                //{
                //    lista = lista.Where(e => e.SolucionadoId == model.solucionadoSeleccionado).ToList();
                //}
                //if (model.estudioSeleccionado > 0/*!= null*/)
                //{
                //    lista = lista.Where(e => e.estudioId == model.estudioSeleccionado).ToList();
                //}
                //if (model.provinciaSeleccionada > 0/*!= null*/)
                //{
                //    lista = lista.Where(e => e.ProvinciaId == model.provinciaSeleccionada).ToList();
                //}
                //if (model.responsableSeleccionado > 0)
                //{
                //    lista = lista.Where(e => e.ResIntId == model.responsableSeleccionado).ToList();
                //}
                //if (model.idDenuncia != null)
                //{
                //    lista = lista.Where(e => e.DenunciaId == model.idDenuncia).ToList();
                //}
                //if (!String.IsNullOrWhiteSpace(model.dniDenunciante))
                //{

                //    lista = lista.Where(e => String.Equals(e.dniDenunciante, model.dniDenunciante)).ToList();
                //}
                //if (model.contestado)
                //{
                //    lista = lista.Where(e => e.Contestado == 1).ToList();
                //}

                //if (String.Equals((string)Session["userRol"], "ESTUDIO"))
                //{
                var estudioIdSession = (int?)Session["estudioExternoId"];
                if (estudioIdSession != null)
                {
                    var estudioId = (int)Session["estudioExternoId"];
                    lista = lista.Where(x => x.estudioId == estudioId).ToList();


                }
                else
                {
                    return PartialView("ExternoSinEstudio");
                }

                //}

                paginadorAgenda.listadoAgenda = lista
                             .Skip((model.CurrentPageIndex - 1) * maxRows)
                             .Take(maxRows).ToList();
                paginadorAgenda.totalDeRegistros = lista.Count();
            }
            double pageCount = (double)((decimal)lista.Count() / Convert.ToDecimal(maxRows));

            paginadorAgenda.PageCount = (int)Math.Ceiling(pageCount);

            paginadorAgenda.CurrentPageIndex = model.CurrentPageIndex;


            return PartialView("ListAgendaPaginadaExternos", paginadorAgenda);

        }

        public ActionResult ExcelVacio()
        {
            return Json("No hay datos para exportar");
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportarListAgenda([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {

            if (HttpContext.User.Identity.Name == string.Empty)
            {
                return RedirectToAction("SesionCulminada", "Home", null);
            }
            List<AgendaExcelSP> lista = new List<AgendaExcelSP>();
            var fechaDesde = model.fechaDesde;
            var fechaHasta = model.fechaHasta;
            //var mes = (DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            //var dia = (DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            var fileName = "Agenda"
                + DateTime.Now.Year.ToString()
                + DateTime.Now.Month.ToString()
                + DateTime.Now.Day.ToString()
                + ".xlsx";

            using (var context = new NuevoDbContext())
            {
                if (model.filtrarPorFechaVencimiento)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = context.Database
                           .SqlQuery<AgendaExcelSP>("GetAgendaExcelDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                           .ToList();
                    }
                    else
                    if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = context.Database
                            .SqlQuery<AgendaExcelSP>("GetAgendaExcelHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                            .ToList();
                    }
                    else
                    {
                        lista = context.Database
                            .SqlQuery<AgendaExcelSP>("GetAgendaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                            .ToList();
                    }
                    //lista = context.Database
                    //         .SqlQuery<AgendaExcelSP>("GetAgendaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //         .ToList();
                }
                //else
                //if (model.filtrarPorFechaDenuncia && String.Equals(fechaDesde, fechaHasta))//cambiar fecha en Sp
                //{
                //    lista = context.Database
                //                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechaDenunciaExcel @fecha", new SqlParameter("@fecha", fechaDesde))
                //                .ToList();
                //}
                //else
                //if (model.filtrarPorFechaDenuncia && !String.Equals(fechaDesde, fechaHasta))
                //{
                //    lista = context.Database
                //                .SqlQuery<AgendaExcelSP>("GetAgendaPorRangoFechaDenunciaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                //                .ToList();
                //}
                else
                 if (model.filtrarPorFechaNotificacion)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionExcelDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                                .ToList();
                    }
                    else
                    if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionExcelHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();

                    }
                    else
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();

                    }
                    //lista = context.Database
                    //            .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //            .ToList();
                }
                else
                 if (model.filtrarPorFechaNotificacionGcia)
                {
                    if (String.IsNullOrEmpty(fechaHasta))
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionGciaExcelDesde @fechaDesde", new SqlParameter("@fechaDesde", fechaDesde))
                                .ToList();
                    }
                    else
                    if (String.IsNullOrEmpty(fechaDesde))
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionGciaExcelHasta @fechaHasta", new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();
                    }
                    else
                    {
                        lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionGciaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();
                    }
                    //lista = context.Database
                    //            .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionGciaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                    //            .ToList();
                }
                if (model.tipoEventoSeleccionado > 0)
                {
                    lista = lista.Where(e => e.TipoEventoId == model.tipoEventoSeleccionado).ToList();
                }
                if (model.organismoSeleccionado > 0)
                {
                    lista = lista.Where(e => e.ORGANISMO_ID == model.organismoSeleccionado).ToList();
                }
                if (model.reqInformeSeleccionado > 0)
                {
                    lista = lista.Where(e => e.REQUERIMIENTOINFORME == model.reqInformeSeleccionado).ToList();
                }
                if (model.solucionadoSeleccionado > 0)
                {
                    lista = lista.Where(e => e.SolucionadoId == model.solucionadoSeleccionado).ToList();
                }
                if (model.estudioSeleccionado > 0)
                {
                    lista = lista.Where(e => e.ESTUDIO_ID == model.estudioSeleccionado).ToList();
                }
                if (model.provinciaSeleccionada > 0)
                {
                    lista = lista.Where(e => e.Provincia_Id == model.provinciaSeleccionada).ToList();
                }
                if (model.responsableSeleccionado > 0)
                {
                    lista = lista.Where(e => e.ResIntId == model.responsableSeleccionado).ToList();
                }
                if (model.idDenuncia != null)
                {
                    lista = lista.Where(e => e.DenunciaId == model.idDenuncia).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.dniDenunciante))
                {

                    //lista = lista.Where(e => e.Dni_Denunciante == (Convert.ToInt32(model.dniDenunciante))).ToList();
                    lista = lista.Where(e => String.Equals(e.Dni_Denunciante.Trim(), model.dniDenunciante.Trim())).ToList();
                }
                if (model.contestado)
                {
                    //lista = lista.Where(e => e.Contestado == model.contestado).ToList();
                    lista = lista.Where(e => e.Contestado == 1).ToList();
                }
                var estudioIdSession = (int?)Session["estudioExternoId"];
                if (estudioIdSession != null)
                {
                    var estudioId = (int)Session["estudioExternoId"];
                    lista = lista.Where(x => x.ESTUDIO_ID == estudioId).ToList();
                }
                else {
                    return RedirectToAction("GetUnauthorizedView", "Home",null);
                }
                lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.Fecha).ToList();
                if (lista.Count > 0)
                {
                    //lista = lista.Where(e => e.Agendable & (e.Deleted != true)).OrderBy(e => e.Fecha).ToList();
                    var listaNueva = from e in lista
                                     select new
                                     {
                                         //e.Fecha,
                                         e.FechaEvento,
                                         e.DenunciaId,
                                         e.Tipo_Evento,
                                         e.Organismo,
                                         e.Denunciante,
                                         e.Dni_Denunciante,
                                         e.Nro_Linea,
                                         e.Servicio,
                                         e.Provincia,
                                         e.Nro_Expediente,
                                         e.TramiteCRM,
                                         e.Estado_Actual,
                                         e.Requerimiento_Informe,
                                         e.Solucionado,
                                         e.Contestado_,
                                         e.Responsable,
                                         e.Estudio,
                                         e.Mediador,
                                         e.Matricula,
                                         e.Domicilio_Mediador
                                         //,
                                         //e.Fecha_Homologacion,
                                         //e.Nro_Gestion_Coprec,
                                         //e.Honorarios_Coprec,
                                         //e.Fecha_Gestion_Honorarios,
                                         //e.Monto_Acordado,
                                         //e.Arancel,
                                         //e.Fecha_Gestion_Arancel,
                                         //e.Agenda_Coprec
                                     };

                    var tabla = LinqQueryToDataTable(listaNueva);

                    var path = Server.MapPath("~/temp");
                    //var fileName = "Spreadsheet.xlsx";

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }
                    DataSet dataSet = new System.Data.DataSet("AgendaSpreadsheet");
                    dataSet.Tables.Add(tabla);
                    string fullPath = Path.Combine(path, fileName);
                    CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);
                    return Json(new { fileName = fileName, errorMessage = "" });

                }
                else
                {
                    return Json(new { fileName = "isEmpty", errorMessage = "" });
                }
            }

            //return Json(new { fileName = fileName, errorMessage = "" });

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpGet]
        [NoCache]
        public ActionResult DownloadSpreadsheet(string file)
        {
            if (Path.IsPathRooted(file))
            {
                throw new ArgumentNullException("El Parámetro contiene caracteres inválidos : " + file);
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


        public static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
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


        public void ExportarListAgenda_old([Bind(Include = "fechaDesde,fechaHasta,soloFechas,estudioSeleccionado,organismoSeleccionado,tipoEventoSeleccionado,reqInformeSeleccionado,solucionadoSeleccionado,provinciaSeleccionada,responsableSeleccionado,idDenuncia,dniDenunciante,contestado,CurrentPageIndex,PageCount,filtrarPorFechaVencimiento,filtrarPorFechaDenuncia,filtrarPorFechaNotificacion,filtrarPorFechaNotificacionGcia,esUnCambioMasivo")]AgendaModelView model)
        {
            List<AgendaExcelSP> lista = new List<AgendaExcelSP>();
            var fechaDesde = model.fechaDesde;
            var fechaHasta = model.fechaHasta;

            using (var context = new NuevoDbContext())
            {
                if (model.filtrarPorFechaVencimiento)
                {
                    lista = context.Database
                             .SqlQuery<AgendaExcelSP>("GetAgendaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                             .ToList();
                }
                else
                if (model.filtrarPorFechaDenuncia && String.Equals(fechaDesde, fechaHasta))//cambiar fecha en Sp
                {
                    lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechaDenunciaExcel @fecha", new SqlParameter("@fecha", fechaDesde))
                                .ToList();
                }
                else
                if (model.filtrarPorFechaDenuncia && !String.Equals(fechaDesde, fechaHasta))
                {
                    lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorRangoFechaDenunciaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();
                }
                else
                 if (model.filtrarPorFechaNotificacion)
                {
                    lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();
                }
                else
                 if (model.filtrarPorFechaNotificacionGcia)
                {
                    lista = context.Database
                                .SqlQuery<AgendaExcelSP>("GetAgendaPorFechasNotificacionGciaExcel @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", fechaDesde), new SqlParameter("@fechaHasta", fechaHasta))
                                .ToList();
                }
                if (model.tipoEventoSeleccionado != null)
                {
                    lista = lista.Where(e => e.TipoEventoId == model.tipoEventoSeleccionado).ToList();
                }
                if (model.organismoSeleccionado != null)
                {
                    lista = lista.Where(e => e.ORGANISMO_ID == model.organismoSeleccionado).ToList();
                }
                if (model.reqInformeSeleccionado != null)
                {
                    lista = lista.Where(e => e.REQUERIMIENTOINFORME == model.reqInformeSeleccionado).ToList();
                }
                if (model.solucionadoSeleccionado != null)
                {
                    lista = lista.Where(e => e.SolucionadoId == model.solucionadoSeleccionado).ToList();
                }
                if (model.estudioSeleccionado != null)
                {
                    lista = lista.Where(e => e.ESTUDIO_ID == model.estudioSeleccionado).ToList();
                }
                if (model.provinciaSeleccionada != null)
                {
                    lista = lista.Where(e => e.Provincia_Id == model.provinciaSeleccionada).ToList();
                }
                if (model.responsableSeleccionado != null && model.responsableSeleccionado > 0)
                {
                    lista = lista.Where(e => e.ResIntId == model.responsableSeleccionado).ToList();
                }
                if (model.idDenuncia != null)
                {
                    lista = lista.Where(e => e.DenunciaId == model.idDenuncia).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.dniDenunciante))
                {

                    //lista = lista.Where(e => e.Dni_Denunciante == (Convert.ToInt32(model.dniDenunciante))).ToList();
                    lista = lista.Where(e => String.Equals(e.Dni_Denunciante.Trim(), model.dniDenunciante.Trim())).ToList();
                }
                if (model.contestado)
                {
                    //lista = lista.Where(e => e.Contestado == model.contestado).ToList();
                    lista = lista.Where(e => e.Contestado == 1).ToList();
                }
                lista = lista.Where(e => e.Agendable).OrderBy(e => e.Fecha).ToList();
                if (lista.Count > 0)
                {

                    var listaNueva = from e in lista
                                     select new
                                     {
                                         e.Fecha,
                                         e.DenunciaId,
                                         e.Tipo_Evento,
                                         e.Organismo,
                                         e.Denunciante,
                                         e.Dni_Denunciante,
                                         e.Nro_Linea,
                                         e.Servicio,
                                         e.Provincia,
                                         e.Nro_Expediente,
                                         e.TramiteCRM,
                                         e.Estado_Actual,
                                         e.Requerimiento_Informe,
                                         e.Solucionado,
                                         e.Contestado_,
                                         e.Responsable,
                                         e.Estudio
                                     };


                    var grid = new System.Web.UI.WebControls.GridView();
                    grid.DataSource = listaNueva;
                    grid.DataBind();

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment; filename=Exported.xls");
                    Response.ContentType = "application/excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Write(sw.ToString());

                    Response.End();
                }
                else
                {
                    RedirectToAction("ExcelVacio");
                }
            }
        }
       

        public ActionResult GetIdDenuncias()
        {

            using (var context = new NuevoDbContext())
            {
                //var ids = context.Denuncias
                //                    .Select(p => p.DenunciaId);
                var ids = from d in context.Denuncias
                          select new
                          {
                              d.DenunciaId,
                              d.DENUNCIANTE_ID,
                              d.CREATIONDATE
                          };


                return PartialView("ListaIdDenuncias", ids.ToList());
            }

        }

        public ActionResult GetEventosDiarios()
        {
            Session.Add("prueba", "UnaPrueba");
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEventosDiariosBis()
        {
            return Json(Session["prueba"], JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEventosDiariosTri()
        {
            return Json(Session["prueba"], JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetOrganismosPorProvincia(int? idProvincia)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var organismos = new List<OrganismoDto>();
                if (idProvincia > 0)
                {
                    organismos = context.Organismos.Where(x => x.Provincia_Id == idProvincia).ToList();
                }
                else
                {
                    organismos = context.Organismos.ToList();
                }

                return Json(organismos);
            }


        }
        
    }
}