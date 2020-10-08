using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Reclamo
{
    public class ReclamoModel : EntityTypeConfiguration<ReclamoDto>
    {
        public ReclamoModel()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.Property(t => t.Id_Motivo_Reclamo);
            this.Property(t => t.Id_SubMotivoReclamo);
            this.ToTable("tReclamos");
        }

    }

}