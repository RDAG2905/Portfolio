using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class MediadorModel:EntityTypeConfiguration <MediadorDto>
    {
        public MediadorModel()
        {
            this.HasKey(m=>m.Id);
            this.Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(m => m.Nombre).HasMaxLength(50).IsUnicode(false);
            this.HasMany(m => m.domicilios)
                .WithRequired(d=> d.Mediador)
                .HasForeignKey(d=>d.MediadorId)
                .WillCascadeOnDelete(false);
            this.Property(m => m.Activo);
            this.Property(m=>m.Matricula).HasMaxLength(50).IsUnicode(false);
            this.ToTable("tMediadores");
        }
    }
}