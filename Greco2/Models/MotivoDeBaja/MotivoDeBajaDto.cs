using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeBaja
{
    public class MotivoDeBajaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Deleted { get; set; }

    }
}