using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteHtml
    { 
        public string posicion { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }    
        public int? NroDocumento { get; set; }        
        public int? IdGrupo { get; set; }

    }
}