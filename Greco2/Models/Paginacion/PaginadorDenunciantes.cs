using Greco2.Models.Denunciante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Paginacion
{
    public class PaginadorDenunciantes : PaginadorModel
    {
        public IEnumerable<DenuncianteDto> denunciantes { get; set;}
    }
}