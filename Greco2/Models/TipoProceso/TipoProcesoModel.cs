using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.TipoProceso
{
    public class TipoProcesoModel : EntityTypeConfiguration<TipoProcesoDto>
    {
        public TipoProcesoModel()
        { 
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).IsUnicode(false).HasMaxLength(30);
            this.ToTable("tTipoProceso");
            
            
        }
    }
}