using Greco2.Models.Organismo;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estudios
{
    public class EstudioDto 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Deleted { get; set; }
        public ICollection<OrganismoDto> Organismos { get; set; }
        
    }
}