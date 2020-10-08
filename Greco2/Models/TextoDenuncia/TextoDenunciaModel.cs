using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.TextoDenuncia
{
    public class TextoDenunciaModel : EntityTypeConfiguration<TextoDenunciaDto>
    {
        public TextoDenunciaModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.DenunciaId);
            this.Property(t => t.Fecha);
            this.Property(t => t.Texto).IsUnicode(false).HasMaxLength(500);
            this.Property(t => t.Usuario).IsUnicode(false).HasMaxLength(250);
            this.ToTable("tTextosDenuncia");
        }
    }
}