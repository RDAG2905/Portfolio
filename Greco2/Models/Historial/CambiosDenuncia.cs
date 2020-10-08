using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Historial
{
    public class CambiosDenuncia
    {
        public DateTime FechaCambio { get; set; }
        public string ObjetoModificado { get; set; }
        public string Descripcion { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
        public string Usuario { get; set; }
    }
}