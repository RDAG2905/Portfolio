using Greco2.Models.MotivoDeReclamo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Reclamo
{
    public class ReclamoDto
    {
        public int Id { get; set; }
        public int? Id_Motivo_Reclamo { get; set; }
        public int? Id_SubMotivoReclamo { get; set; }
        //public MotivoDeReclamoDto MotivoDeReclamo { get; set; }
       
    }
}