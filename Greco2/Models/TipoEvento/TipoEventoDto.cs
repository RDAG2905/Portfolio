using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.TipoEvento
{
    public class TipoEventoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Agendable { get; set; }
        public string Instruccion { get; set; }
        public bool Deleted { get; set; }
    }
}