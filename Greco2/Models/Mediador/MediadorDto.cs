using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class MediadorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<DomicilioMediadorDto> domicilios { get; set; }
        public bool Activo { get; set; }
        public string Matricula { get; set; }

      
    }
}