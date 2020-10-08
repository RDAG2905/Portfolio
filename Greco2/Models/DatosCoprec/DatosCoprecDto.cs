using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Greco2.Models.DatosCoprec
{
    public class DatosCoprecDto
    {
        public int Id { get; set; }
        [DisplayName("Honorarios")]
        public string honorariosCoprec { get; set; }
        [DisplayName("Nro Gestión")]
        public string nroGestion { get; set; }
        [DisplayName("Fecha Gestión Honorarios")]
        public DateTime fechaGestionHonorarios { get; set; }
        [DisplayName("Monto Acordado")]
        public string montoAcordado { get; set; }
        public string arancel { get; set; }
        [DisplayName("Fecha Gestión Arancel")]
        public DateTime fechaGestionArancel { get; set; }
        public int DenunciaId { get; set; }
    }
}