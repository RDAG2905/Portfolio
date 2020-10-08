using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Organismo
{
    public class OrganismoSP
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Region { get; set; }
        public bool Activo { get; set; }
        
    }
}