using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.TextoDenuncia
{
    public class TextoDenunciaDto
    {
        public int Id { get; set; }
        public int? DenunciaId { get; set; }
        public DateTime? Fecha { get; set; }
        public string Texto { get; set; }
        public string Usuario { get; set; }
        
    }
}