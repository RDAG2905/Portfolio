using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Localidad
{
    public class LocalidadSP
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PROVINCIA { get; set; }
        public bool Deleted { get; set; }
    }
}