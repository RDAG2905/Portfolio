using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoReclamoServicioTipoProceso
    {
        public int Id { get; set; }
        public string NombreCompuesto { get; set; }
        public int ServicioId { get; set; }
    }
}