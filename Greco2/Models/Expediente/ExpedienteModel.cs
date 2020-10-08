using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Expediente
{
    public class ExpedienteModel : EntityTypeConfiguration<ExpedienteDto>
    {
        public ExpedienteModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Numero).IsUnicode(false).HasMaxLength(40);
            this.ToTable("tExpedientes");
        }
    }
}