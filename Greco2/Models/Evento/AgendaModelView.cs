using System;
using System.Collections.Generic;
using Greco2.Models.Estudios;
using Greco2.Models.Organismo;
using Greco2.Models.Provincia;
using Greco2.Models.ReqInforme;
using Greco2.Models.Solucionado;
using Greco2.Models.TipoEvento;
using System.Linq;
using System.Web;
using Greco2.Model;
using System.Web.Mvc;
using Greco2.Models.Responsables;

namespace Greco2.Models.Evento
{
    public class AgendaModelView
    {

        public AgendaModelView()
        {
            fechaDesde = DateTime.Today.Date.ToString("yyyy-MM-dd");
            fechaHasta = DateTime.Today.Date.ToString("yyyy-MM-dd");

            
            var estudios = new List<EstudioDto>();
            var tipoEventos = new List<TipoEventoDto>();
            var reqsInforme = new List<ReqInformeDto>();
            var organismos = new List<OrganismoDto>();
            var solucionados = new List<SolucionadoDto>();
            var provincias = new List<ProvinciaDto>();
            var responsables = new List<ResponsableDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                //estudios = context.Estudios.Where(e=>!e.Deleted).OrderBy(e=>e.Nombre).ToList();
                //tipoEventos = context.TiposDeEventos.Where(t=>t.Agendable & !t.Deleted).OrderBy(t=>t.Nombre).ToList();
                //reqsInforme = context.ReqsInforme.OrderBy(r=>r.Nombre).ToList();
                //organismos = context.Organismos.Where(o => !o.Activo).OrderBy(o=>o.Nombre).ToList();
                //solucionados = context.Solucionados.ToList();
                //provincias = context.Provincias.Where(p => !p.Deleted).OrderBy(p=>p.Nombre).ToList();
                //responsables = context.Responsables.Where(p => !p.Deleted && String.Equals(p.TipoResponsable,"RI")).OrderBy(p => p.Apellido).ToList();
                estudios = context.Estudios.OrderBy(e => e.Nombre).ToList();
                tipoEventos = context.TiposDeEventos.Where(t => t.Agendable).OrderBy(t => t.Nombre).ToList();
                reqsInforme = context.ReqsInforme.OrderBy(r => r.Nombre).ToList();
                organismos = context.Organismos.OrderBy(o => o.Nombre).ToList();
                solucionados = context.Solucionados.ToList();
                provincias = context.Provincias.OrderBy(p => p.Nombre).ToList();
                responsables = context.Responsables.Where(p => String.Equals(p.TipoResponsable, "RI")).OrderBy(p => p.Apellido).ToList();
                var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                this.responsable = context.Responsables.Where(x => String.Equals(x.UmeId.Trim(), usuario.Trim())).FirstOrDefault();
            }
            this.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tipoEventos = tipoEventos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.reqInformes = reqsInforme.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Solucionados = solucionados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Provincias = provincias.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre , Value = p.Id.ToString() });
            //this.Responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId + ')', Value = p.Id.ToString() });
        }
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        //public IEnumerable<SelectListItem> tiposFechas { get; set; }
        public bool soloFechas { get; set; }
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public IEnumerable<SelectListItem> Organismos { get; set; }
        public IEnumerable<SelectListItem> tipoEventos { get; set; }
        public IEnumerable<SelectListItem> reqInformes { get; set; }
        public IEnumerable<SelectListItem> Solucionados { get; set; }
        public IEnumerable<SelectListItem> Provincias { get; set; }
        public IEnumerable<SelectListItem> Responsables{ get; set; }
        public int? estudioSeleccionado { get; set; }
        public int? organismoSeleccionado { get; set; }
        public int? tipoEventoSeleccionado { get; set; }
        public int? reqInformeSeleccionado { get; set; }
        public int? solucionadoSeleccionado { get; set; }
        public int? provinciaSeleccionada { get; set; }
        public int? responsableSeleccionado { get; set; }
        public int? idDenuncia { get; set; }
        public string dniDenunciante { get; set; }
        //public int? dniDenunciante { get; set; }
        public bool contestado { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public bool filtrarPorFechaVencimiento { get; set; }
        public bool filtrarPorFechaDenuncia { get; set; }
        public bool filtrarPorFechaNotificacion { get; set; }
        public bool filtrarPorFechaNotificacionGcia { get; set; }
        public bool esUnCambioMasivo { get; set; }

        public ResponsableDto responsable { get; set; }
       
    }
}



