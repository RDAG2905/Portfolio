using Greco2.Model;
using Greco2.Models.Estado;
using Greco2.Models.Estudios;
using Greco2.Models.Localidad;
using Greco2.Models.MotivoDeReclamo;
using Greco2.Models.Organismo;
using Greco2.Models.Provincia;
using Greco2.Models.Region;
using Greco2.Models.Responsables;
using Greco2.Models.Servicio;
using Greco2.Models.TipoProceso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Denuncia
{
    public class FiltroDenunciasModelView
    {
        public FiltroDenunciasModelView()
        {
            var estudios = new List<EstudioDto>();
            var organismos = new List<OrganismoDto>();
            var regiones = new List<RegionDto>();
            var provincias = new List<ProvinciaDto>();
            var localidades = new List<LocalidadDto>();
            var servicios = new List<ServicioDto>();
            var motivosDeReclamo = new List<MotivoReclamoServicioTipoProceso>();
            var etapasProcesales = new List<EstadoDto>();
            var estadosConciliacion = new List<string>();
            var responsables = new List<ResponsableDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                estudios = context.Estudios.OrderBy(e => e.Nombre).ToList();

                organismos = context.Database.SqlQuery<OrganismoDto>("select * from tOrganismos order by nombre")
                                                 .ToList();
                /*organismos = context.Organismos.Where(o => !o.Activo).OrderBy(o => o.Nombre).ToList();*///descomentar
                regiones = context.Regiones.OrderBy(o => o.Nombre).ToList();
                provincias = context.Provincias.OrderBy(p => p.Nombre).ToList();
                localidades = context.Localidades.OrderBy(p => p.Nombre).ToList();
                servicios = context.Servicios.OrderBy(p => p.Nombre).ToList();

                motivosDeReclamo = context.Database.SqlQuery<MotivoReclamoServicioTipoProceso>("JoinTipoProcesoServicioMotivoReclamo")
                                                 .ToList();
                estadosConciliacion = context.Database.SqlQuery<string>("select distinct nombre from tSubEstados order by nombre")
                                                 .ToList();

                etapasProcesales = context.Estados.OrderBy(p => p.TipoEstado).ToList();
                responsables = context.Responsables.Where(p => String.Equals(p.TipoResponsable, "RI")).OrderBy(p => p.Apellido).ToList();
                //estudios = context.Estudios.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();

                //organismos = context.Database.SqlQuery<OrganismoDto>("select * from tOrganismos where Activo=0 order by nombre")
                //                                 .ToList();
                ///*organismos = context.Organismos.Where(o => !o.Activo).OrderBy(o => o.Nombre).ToList();*///descomentar
                //regiones = context.Regiones.Where(o => !o.Deleted).OrderBy(o => o.Nombre).ToList();
                //provincias = context.Provincias.Where(p => !p.Deleted).OrderBy(p => p.Nombre).ToList();
                //localidades = context.Localidades.Where(P => !P.Deleted).OrderBy(p => p.Nombre).ToList();
                //servicios = context.Servicios.Where(p => !p.Deleted).OrderBy(p => p.Nombre).ToList();

                //motivosDeReclamo = context.Database.SqlQuery<MotivoReclamoServicioTipoProceso>("JoinTipoProcesoServicioMotivoReclamo")
                //                                 .ToList();
                //estadosConciliacion = context.Database.SqlQuery<string>("select distinct nombre from tSubEstados where Deleted=0 order by nombre")
                //                                 .ToList();

                //etapasProcesales = context.Estados.OrderBy(p => p.TipoEstado).ToList();
                //responsables = context.Responsables.Where(p => String.Equals(p.TipoResponsable, "RI") & !p.Deleted  ).OrderBy(p => p.Apellido).ToList();
            }
            this.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });//descomentar
            this.Regiones = regiones.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Provincias = provincias.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Localidades = localidades.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Servicios = servicios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.MotivosDeReclamo = motivosDeReclamo.Select(p => new SelectListItem { Text = p.NombreCompuesto, Value = p.Id.ToString() });
            //this.estadosDeConciliacion = subEstados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.estadosDeConciliacion = estadosConciliacion.Select(p => new SelectListItem { Text = p });
            this.etapasProcesales = etapasProcesales.Select(p => new SelectListItem { Text = p.TipoEstado, Value = p.Id.ToString()/*,Disabled = (p.Nombre != "Conciliación")*/ });
            //this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId +')', Value = p.Id.ToString()});
            this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre , Value = p.Id.ToString() });
        }


        public IEnumerable<SelectListItem> Organismos { get; set; }
        public IEnumerable<SelectListItem> Provincias { get; set; }
        public IEnumerable<SelectListItem> Localidades { get; set; }
        
        public string DenuncianteNombre { get; set; }

        public IEnumerable<SelectListItem> estadosDeConciliacion { get; set; }
        public string fechaHasta { get; set; }
        public IEnumerable<SelectListItem> etapasProcesales { get; set; }
        public IEnumerable<SelectListItem> Regiones { get; set; }
        public IEnumerable<SelectListItem> Servicios { get; set; }
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public IEnumerable<SelectListItem> TiposDeProceso { get; set; }
        public IEnumerable<SelectListItem> MotivosDeReclamo { get; set; }
        public IEnumerable<SelectListItem> Responsables { get; set; }
        //public IEnumerable<SelectListItem> SubEstados { get; set; }
        public string fechaNotifDesde { get; set; }
        public string fechaNotifGciaDesde { get; set; }
        public string fechaNotifHasta { get; set; }
        public string fechaNotifGciaHasta { get; set; }
        public string nroExpediente { get; set; }
        public int? etapaProcesalSeleccionada { get; set; }
        public int? organismoSeleccionado { get; set; }
        public int? regionSeleccionada { get; set; }
        public int? provinciaSeleccionada { get; set; }
        public int? localidadSeleccionada { get; set; }
        public int? servicioSeleccionado { get; set; }
        public int? idDenuncia { get; set; }
        public int? estudioSeleccionado { get; set; }
        public string apellidoDenunciante { get; set; }
        public string nombreDenunciante { get; set; }
        public int? tipoDocumento { get; set; }
        public int? dniDenunciante { get; set; }
        //public int? estadoSeleccionado { get; set; }
        public string estadoSeleccionado { get; set; }
        public int? nroLinea { get; set; }
        public string tramiteCRM { get; set; }
        public int? motivoDeReclamoSeleccionado { get; set; }
        public int? responsableSeleccionado { get; set; }
        public int? responsableInterno { get; set; }
        public int? idDenunciaOriginal { get; set; }

        public List<DenunciaDto> lista { get; set; }
        public bool exportarAExcel { get; set; }
        public bool esUnCambioMasivo { get; set; }
        public bool incluirDenunciasInactivas { get; set; }
        public bool verDenunciasEliminadas { get; set; }
        public int CurrentPageIndex { get; set; }
        public bool seAgregaUnEvento { get; set; }
    }
}