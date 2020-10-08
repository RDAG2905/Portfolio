using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class AgendaMM
    {
            //public string Fecha { get; set; }
            public DateTime Fecha { get; set; }
            public int EventoId { get; set; }
            public int DenunciaId { get; set; } 
            public string nombretipoEvento { get; set; }
            public string Organismo { get; set; }
            public string Denunciante { get; set; }
            public string TramiteCRM { get; set; }           
            public string RequerimientoInforme { get; set; }            
            public string Solucionado { get; set; }
            //public bool Contestado { get; set; }
            public int? Contestado { get; set; }
            public string Estudio { get; set; }
            public string Responsable { get; set; }
    }
}