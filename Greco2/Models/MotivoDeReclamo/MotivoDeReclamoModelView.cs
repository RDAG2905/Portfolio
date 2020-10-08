using Greco2.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoDeReclamoModelView
    {
        [Required(ErrorMessage = "Ingrese un Motivo de ReclamoDto")]
        [Display(Name ="Nombre")]
        public string MotivoDeReclamo { get; set; }
        [Required(ErrorMessage = "Seleccione un Servicio")]
        [Display(Name = "Servicio")]
        public IEnumerable<SelectListItem> Servicios { get; set; }
        public int servicioSeleccionado { get; set; }
        [Required(ErrorMessage = "Seleccione un Tipo de Proceso")]
        public int tipoProcesoSeleccionado { get; set; }
        [Display(Name = "Tipo de Proceso")]
        public IEnumerable<SelectListItem> TiposDeProceso { get; set; }
        
        //public TipoDeProceso TipoDeProceso { get; set; }
    }
}