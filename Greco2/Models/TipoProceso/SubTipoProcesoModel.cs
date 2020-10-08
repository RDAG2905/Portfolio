using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.TipoProceso
{
    public class SubTipoProcesoModel : EntityTypeConfiguration<SubTipoProcesoDto>
    {

        public SubTipoProcesoModel()
        {
            this.HasKey(m => m.Id);
            this.Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(m => m.Nombre).IsUnicode(false).HasMaxLength(30);
            this.ToTable("tSubTipoProceso");
        }

    }
}

