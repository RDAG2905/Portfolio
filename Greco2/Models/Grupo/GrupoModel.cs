using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Grupo
{
    public class GrupoModel:EntityTypeConfiguration<GrupoDto>
    {
        public GrupoModel()
        {
           
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IdDenunciantePrincipal);
            this.ToTable("tGrupos");
            HasMany(t => t.grupoDenunciantes)
               .WithMany(t => t.Grupos)
               .Map(t => t.ToTable("GrupoDenunciantesRel"));

        }
    }
}