using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Servicio
{
    public class ServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Deleted { get; set; }
        public string Grupo { get; set; }
        
    }
}