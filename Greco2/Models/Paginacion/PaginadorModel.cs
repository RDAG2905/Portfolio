using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Greco2.Models.Paginacion

{
    public class PaginadorModel
    {
        public int PaginaActual { get;set; }
        public int TotalDeRegistros { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalDePaginas { get; set; }
        public RouteValueDictionary ValoresQueryString { get; set; }
        }
}