using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class DomicilioMediadorDto
    {
        public int Id { get; set; }
        public string Domicilio { get; set; }
        public int MediadorId { get; set; }
        public MediadorDto Mediador { get; set; }
        public bool Activo { get; set; }

    }
}