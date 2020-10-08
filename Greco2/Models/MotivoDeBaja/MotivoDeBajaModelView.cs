using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeBaja
{
    public class MotivoDeBajaModelView
    {
        public string nombreMotivoDeBaja { get; set; }
        public MotivoDeBajaDto motivoDeBaja { get; set; }
        public List<MotivoDeBajaDto> motivos { get; set; }
    }
}