using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteGrupoSP
    {
        public int GrupoDto_Id { get; set; }
        public int DenuncianteId { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string DNI { get; set; }
        //public int? DNI { get; set; }
    }
}