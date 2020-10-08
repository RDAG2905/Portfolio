using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class EventoSP
    {
        public int EventoId { get; set; }
        public DateTime? Fecha { get; set; }
        public string tipoEvento { get; set; }
        public string observacion { get; set; }
        public int? contestado { get; set; }
        public string Solucionado { get; set; }
        public string RequerimientoInforme { get; set; }
        public int DenunciaId { get; set; }
        public bool Deleted { get; set; }
    }
}