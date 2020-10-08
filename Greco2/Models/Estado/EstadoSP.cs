using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class SubEstadoSP
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string EtapaProcesal { get; set; }
        public bool Deleted { get; set; }
    }
}