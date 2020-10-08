using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class SubEstadoModel : EntityTypeConfiguration<SubEstadoDto>
    {
        public SubEstadoModel() 
        {            
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).IsUnicode(false).HasMaxLength(100);
            this.Property(t => t.Deleted);
            this.Property(t => t.CierraDenuncia);
            this.ToTable("tSubEstados");
        }
    }
}