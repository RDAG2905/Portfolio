using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class DomicilioMediadorModel:EntityTypeConfiguration <DomicilioMediadorDto>
    {
        public DomicilioMediadorModel(){
            this.HasKey(d => d.Id);
            this.Property(d=>d.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(d => d.Domicilio).IsRequired().HasMaxLength(250).IsUnicode(false);
            this.Property(d => d.MediadorId);
            this.ToTable("tDomicilioMediadores");
        }
    }
}