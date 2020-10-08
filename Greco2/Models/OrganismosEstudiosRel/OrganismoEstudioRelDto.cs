using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.OrganismosEstudiosRel
{
    public class OrganismoEstudioRelDto
    {
        public int Id { get; set; }
        public int OrganismoId { get; set; }
        public int EstudioId { get; set; }
    }
}