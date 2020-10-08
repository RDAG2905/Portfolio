using Greco2.Models.Denunciante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Grupo
{
    public class GrupoDenunciantesDto
    {
        public int Id { get; set; }
        public ICollection<DenuncianteDto> grupoDenunciantes { get; set; }
        public int? IdDenunciantePrincipal { get; set; }
        public DenuncianteDto DenunciantePrincipal { get; set; }


    }
}