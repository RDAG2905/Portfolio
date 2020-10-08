using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.DatosCoprec
{
    public class DatosCoprec
    {
        
        public string HonorariosCoprec { get; set; }
        public string NroGestionCoprec { get; set; }
        public DateTime? FechaGestionHonorarios { get; set; }
        public string MontoAcordado { get; set; }
        public string Arancel { get; set; }
        public DateTime? FechaGestionArancel { get; set; }
        public int DenunciaId { get; set; }
        public string Resultado { get; set; }
    }
}