using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.ModalidadGestión
{
    public class ModalidadGestionModel : EntityTypeConfiguration<ModalidadGestionDto>
    {
        public ModalidadGestionModel()
        {
            this.HasKey(m => m.Id);
            this.Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(m => m.Nombre).IsUnicode(false).HasMaxLength(50);
            this.ToTable("tModalidadGestion");
        }
    }
}


