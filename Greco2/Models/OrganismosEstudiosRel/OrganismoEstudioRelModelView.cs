using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.OrganismosEstudiosRel
{
    public class OrganismoEstudioRelModelView
    {
        public IEnumerable<SelectListItem> Organismos { get; set; }
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public int organismoSeleccionado { get; set; }
        public int estudioSeleccionado { get; set; }
    }
}