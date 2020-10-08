using Greco2.Models.Denuncia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Paginacion
{
    public class ResultadoDenuncias:PaginadorModel
    {
       public IEnumerable<ListaSabanaSP> listaDenuncias;

    }
}