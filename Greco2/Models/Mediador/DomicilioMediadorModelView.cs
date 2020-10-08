using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Mediador
{
    public class DomicilioMediadorModelView
    {
        public string NombreMediador { get; set; }
        public IEnumerable<SelectListItem> Mediadores { get; set; }
    }
}