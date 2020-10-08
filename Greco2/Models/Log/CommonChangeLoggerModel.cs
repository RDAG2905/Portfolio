using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class CommonChangeLoggerModel : EntityTypeConfiguration<CommonChangeLoggerDto>
    {
        public CommonChangeLoggerModel() {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.FechaCambio);
            this.Property(t => t.ObjetoModificado).HasMaxLength(20).IsUnicode(false);
            this.Property(t => t.Descripcion).HasMaxLength(512).IsUnicode(false);
            this.Property(t => t.ValorAnterior).HasMaxLength(512).IsUnicode(false);
            this.Property(t => t.ValorActual).HasMaxLength(512).IsUnicode(false);
            this.Property(t => t.Usuario).HasMaxLength(20).IsUnicode(false);
            this.Property(t => t.ObjetoId);
            this.ToTable("tCommonChLogger");
        }
    }
}