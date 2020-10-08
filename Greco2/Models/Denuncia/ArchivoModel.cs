using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{

    public class ArchivoModel : EntityTypeConfiguration<ArchivoDto>
    {
        public ArchivoModel()
        {
           
            this.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
            this.Property(t => t.fechaCreacion).IsRequired();
            
            this.Property(t => t.Extension).IsRequired().HasMaxLength(4);
            this.Property(t => t.Descargas);
            this.Property(t => t.path).HasMaxLength(250).IsUnicode(false);
            this.Property(t => t.DenunciaId).IsRequired();
            this.Property(t => t.usuarioCreador).HasMaxLength(30).IsRequired();
           
            this.ToTable("tArchivos");
        }
    }
}