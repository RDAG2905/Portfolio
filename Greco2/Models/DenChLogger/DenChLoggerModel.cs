using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.DenChLogger
{
    public class DenChLoggerModel : EntityTypeConfiguration<DenChLoggerDto>
    {
        public DenChLoggerModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(e => e.ObjetoModificado).IsUnicode(false).HasMaxLength(20);
            this.Property(e => e.Descripcion).IsUnicode(false).HasMaxLength(512);
            this.Property(e => e.ValorAnterior).IsUnicode(false).HasMaxLength(512);
            this.Property(e => e.ValorActual).IsUnicode(false).HasMaxLength(512);
            this.Property(e => e.Usuario).IsUnicode(false).HasMaxLength(20);
            this.Property(e => e.ObjetoId);
            this.ToTable("tDenChLogger");
        }

    }
}