using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class EstadoModel : EntityTypeConfiguration<EstadoDto>
    {
        public EstadoModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.TipoEstado).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.Activo);
            HasMany(t => t.subEstados)
               .WithRequired(t => t.Estado)
               .HasForeignKey(t => t.EstadoId)
               .WillCascadeOnDelete(false);
            this.ToTable("tEstados");
        }
    }
}