using Greco2.Models.Localidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Provincia
{
    public class ProvinciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<LocalidadDto>localidades{ get; set; }
        public bool Deleted { get; set; }
    }
}