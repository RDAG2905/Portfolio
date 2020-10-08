using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciasContainerModel
    {
        public DenunciasContainerModel(DenunciaModelView model)
        {
            this.denunciasModelView = model;
        }

        public DenunciaModelView denunciasModelView { get; set; }
    }
}