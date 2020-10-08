using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Organismo
{
    public class OrganismoModelView
    {
        public string Organismo { get; set; }
        public int provinciaSeleccionada { get; set; }
        public int localidadSeleccionada { get; set; }
        public int regionSeleccionada { get; set; }
        public IEnumerable<SelectListItem> Provincias { get; set; }
        public IEnumerable<SelectListItem> Localidades { get; set; }
        public IEnumerable<SelectListItem> Regiones { get; set; }
        public bool Activo { get; set; }
    }
}