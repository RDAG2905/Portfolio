using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class AgendaExcelSP
    {
        public string FechaEvento { get; set; }
        public DateTime? Fecha { get; set; }
        public int EventoId { get; set; }
        public int DenunciaId { get; set; }
        //public string nombreTipoEvento { get; set; }
        public string Tipo_Evento { get; set; }
        public int? TipoEventoId { get; set; }
        public bool Agendable { get; set; }
        public string Organismo { get; set; }
        public int? ORGANISMO_ID { get; set; }
        public string Denunciante { get; set; }
        //public int? dniDenunciante { get; set; }
        //public int? Dni_Denunciante { get; set; }
        public string Dni_Denunciante { get; set; }
        public string Nro_Linea { get; set; }
        public string Servicio { get; set; }
        public string Provincia { get; set; }
        public string Nro_Expediente { get; set; }        
        public string Estado_Actual { get; set; }
        public int? Provincia_Id { get; set; }
        public string TramiteCRM { get; set; }
        
        public int? SolucionadoId { get; set; }
        public string Solucionado { get; set; }
        public int? REQUERIMIENTOINFORME { get; set; }
        public string Requerimiento_Informe { get; set; }

        public int? Contestado { get; set; }
        public string Contestado_ { get; set; }
        public int? ResIntId { get; set; }
        public string Responsable { get; set; }
        public string Estudio { get; set; }
        public int? ESTUDIO_ID { get; set; }
        public int? DENUNCIANTE_ID { get; set; }
        public string Mediador { get; set; }
        public string Matricula { get; set; }
        public string Domicilio_Mediador { get; set; }
        public bool? Deleted { get; set; }

        //public DateTime? Fecha_Homologacion { get; set; }
        //public string Nro_Gestion_Coprec { get; set; }
        //public string Honorarios_Coprec { get; set; }        
        //public DateTime? Fecha_Gestion_Honorarios { get; set; }
        //public string Monto_Acordado { get; set; }
        //public string Arancel { get; set; }
        //public DateTime? Fecha_Gestion_Arancel { get; set; }
        //public string Agenda_Coprec { get; set; }
    }
}