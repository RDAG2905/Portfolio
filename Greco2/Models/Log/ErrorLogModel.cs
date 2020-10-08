using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class LogErrorModel : EntityTypeConfiguration<LogErrorDto>
    {
        public  LogErrorModel(){
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Fecha);
            this.Property(t => t.Error).HasMaxLength(500).IsUnicode(false);
            this.Property(t => t.UrlRequest).HasMaxLength(250).IsUnicode(false);
            this.Property(t => t.UserId).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.ErrorDetallado).IsUnicode(false);
            this.ToTable("tLogErrors");
        }
        
    }
}