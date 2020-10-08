using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Responsables
{
    public class ResponsableModel : EntityTypeConfiguration<ResponsableDto>
    {
        public ResponsableModel() {
            this.HasKey(m => m.Id);
            this.Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(m => m.TipoResponsable).HasMaxLength(2);
            this.Property(m => m.UmeId).HasMaxLength(30).IsUnicode(false);
            this.Property(m => m.Deleted);
            this.Property(m => m.Nombre).HasMaxLength(50).IsUnicode(false);
            this.Property(m => m.Apellido).HasMaxLength(50).IsUnicode(false);
            this.Property(m => m.Estudio_Id);
            this.Property(m => m.FechaBaja);
            this.Property(m => m.Rol).HasMaxLength(50);            
            this.ToTable("tResponsables");
        }    
        


    }
}