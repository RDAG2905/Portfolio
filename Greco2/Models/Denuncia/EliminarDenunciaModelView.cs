using Greco2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Denuncia
{
    public class EliminarDenunciaModelView
    {
        public EliminarDenunciaModelView() {
            using (NuevoDbContext context = new NuevoDbContext()) {
                var motivos = context.MotivosDeBaja.OrderBy(x=>x.Nombre).ToList();
                this.motivosDeBaja = motivos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            }
        }
        public IEnumerable<SelectListItem> motivosDeBaja { get; set; }
        public string motivoSeleccionado { get; set; }
    }
}