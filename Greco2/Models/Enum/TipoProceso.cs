using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Greco2.Models.Enum
{
    public enum TipoProceso
    {
        //[Display(Name = "Denuncia")]
        //Denuncia = 1,
        //[Display(Name = "Denuncia Preventiva")]
        //Denuncia_Preventiva = 2,
        //[Display(Name ="Actuación Especial")]
        //Actuacion_Especial = 3
    }

    public enum TipoResponsable
    {

        [Display(Name = "Responsable Interno")]
        RI = 1,
        [Display(Name = "Responsable Externo")]
        RE = 2
       
    }
}