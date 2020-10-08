using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Parametros
{
    public class ParametroModel : EntityTypeConfiguration<ParametroDto>
    {
        public ParametroModel()
        {
            this.HasKey(t => t.KEY);
            this.Property(t => t.KEY).IsUnicode(false).HasMaxLength(255);
            this.Property(t => t.VALUE).IsUnicode(false).HasMaxLength(255);
            this.ToTable("tParametros");
        }
    }
}