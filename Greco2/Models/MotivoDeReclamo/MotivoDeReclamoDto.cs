﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoDeReclamoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int servicioId { get; set; }
        public int tipoProcesoId { get; set; }
        public bool Deleted { get; set; }


    }
}