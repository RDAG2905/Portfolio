using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greco2.Models.Denunciante;

namespace Greco2.Models.Grupo
{
    public class GrupoSP
    {
       
            public int Id { get; set; }
            public ICollection<DenuncianteGrupoSP> grupoDenunciantes { get; set; }
            public int? IdDenunciantePrincipal { get; set; }
            public DenuncianteDto DenunciantePrincipal { get; set; }
    }
}