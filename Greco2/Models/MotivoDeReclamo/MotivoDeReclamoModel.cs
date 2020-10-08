using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoDeReclamoModel : EntityTypeConfiguration<MotivoDeReclamoDto>
    {
        public MotivoDeReclamoModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).HasMaxLength(30).IsUnicode(false);
            this.Property(t => t.servicioId);
            this.Property(t => t.tipoProcesoId);
            this.Property(t => t.Deleted);
            this.ToTable("tMotivoDeReclamo");

        }
    }
}