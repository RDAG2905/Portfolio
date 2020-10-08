using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Estado
{
    public class EstadoModelView
    {
        public string nombreSubEstado { get; set; }
        public IEnumerable<SelectListItem> estados { get; set; }
        public int estadoSeleccionado { get; set; }
    }
}