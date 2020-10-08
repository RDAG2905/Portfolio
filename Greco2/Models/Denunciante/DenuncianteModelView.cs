using Greco2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteModelView
    {
       
        public List<DenuncianteDto> Denunciantes { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public string sinResultados{ get; set; }

        public DenuncianteDto denunciante { get; set; }
    }
}