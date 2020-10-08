using Greco2.Models.Grupo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteDto
    {
        public int DenuncianteId { get; set; }
        public string tipoDenunciante { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string apellido { get; set; }
        public string linea { get; set; }
        public string nombre { get; set; }
        public string NroCliente { get; set; }
        public string NroDocumento { get; set; }
        public string Observaciones { get; set; }
        public string Telefono { get; set; }
        public int? tipoDocumento { get; set; }
        public int? IdGrupo { get; set; }
        public bool? Deleted { get; set; }

        public List<GrupoDto> Grupos { get; set; }
        //public List<GrupoDenunciantesRelDto> GrupoDenunciantesRels { get; set; }
        


    }
}