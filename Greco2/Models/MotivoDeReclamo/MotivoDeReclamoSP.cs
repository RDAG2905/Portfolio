using Greco2.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoDeReclamoSP
    {
        //public MotivoDeReclamoSP()
        //{
        //    this.ProcesoDescripcion = Enum.GetName(TipoProceso,Proceso);
        //}
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Servicio { get; set; }
        public int Proceso { get; set; }
        public string ProcesoDescripcion { get; set; }
        //public TipoProceso ProcesoDescripcion { get; set; }
        public bool Deleted { get; set; }
    }
}