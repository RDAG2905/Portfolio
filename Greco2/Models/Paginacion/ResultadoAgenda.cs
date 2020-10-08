using Greco2.Models.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Paginacion
{
    public class ResultadoAgenda : PaginadorModel
    {
        public IEnumerable<AgendaSP> listadoAgenda { get; set; }
    }
}