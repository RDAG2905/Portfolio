using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Provincia
{
    public class ProvinciaModel : EntityTypeConfiguration<ProvinciaDto>
    {
        public ProvinciaModel() {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).HasMaxLength(50).IsUnicode(false); 
            this.Property(t => t.Deleted);
            HasMany(t => t.localidades)
                .WithRequired(t => t.Provincia)
                .HasForeignKey(t => t.ProvinciaId);
            this.ToTable("tProvincias");
        }
        
    }
}