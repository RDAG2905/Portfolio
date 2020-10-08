using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Localidad
{
    public class LocalidadModelView
    {
        public string LocalidadNombre { get; set; }
        public IEnumerable<SelectListItem> Provincias { get; set; }
    }
}