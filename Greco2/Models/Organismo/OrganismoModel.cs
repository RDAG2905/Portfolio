using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Organismo
{
    public class OrganismoModel : EntityTypeConfiguration<OrganismoDto>
    {
        public OrganismoModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.Localidad_Id);
            this.Property(t => t.Provincia_Id);
            this.Property(t => t.Region_Id);
            this.Property(t => t.Activo);           
            this.ToTable("tOrganismos");
            HasMany(t => t.Estudios)
                .WithMany(t => t.Organismos)
                .Map(t => t.ToTable("tOrganismosEstudiosRel"));

        }
    }
}