using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class EstadoDto
    {
        public int Id { get; set; }
        public string TipoEstado { get; set; }
        public bool Activo { get; set; }
        public ICollection<SubEstadoDto> subEstados { get; set; }
    }
}