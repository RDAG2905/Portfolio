using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace Greco2.Models.Estudios
{
    public class EstudioModel : EntityTypeConfiguration<EstudioDto>
    {
        public EstudioModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.Deleted);
           
            //HasMany(e => e.Organismos)
            //    .WithMany(e => e.Estudios)
            //    .Map(m => m.ToTable("tOrganismosEstudiosRel").MapLeftKey("EstudioDto_Id").MapRightKey("OrganismoDto_Id"));
            this.ToTable("tEstudios");
        }
    }
}