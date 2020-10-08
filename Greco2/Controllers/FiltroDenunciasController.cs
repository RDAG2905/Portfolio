//using ClosedXML.Excel;
using Greco2.Excel;
using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Localidad;
using Greco2.Models.Mail;
using Greco2.Models.Organismo;
//using ICSharpCode.SharpZipLib.Zip;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;
using Greco2.Models.Paginacion;
using System.Web.Routing;

namespace Greco2.Controllers
{
    
    public class FiltroDenunciasController : Controller
    {
        // GET: FiltroDenuncias
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
        public ActionResult VerificarToken() {
           
            return JavaScript("descargarZip()");
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
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        

        private List<ListaSabanaSP> aplicarFiltros(FiltroDenunciasModelView filtroModel)
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();


            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorResponsable @RESP_INT_ID", new SqlParameter("@RESP_INT_ID", filtroModel.responsableInterno))
                            .ToList();
                }
                lista = filtrarSinResponsable(filtroModel, lista);
            }
            else
             if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechasDeNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGcia(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechasDeNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacion(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionGciaDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGciaDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionGciaHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGciaHasta(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacionHasta(filtroModel, lista);
            }
            else
            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorNroExpediente @nroExpediente", new SqlParameter("@nroExpediente", filtroModel.nroExpediente))
                            .ToList();
                }
                lista = filtrarSinNroExpediente(filtroModel, lista);
            }
            else
            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEtapaProcesal @etapaProcesalId", new SqlParameter("@etapaProcesalId", filtroModel.etapaProcesalSeleccionada))
                            .ToList();
                }
                lista = filtrarSinEtapaProcesal(filtroModel, lista);
            }
            else
            if (filtroModel.organismoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorOrganismo @organismoId", new SqlParameter("@organismoId", filtroModel.organismoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinOrganismo(filtroModel, lista);
            }
            else
            if (filtroModel.regionSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorRegion @regionId", new SqlParameter("@regionId", filtroModel.regionSeleccionada))
                            .ToList();
                }
                lista = filtrarSinRegion(filtroModel, lista);
            }
            else
            if (filtroModel.provinciaSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorProvincia @provinciaId", new SqlParameter("@provinciaId", filtroModel.provinciaSeleccionada))
                            .ToList();
                }
                lista = filtrarSinProvincia(filtroModel, lista);
            }
            else
            if (filtroModel.localidadSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorLocalidad @localidadId", new SqlParameter("@localidadId", filtroModel.localidadSeleccionada))
                            .ToList();
                }
                lista = filtrarSinLocalidad(filtroModel, lista);
            }
            else
            if (filtroModel.servicioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorServicio @servicioId", new SqlParameter("@servicioId", filtroModel.servicioSeleccionado))
                            .ToList();
                }
                lista = filtrarSinServicio(filtroModel, lista);
            }
            else
            if (filtroModel.idDenuncia != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorIdDenuncia @denunciaId", new SqlParameter("@denunciaId", filtroModel.idDenuncia))
                            .ToList();
                }
                lista = filtrarSinIdDenuncia(filtroModel, lista);
            }
            else
            if (filtroModel.estudioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEstudio @estudioId", new SqlParameter("@estudioId", filtroModel.estudioSeleccionado))
                            .ToList();
                }
                lista = filtrarSinEstudio(filtroModel, lista);
            }
            else // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }
            else
            if (!String.IsNullOrWhiteSpace(filtroModel.apellidoDenunciante))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorApellido @apellido", new SqlParameter("@apellido", filtroModel.apellidoDenunciante))
                            .ToList();
                }
                lista = filtrarSinApellido(filtroModel, lista);
            }
            //else
            //if (filtroModel.nombreDenunciante != null)
            //{ }
            else
            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorDni @dni", new SqlParameter("@dni", filtroModel.dniDenunciante))
                            .ToList();
                }
                lista = filtrarSinNroDocumento(filtroModel, lista);
            }
            else
            if (!String.IsNullOrWhiteSpace(filtroModel.estadoSeleccionado))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEstadoConciliacion @estadoId", new SqlParameter("@estadoId", filtroModel.estadoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinEstadoConciliacion(filtroModel, lista);
            }
            else
            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorNroLinea @nroLinea", new SqlParameter("@nroLinea", filtroModel.nroLinea))
                            .ToList();
                }
                lista = filtrarSinNroLinea(filtroModel, lista);
            }
            else
            if (!String.IsNullOrWhiteSpace(filtroModel.tramiteCRM))// --> va solo? Falta el SP
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorTramiteCRM @tramiteCRM", new SqlParameter("@tramiteCRM", filtroModel.tramiteCRM))
                            .ToList();
                }
                lista = filtrarSinTramiteCRM(filtroModel, lista);

            }

            else
            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorMotivoDeReclamo @motivoReclamo", new SqlParameter("@motivoReclamo", filtroModel.motivoDeReclamoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinMotivoDeReclamo(filtroModel, lista);
            }
            
            return lista;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public async Task<ActionResult> GetDenuncias([Bind(Include = "fechaNotifDesde,fechaNotifGciaDesde,fechaNotifHasta,fechaNotifGciaHasta,nroExpediente,etapaProcesalSeleccionada,organismoSeleccionado,regionSeleccionada,provinciaSeleccionada,localidadSeleccionada,servicioSeleccionado,idDenuncia,estudioSeleccionado,apellidoDenunciante,nombreDenunciante,tipoDocumento,dniDenunciante,estadoSeleccionado,nroLinea,tramiteCRM,motivoDeReclamoSeleccionado,responsableSeleccionado,responsableInterno,idDenunciaOriginal,exportarAExcel,esUnCambioMasivo,incluirDenunciasInactivas,verDenunciasEliminadas,CurrentPageIndex,seAgregaUnEvento")] FiltroDenunciasModelView filtroModel)
        {


            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
           
            
            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechasDeNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacion(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechasDeNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacionGcia(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechaDeNotificacionGciaDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacionGciaDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechaDeNotificacionGciaHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacionHasta(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechaDeNotificacionDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacionDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorFechaDeNotificacionHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToListAsync();
                }
                lista = filtrarSinFechaNotificacionHasta(filtroModel, lista);
            }
            else
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorResponsable @RESP_INT_ID", new SqlParameter("@RESP_INT_ID", filtroModel.responsableInterno))
                            .ToListAsync();
                }
                lista = filtrarSinResponsable(filtroModel, lista);
            }
            else
            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorNroExpediente @nroExpediente", new SqlParameter("@nroExpediente", filtroModel.nroExpediente))
                            .ToListAsync();
                }
                lista = filtrarSinNroExpediente(filtroModel, lista);

            }
            else
            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorEtapaProcesal @etapaProcesalId", new SqlParameter("@etapaProcesalId", filtroModel.etapaProcesalSeleccionada))
                            .ToListAsync();
                }
                lista = filtrarSinEtapaProcesal(filtroModel, lista);
            }
            else
            if (filtroModel.organismoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorOrganismo @organismoId", new SqlParameter("@organismoId", filtroModel.organismoSeleccionado))
                            .ToListAsync();
                }
                lista = filtrarSinOrganismo(filtroModel, lista);
            }
            else
            if (filtroModel.regionSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorRegion @regionId", new SqlParameter("@regionId", filtroModel.regionSeleccionada))
                            .ToListAsync();
                }
                lista = filtrarSinRegion(filtroModel, lista);
            }
            else
            if (filtroModel.provinciaSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorProvincia @provinciaId", new SqlParameter("@provinciaId", filtroModel.provinciaSeleccionada))
                            .ToListAsync();
                }
                lista = filtrarSinProvincia(filtroModel, lista);
            }
            else
            if (filtroModel.localidadSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorLocalidad @localidadId", new SqlParameter("@localidadId", filtroModel.localidadSeleccionada))
                            .ToListAsync();
                }
                lista = filtrarSinLocalidad(filtroModel, lista);
            }
            else
            if (filtroModel.servicioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorServicio @servicioId", new SqlParameter("@servicioId", filtroModel.servicioSeleccionado))
                            .ToListAsync();
                }
                lista = filtrarSinServicio(filtroModel, lista);
            }
            else
            if (filtroModel.idDenuncia != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorIdDenuncia @denunciaId", new SqlParameter("@denunciaId", filtroModel.idDenuncia))
                            .ToListAsync();
                }
                lista = filtrarSinIdDenuncia(filtroModel, lista);
            }
            else
            if (filtroModel.estudioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorEstudio @estudioId", new SqlParameter("@estudioId", filtroModel.estudioSeleccionado))
                            .ToListAsync();
                }
                lista = filtrarSinEstudio(filtroModel, lista);
            }
            
            else
            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorApellido @apellido", new SqlParameter("@apellido", filtroModel.apellidoDenunciante))
                            .ToListAsync();
                }
                lista = filtrarSinApellido(filtroModel, lista);
            }
            else
            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorNombreDenunciante @nombre", new SqlParameter("@nombre", filtroModel.nombreDenunciante))
                            .ToListAsync();
                }
                lista = filtrarSinNombre(filtroModel, lista);
            }
            else
            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    var dni = filtroModel.dniDenunciante.ToString();
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorDni @dni", new SqlParameter("@dni",dni))
                            .ToListAsync();
                }
                lista = filtrarSinNroDocumento(filtroModel, lista);
            }
            else
            if (filtroModel.estadoSeleccionado != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorEstadoConciliacion @estado", new SqlParameter("@estado", filtroModel.estadoSeleccionado))
                            .ToListAsync();
                }
                lista = filtrarSinEstadoConciliacion(filtroModel, lista);
            }
            else
            if (filtroModel.nroLinea != null)// --> 
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorNroLinea @nroLinea", new SqlParameter("@nroLinea", filtroModel.nroLinea))
                            .ToListAsync();
                }
                lista = filtrarSinNroLinea(filtroModel, lista);
            }
            else
            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))// --> 
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorTramiteCRM @tramiteCRM", new SqlParameter("@tramiteCRM", filtroModel.tramiteCRM))
                            .ToListAsync();
                }
                lista = filtrarSinTramiteCRM(filtroModel, lista);

            }


            else
            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = await context.Database
                            .SqlQuery<ListaSabanaSP>("GetDenunciasPorMotivoDeReclamo @motivoReclamo", new SqlParameter("@motivoReclamo", filtroModel.motivoDeReclamoSeleccionado))
                            .ToListAsync();
                }
                lista = filtrarSinMotivoDeReclamo(filtroModel, lista);
            }
            if (!filtroModel.incluirDenunciasInactivas) {
                lista = lista.Where(x => !x.INACTIVO == true).ToList();
            }
            if (filtroModel.verDenunciasEliminadas)
            {
                lista = lista.Where(x => x.DELETED == true).ToList();
            }
            else
            {
                lista = lista.Where(x => x.DELETED != true).ToList();
            }
            if (filtroModel.esUnCambioMasivo) {
                return PartialView("ListSabanaMM", lista);
            }
            
            if (String.Equals((string)Session["userRol"], "ESTUDIO"))
            {
                var estudioIdSession = (int?)Session["estudioExternoId"];
                if (estudioIdSession != null) { 
                    var estudioId = (int)Session["estudioExternoId"];
                    lista = lista.Where(x => x.ESTUDIO_ID == estudioId).ToList();
                    if (filtroModel.seAgregaUnEvento)
                    {
                        var totalDeReg = lista.Count();
                        var cantidadRegPorPagina = 15;
                        var cantidadPag = (int)Math.Ceiling((double)totalDeReg / cantidadRegPorPagina);
                        lista = lista.OrderBy(x => x.DenunciaId)
                                 .Skip((filtroModel.CurrentPageIndex - 1) * cantidadRegPorPagina)
                                .Take(cantidadRegPorPagina).ToList();

                        var modelEx = new ResultadoDenuncias();
                        modelEx.listaDenuncias = lista;
                        modelEx.PaginaActual = filtroModel.CurrentPageIndex;
                        modelEx.TotalDeRegistros = totalDeReg;
                        modelEx.RegistrosPorPagina = cantidadRegPorPagina;
                        modelEx.TotalDePaginas = cantidadPag;
                        modelEx.ValoresQueryString = new RouteValueDictionary();
                        return PartialView("ListSabanaNuevoEvento", modelEx);
                    }
                    else {
                        var totalDeReg = lista.Count();
                        var cantidadRegPorPagina = 15;
                        var cantidadPag = (int)Math.Ceiling((double)totalDeReg / cantidadRegPorPagina);
                        lista = lista.OrderBy(x => x.DenunciaId)
                                 .Skip((filtroModel.CurrentPageIndex - 1) * cantidadRegPorPagina)
                                .Take(cantidadRegPorPagina).ToList();

                        var modelEx = new ResultadoDenuncias();
                        modelEx.listaDenuncias = lista;
                        modelEx.PaginaActual = filtroModel.CurrentPageIndex;
                        modelEx.TotalDeRegistros = totalDeReg;
                        modelEx.RegistrosPorPagina = cantidadRegPorPagina;
                        modelEx.TotalDePaginas = cantidadPag;
                        modelEx.ValoresQueryString = new RouteValueDictionary();

                        return PartialView("ListSabanaExternos", modelEx);
                    }
                    
                }
                else {

                    return PartialView("ExternoSinEstudio");
                }
                
                
            }
            var totalDeRegistros = lista.Count();
            var cantidadRegistrosPorPagina = 15;
            var cantidadPaginas = (int)Math.Ceiling((double)totalDeRegistros / cantidadRegistrosPorPagina);
            lista = lista.OrderByDescending(x=>x.Fecha_Creacion)
                     .Skip((filtroModel.CurrentPageIndex - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
            
            var model = new ResultadoDenuncias();
            model.listaDenuncias = lista;
            model.PaginaActual = filtroModel.CurrentPageIndex ;
            model.TotalDeRegistros = totalDeRegistros;
            model.RegistrosPorPagina = cantidadRegistrosPorPagina;
            model.TotalDePaginas = cantidadPaginas;
            model.ValoresQueryString = new RouteValueDictionary();

            if (filtroModel.seAgregaUnEvento) {
                return PartialView("ListSabanaNuevoEvento", model);
            }
            return PartialView("ListSabanaSP",model);
           

        }

        private List<ListaSabanaSP> filtrarSinNroExpediente(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {   
                lista = lista.Where(e =>e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null) {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            //if (filtroModel.nroExpediente != null)
            //{
            //    lista = lista.Where(e => e.Expediente.Contains(filtroModel.nroExpediente)).ToList();
            //}

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();        
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }
            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }


            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString().ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinEtapaProcesal(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }


            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinOrganismo(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            //if (filtroModel.organismoSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            //}

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            //if (filtroModel.tramiteCRM != null)
            //{
            //    lista = lista.Where(e =>e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            //}

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }


            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinRegion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            //if (filtroModel.regionSeleccionada > 0)
            //{
            //    lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            //}

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinProvincia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }
            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }


            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinLocalidad(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            //if (filtroModel.localidadSeleccionada > 0)
            //{
            //    lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            //}

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
               lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinServicio(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            //if (filtroModel.servicioSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            //}

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinIdDenuncia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            //if (filtroModel.idDenuncia != null)
            //{
            //    lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            //}

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinEstudio(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            //if (filtroModel.estudioSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            //}
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinApellido(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            //if (filtroModel.apellidoDenunciante != null)
            //{
            //    lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            //}

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNombre(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNroDocumento(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinEstadoConciliacion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            //if (filtroModel.estadoSeleccionado != null)
            //{
            //    lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            //}

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNroLinea(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            //if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            //{
            //    lista = lista.Where(e => e.nroLinea == filtroModel.nroLinea.ToString()).ToList();
            //}

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinTramiteCRM(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            //if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            //{
            //    lista = lista.Where(e =>e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            //}

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinMotivoDeReclamo(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if(filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            //if (filtroModel.motivoDeReclamoSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            //}
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinResponsable(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
           
            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion.Date.ToString().Equals(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion.Date.ToString().Equals(filtroModel.fechaNotifHasta)).ToList();
                //lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                //                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinFechaNotificacionDesde(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
            //                         && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            //}
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }
            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionHasta(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
            //                         && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            //if (filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGciaDesde(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            //if (filtroModel.fechaNotifGciaDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            //}

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGciaHasta(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            //if (filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

           
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia.Date >= DateTime.Parse(filtroModel.fechaNotifGciaDesde).Date
                                     && e.Fecha_Notificacion_Gcia.Date <= DateTime.Parse(filtroModel.fechaNotifGciaHasta).Date).ToList();
                //lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                //                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }
            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            //if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            //{
            //    lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante)).ToList();
            //}
            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }


            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGcia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            //if (filtroModel.fechaNotifGciaDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            //}

            //if (filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (!String.IsNullOrEmpty(filtroModel.nroExpediente))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Expediente)).ToList();
                lista = lista.Where(e => e.Expediente.Trim().ToUpper().Contains(filtroModel.nroExpediente.Trim().ToUpper())).ToList();

            }

            if (filtroModel.etapaProcesalSeleccionada  > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            
            if (!String.IsNullOrEmpty(filtroModel.apellidoDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Apellido)).ToList();
                lista = lista.Where(e => e.Apellido.Trim().ToUpper().Contains(filtroModel.apellidoDenunciante.Trim().ToUpper())).ToList();

            }

            //if (filtroModel.nombreDenunciante != null)
            //{
            //    lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            //}
            if (!String.IsNullOrEmpty(filtroModel.nombreDenunciante))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nombre)).ToList();
                lista = lista.Where(e => e.Nombre.Trim().ToUpper().Contains(filtroModel.nombreDenunciante.Trim().ToUpper())).ToList();

            }

            if (filtroModel.dniDenunciante != null)
            {
                var dni = filtroModel.dniDenunciante.ToString();
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Nro_Documento)).ToList();
                lista = lista.Where(e => e.Nro_Documento.Trim().Contains(dni.Trim())).ToList();

            }

            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (!String.IsNullOrEmpty(filtroModel.tramiteCRM))
            {
                lista = lista.Where(e => !String.IsNullOrEmpty(e.Tramite_CRM)).ToList();
                lista = lista.Where(e => e.Tramite_CRM.Trim().ToUpper().Contains(filtroModel.tramiteCRM.Trim().ToUpper())).ToList();

            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        [HttpPost]
        public JsonResult GetLocalidadesPorProvincia(int idProvincia)
        {
            try
            {
                var localidades = new List<LocalidadDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    localidades = context.Localidades.Where(m => m.ProvinciaId == idProvincia).OrderBy(m=>m.Nombre).ToList();
                }
                return Json(localidades);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [HttpPost]
        public JsonResult GetOrganismosPorProvincia(int idProvincia)
        {
            try
            {
                var organismos = new List<OrganismoDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    organismos = context.Organismos.Where(m => m.Provincia_Id == idProvincia).OrderBy(m => m.Nombre).ToList();
                }
                return Json(organismos);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ViewResult FiltroDenuncias()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View(new FiltroDenunciasModelView());
        }

        [ActionAuthorize(Roles = "ESTUDIO")]
        public ViewResult FiltroDenunciasExternos()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View("FiltroDenunciasExternos", new FiltroDenunciasModelView());
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [ValidateAntiForgeryToken]
        public JsonResult SendMail()
        {
            GenerarExcelZipActivas();

            List<string> archivos = new List<string>();
            archivos.Add((string)Session["PathReporteSabanaZip"]);
            //archivos.Add((string)Session["PathListaSabanaZip"]);

            var emailTEAM = (string)Session["emailTEAM"];
            var destinatarios = (List<EmailAdressDto>)Session["destinatarios"];

            foreach (var item in destinatarios) {
                Mail correo = new Mail(emailTEAM,item.EmailAdress, "Reporte de Denuncias TEAM", "Reporte por Correo", archivos);
                correo.enviaMail();

            }
            //Mail correo = new Mail("team@teco.com.ar", "RDAlvarezGonzalez@teco.com.ar", "Envio de Prueba", "Reporte de Denuncias", archivos); MaAQuiroga @teco.com.ar
            //Mail correo = new Mail("team@teco.com.ar", "MaAQuiroga@teco.com.ar", "Envio de Prueba", "Reporte por Correo", archivos); 
            //correo.enviaMail();
            //Mail correo2 = new Mail("team@teco.com.ar", "MMAguirreZanardi@teco.com.ar", "Envio de Prueba", "Reporte por Correo", archivos);
            //correo2.enviaMail();
            //Mail correo3 = new Mail("team@teco.com.ar", "RDAlvarezGonzalez@teco.com.ar", "Envio de Prueba", "Reporte por Correo", archivos);
            //correo3.enviaMail();
            return Json("Correo enviado");
        }



        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public JsonResult GenerarListaSabanaExcel()
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = context.Database
                        .SqlQuery<ListaSabanaSP>("ExcelGetListaSabana")
                        .ToList();
            }

            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                     e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                     e.Nro_Documento,
                                     e.Nro_Linea,
                                     e.Tramite_CRM,
                                     e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                     e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Mediador,
                                     e.Matricula,
                                     e.Domicilio_Mediador,
                                     e.Fecha_Homologacion,
                                     e.Nro_Gestion_Coprec,
                                     e.Honorarios_Coprec,
                                     e.Fecha_Gestion_Honorarios,
                                     e.Monto_Acordado,
                                     e.Arancel,
                                     e.Fecha_Gestion_Arancel,
                                     e.Agenda_Coprec,
                                     e.Denuncia_Preventiva,
                                     e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";
                

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("Denuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);
                
                Session["PathListaSabanaZip"] = fullPath;
                Session["PathListaSabana"] = fullPath;

                CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

                return Json(new { fileName = fileName, errorMessage = "" });
            }
            else
            {
                return Json(new { fileName = "isEmpty", errorMessage = "" });
            }
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
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

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult EliminarReporteDenuncias()
        {

            string fullpath = (string)Session["pathReporteDenunciasAEliminar"];
            System.IO.File.Delete(fullpath);
            return Json("Se ha eliminado archivo temporal",JsonRequestBehavior.AllowGet);
        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public void GenerarExcelZipActivas()
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            //List<ListaSabanaSP> activos = new List<ListaSabanaSP>();
            //List<ListaSabanaSP> inactivos = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = context.Database
                        .SqlQuery<ListaSabanaSP>("ExcelGetListaSabana")
                        .ToList();
                lista = lista.Where(x => x.DELETED != true).ToList();
                lista = lista.Where(x => x.INACTIVO != true).ToList();
                //lista = lista.Where(x=>x.INACTIVO == null || x.INACTIVO == false).ToList();
                
            }

            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                     e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                     e.Nro_Documento,
                                     e.Nro_Linea,
                                     e.Tramite_CRM,
                                     e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                     e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Mediador,
                                     e.Matricula,
                                     e.Domicilio_Mediador,
                                     e.Fecha_Homologacion,
                                     e.Nro_Gestion_Coprec,
                                     e.Honorarios_Coprec,
                                     e.Fecha_Gestion_Honorarios,
                                     e.Monto_Acordado,
                                     e.Arancel,
                                     e.Fecha_Gestion_Arancel,
                                     e.Agenda_Coprec,
                                     e.Denuncia_Preventiva,
                                     e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";

                
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("Denuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);

                //Session["PathListaSabanaZip"] = fullPath; Versión anterior

                CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

                //inicio seccion agregada para el zip
                using (ZipFile zipfile = new ZipFile())
                {
                    var file = fullPath;
                    var archivo = Path.Combine(Server.MapPath("~/temp"), file);
                    var archivoNombre = Path.GetFileName(archivo);
                    var archivoBytes = System.IO.File.ReadAllBytes(archivo);
                    zipfile.AddEntry(archivoNombre, archivoBytes);
                    

                    var nombreZip = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".zip";
                    string pathZip = Path.Combine(path, nombreZip);
                    zipfile.Save(pathZip);
                    
                    Session["PathReporteSabanaZip"] = pathZip;
                }
                
                //fin seccion agregada para el zip

            }
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public void GenerarExcelZip()
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = context.Database
                        .SqlQuery<ListaSabanaSP>("ExcelGetListaSabana")
                        .ToList();
            }

            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                     e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                     e.Nro_Documento,
                                     e.Nro_Linea,
                                     e.Tramite_CRM,
                                     e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                     e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Mediador,
                                     e.Matricula,
                                     e.Domicilio_Mediador,
                                     e.Fecha_Homologacion,
                                     e.Nro_Gestion_Coprec,
                                     e.Honorarios_Coprec,
                                     e.Fecha_Gestion_Honorarios,
                                     e.Monto_Acordado,
                                     e.Arancel,
                                     e.Fecha_Gestion_Arancel,
                                     e.Agenda_Coprec,
                                     e.Denuncia_Preventiva,
                                     e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";


                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("Denuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);

                Session["PathListaSabanaZip"] = fullPath;
                
                CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

            
            }
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public FileResult DescargaZip()
        {
            GenerarExcelZip();
            using (ZipFile zipfile = new ZipFile())
            {
                var file = (string)Session["PathListaSabanaZip"];
                var archivo = Path.Combine(Server.MapPath("~/temp"), file);
                var archivoNombre = Path.GetFileName(archivo);
                var archivoBytes = System.IO.File.ReadAllBytes(archivo);
                zipfile.AddEntry(archivoNombre, archivoBytes);
                var nombreZip = "ReporteDenuncias.zip";
                using (MemoryStream output = new MemoryStream())
                {
                    zipfile.Save(output);
                    return File(output.ToArray(), "application/zip",nombreZip);
                }
            }

           
            
        }


        private DataTable Table()
        {
            // Sample dataset from https://www.dotnetperls.com/dataset and I added more entries

            //DataTable table = new DataTable("Prescription");
            //table.Columns.Add("Dosage", typeof(int));
            //table.Columns.Add("Drug", typeof(string));
            //table.Columns.Add("Patient", typeof(string));
            //table.Columns.Add("Date", typeof(DateTime));

            //table.Rows.Add(25, "Indocin", "David", DateTime.Parse("01/01/2001"));
            //table.Rows.Add(50, "Enebrel", "Sam", DateTime.Parse("01/01/2002"));
            //table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Parse("01/01/2003"));
            //table.Rows.Add(21, "Combivent", "Janet", DateTime.Parse("01/01/2004"));
            //table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Parse("01/01/2005"));

            //table.Rows.Add(50, "Meloxicam", "Nobert", DateTime.Parse("02/01/2001"));
            //table.Rows.Add(25, "Clonazepam", "Noah", DateTime.Parse("03/01/2002"));
            //table.Rows.Add(150, "Metformin", "Liam", DateTime.Parse("04/01/2003"));
            //table.Rows.Add(300, "Cyclobenzaprine", "Mason", DateTime.Parse("05/01/2004"));
            //table.Rows.Add(200, "Doxycycline", "Jacob", DateTime.Parse("06/01/2005"));

            //table.Rows.Add(250, "Gabapentin", "William", DateTime.Parse("01/02/2001"));
            //table.Rows.Add(125, "Hydrochlorothiazide", "Linda", DateTime.Parse("01/03/2002"));
            //table.Rows.Add(10, "Ibuprofen", "Ethan", DateTime.Parse("01/04/2003"));
            //table.Rows.Add(15, "Lexapro", "James", DateTime.Parse("01/05/2004"));
            //table.Rows.Add(50, "Lorazepam", "Alexander", DateTime.Parse("01/06/2005"));

            using (NuevoDbContext context = new NuevoDbContext())
            {

                var lista = context.Database
                        .SqlQuery<ListaSabanaSP>("GetListaSabana")
                        .ToList();
                return this.ToDataTable(lista);
            }
            //return table;
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