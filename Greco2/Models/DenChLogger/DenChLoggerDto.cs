using Greco2.Models.Denuncia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.DenChLogger
{
    public class DenChLoggerDto
    {
        public int Id { get; set; }
        public DateTime? FechaCambio { get; set; }
        public string ObjetoModificado { get; set; }
        public string Descripcion { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
        public string Usuario { get; set; }
        public int? ObjetoId { get; set; }
        //public DenunciaDto Denuncia { get; set; }
    }
}