using Greco2.Models.Estudios;
using Greco2.Models.Provincia;
using Greco2.Models.Region;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Organismo
{
    public class OrganismoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? Localidad_Id { get; set; }
        public int? Provincia_Id { get; set; }
        public int? Region_Id { get; set; }
        public bool Activo { get; set; }
        public ICollection<EstudioDto> Estudios { get; set; }
        //public ProvinciaDto Provincia { get; set; }
        //public RegionDto Region { get; set; }

    }
}