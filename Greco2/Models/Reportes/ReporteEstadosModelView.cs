using Greco2.Model;
using Greco2.Models.Estudios;
using Greco2.Models.Organismo;
using Greco2.Models.Provincia;
using Greco2.Models.Responsables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Reportes
{
    public class ReporteEstadosModelView
    {
        public ReporteEstadosModelView()
        {
            var estudios = new List<EstudioDto>();
            var organismos = new List<OrganismoDto>();
            var provincias = new List<ProvinciaDto>();
            var estadosConciliacion = new List<string>();
            var responsables = new List<ResponsableDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                responsables = context.Responsables.Where(p => String.Equals(p.TipoResponsable, "RI")).OrderBy(p => p.Apellido).ToList();
                estudios = context.Estudios.OrderBy(e => e.Nombre).ToList();
                organismos = context.Organismos.OrderBy(o => o.Nombre).ToList();
                provincias = context.Provincias.OrderBy(p => p.Nombre).ToList();
                estadosConciliacion = context
                                            .Database
                                            .SqlQuery<string>("select distinct nombre from tSubEstados order by nombre")
                                            .ToList();
                //responsables = context.Responsables.Where(p => String.Equals(p.TipoResponsable, "RI") & !p.Deleted).OrderBy(p => p.Apellido).ToList();
                //estudios = context.Estudios.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();
                //organismos = context.Organismos.Where(o => !o.Activo).OrderBy(o => o.Nombre).ToList();
                //provincias = context.Provincias.Where(p => !p.Deleted).OrderBy(p => p.Nombre).ToList();
                //estadosConciliacion = context
                //                            .Database
                //                            .SqlQuery<string>("select distinct nombre from tSubEstados where Deleted=0 order by nombre")
                //                            .ToList();
            }
            this.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Provincias = provincias.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.estadosDeConciliacion = estadosConciliacion.Select(p => new SelectListItem { Text = p });
            this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre , Value = p.Id.ToString() });
            //this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId + ')', Value = p.Id.ToString() });
        }
        public IEnumerable<SelectListItem> Organismos { get; set; }
        public IEnumerable<SelectListItem> Provincias { get; set; }
        public IEnumerable<SelectListItem> estadosDeConciliacion { get; set; }        
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public IEnumerable<SelectListItem> Responsables { get; set; }

        public string fechaNotifDesde { get; set; }
        public string fechaNotifHasta { get; set; }
        public string fechaNotifGciaDesde { get; set; }
        public string fechaNotifGciaHasta { get; set; }
        public int? organismoSeleccionado { get; set; }
        public int? provinciaSeleccionada { get; set; }
        public int? estudioSeleccionado { get; set; }
        public int? responsableSeleccionado { get; set; }
        public string estadoInicialSeleccionado { get; set; }
        public string estadoFinalSeleccionado { get; set; }
    }

}