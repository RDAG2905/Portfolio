using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Greco2.Models.Responsables
{
    public class ResponsableDto
    {
        public int Id { get; set; }
        public string TipoResponsable { get; set; }
        public string UmeId { get; set; }
        public bool Deleted { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int? Estudio_Id { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Rol { get; set; }
        
    }
}