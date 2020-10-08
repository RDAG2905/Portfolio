using Greco2.Model;
using Greco2.Models.ReqInforme;
using Greco2.Models.Responsables;
using Greco2.Models.Solucionado;
using Greco2.Models.TipoEvento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Evento
{
    public class EventoModelView
    {
        public EventoModelView(int id)
        {
            var reqsInforme = new List<ReqInformeDto>();
            var tipoEventos = new List<TipoEventoDto>();
            var solucionados = new List<SolucionadoDto>();
            var responsables = new List<ResponsableDto>();
            EventoDto eventoDB = new EventoDto();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                eventoDB = context.getEventos().Where(t => t.EventoId == id).FirstOrDefault();
                solucionados = context.Solucionados.ToList();               
                tipoEventos = context.TiposDeEventos.OrderBy(x=>x.Nombre).ToList();
                reqsInforme = context.ReqsInforme.ToList();
                responsables = context.Responsables.Where(e => String.Equals(e.TipoResponsable, "RI")).OrderBy(e => e.Apellido).ToList();
               
                //responsables = context.Responsables.Where(e => !e.Deleted && String.Equals(e.TipoResponsable, "RI")).OrderBy(e => e.Apellido).ToList();
                //tipoEventos = context.TiposDeEventos.Where(t=>!t.Deleted).ToList();
            }
            this.evento = eventoDB;
            if (evento.Fecha.HasValue)
            {
                this.fechaVencimiento = evento.Fecha;
                this.horaVencimiento = evento.Fecha.Value.TimeOfDay.ToString();
            }
            else {
               
                this.horaVencimiento = "";
            }
            //this.fechaVencimiento = evento.Fecha.Value.Date;
            this.reqsInforme = reqsInforme.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.solucionados = solucionados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tipoEventos = tipoEventos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre , Value = p.Id.ToString() });
            
            //this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId + ')', Value = p.Id.ToString() });


        }
        public EventoDto evento { get; set; }
        public DateTime? fechaVencimiento { get; set; }
        
        public string horaVencimiento { get; set; }
        public IEnumerable<SelectListItem> reqsInforme { get; set; }
        public string reqInformeSeleccionado { get; set; }
        public IEnumerable<SelectListItem> tipoEventos { get; set; }
        public string tipoEventoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> solucionados { get; set; }
        public string solucionadoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> responsables { get; set; }
        public int? ResIntId { get; set; }
        public string responsableId { get; set; }
        public int DenunciaId { get; set; }
        public int? CONTESTADO { get; set; }
        public int TipoEventoId { get; set; }
        public DateTime? Fecha { get; set; }
        public int? REQUERIMIENTOINFORME { get; set; }
        public int? SOLUCIONADO { get; set; }
        public string Observacion { get; set; }
    }
}