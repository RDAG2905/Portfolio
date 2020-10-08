
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class AgendaSP
    {
            
        //public string Fecha { get; set; }
        public DateTime? Fecha { get; set; }
        public int EventoId { get; set; }
        public int DenunciaId { get; set; }
       
        public int? TipoEventoId { get; set; }
        public bool Agendable { get; set; }
        
        public string nombretipoEvento { get; set; }
        public int? OrganismoId { get; set; }
        public string Organismo { get; set; }
        public string Denunciante { get; set; }
        //public int? dniDenunciante { get; set; }
        public string dniDenunciante { get; set; }
        public int? ProvinciaId { get; set; }
        public string TramiteCRM { get; set; }
        public int? ReqInformeId { get; set; }
        public string RequerimientoInforme { get; set; }
        public int? SolucionadoId { get; set; }
        public string Solucionado { get; set; }
        //public bool Contestado { get; set; }
        public int? Contestado { get; set; }
        public int? ResIntId { get; set; }
        public int? estudioId { get; set; }
        public string Estudio { get; set; }
        public string Responsable { get; set; }
        public bool? Deleted { get; set; }
       
    }
}