using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class ListaSabanaMM
    {
        public int DenunciaId { get; set; }
        public string Expediente{ get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public string Organismo { get; set; }
        public string Servicio { get; set; }
        public string Estudio { get; set; }
        public string Nombre { get; set; }
        public string Denunciante { get; set; } 
        public string Estado_Actual { get; set; }
        public string Tipo_Proceso { get; set; }
        public string ResponsableApellido { get; set; }
        public string ResponsableNombre { get; set; }

    }
}