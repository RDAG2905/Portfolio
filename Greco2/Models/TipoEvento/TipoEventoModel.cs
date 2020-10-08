using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.TipoEvento
{
    public class TipoEventoModel : EntityTypeConfiguration<TipoEventoDto>
    {
        public TipoEventoModel()
        {
            
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.Codigo).IsUnicode(false).HasMaxLength(8);
            this.Property(t => t.Instruccion).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.Agendable);
            this.Property(t => t.Deleted);
            this.ToTable("tTipoEvento");

        }
    }
}