using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.OrganismosEstudiosRel
{
    public class OrganismoEstudioRelSP
    {
        public int Id { get; set; }
        public int OrganismoRelacionId { get; set; }
        public string Organismo { get; set; }
        public int EstudioRelacionId { get; set; }
        public string Estudio { get; set; }
        
    }
}