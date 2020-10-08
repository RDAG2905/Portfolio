using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class SubEstadoDto : IEquatable<SubEstadoDto>
    {
        public int Id { get; set; }
        public int EstadoId { get; set; }
        public EstadoDto Estado { get; set; }
        public string Nombre { get; set; }
        public bool Deleted { get; set; }
        public bool CierraDenuncia { get; set; }
       
        public bool Equals(SubEstadoDto other)
        {
            return this.Nombre == other.Nombre;
        }
    }
}