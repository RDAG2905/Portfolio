using Greco2.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Responsables
{
    public class ResponsablesModelView
    {
        public int estudioSeleccionado { get; set; }
        public int rolSeleccionado { get; set; }
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public bool Inactivo { get; set; }

        //public ResponsablesModelView()
        //{
        //    Roles = new SelectList(new[]
        //                                     {
        //                                         new {Text="",},
        //                                         new {Text=Rol.ADMINISTRADOR.ToString()},
        //                                         new {Text=Rol.ANALISTA.ToString()},
        //                                         new {Text=Rol.COORDINADOR.ToString()},
        //                                         new {Text=Rol.GERENTE.ToString()}                                                                                                                                                                                          },
        //                      ,"Text",this.rolSeleccionado.ToString());
        //    }
        //}
    }

}