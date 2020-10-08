using Greco2.Models.Provincia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Localidad
{
    public class LocalidadModel : EntityTypeConfiguration <LocalidadDto>
    {
        public LocalidadModel() {
           
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.Deleted);
            this.Property(t => t.ProvinciaId);
            this.ToTable("tLocalidades");


        }
    }
}