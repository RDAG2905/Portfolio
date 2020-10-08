using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Solucionado
{
    public class SolucionadoModel : EntityTypeConfiguration<SolucionadoDto>
    {
        public SolucionadoModel()
        {
            this.HasKey(m => m.Id);
            this.Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(m => m.Nombre).HasMaxLength(30).IsUnicode(false);
            this.ToTable("tSolucionado");
        }
    }
}