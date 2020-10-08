using Greco2.Models.Paginacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class PaginadorAgenda:PaginadorModel
    {
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public int totalDeRegistros { get; set; }
        public IEnumerable<AgendaSP> listadoAgenda { get; set; }
    }
}