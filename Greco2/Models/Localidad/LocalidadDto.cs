using Greco2.Models.Provincia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Greco2.Models.Localidad
{
    public class LocalidadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int ProvinciaId { get; set; }
        public ProvinciaDto Provincia { get; set; }
        public bool Deleted { get; set; }
    }
}