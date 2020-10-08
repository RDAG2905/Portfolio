using Greco2.Models.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciaSimple
    {
        public int DenunciaId { get; set; }
        public IEnumerable<EventoDto> eventos { get; set; }
    }
}