using Greco2.Models.Denuncia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class EventoDto
    {
        public int EventoId { get; set; }
       
        public int DenunciaId { get; set; }
        public int? DenunciaHistId { get; set; }

        [DisplayName("Fecha Vencimiento:")]
        public DateTime? Fecha { get; set; }
        public int? Organismo_Id { get; set; }
        public int? ResExId { get; set; }
        public int? ResIntId { get; set; }

        [DisplayName("Observación:")]
        public string Observacion { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? FECHACREACION { get; set; }
        public bool? PRESENCIAL { get; set; }
        public string CREATIONPERSON { get; set; }
        public int? ASISTENCIA { get; set; }

        [DisplayName("Req. Informe:")]
        public int? REQUERIMIENTOINFORME { get; set; }

        [DisplayName("Solucionado:")]
        public int? SOLUCIONADO { get; set; }

        [DisplayName("Contestado:")]
        //public bool CONTESTADO { get; set; }
        public int? CONTESTADO { get; set; }

        [DisplayName("Tipo Evento:")]
        public int TipoEventoId { get; set; }
        //public DenunciaDto Denuncia { get; set; }

    }
}

	