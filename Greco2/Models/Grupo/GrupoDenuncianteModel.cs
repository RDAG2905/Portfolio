using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Grupo
{
    public class GrupoDenuncianteModel : EntityTypeConfiguration<GrupoDenunciantesDto>
    {
        public GrupoDenuncianteModel()
        {
            //this.HasKey(t => t.Id);
            //this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //this.Property(t => t.IdDenunciantePrincipal);
            //this.ToTable("tGrupoDenunciantes");
            //HasMany(t => t.grupoDenunciantes)
            //   .WithOptional(t => t.Grupo)
            //   .HasForeignKey(t => t.IdGrupo)
            //   .WillCascadeOnDelete(false);
            

        }
    }
}