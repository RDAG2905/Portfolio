using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mail
{
    public class EmailAdressModel : EntityTypeConfiguration<EmailAdressDto>
    {
        public EmailAdressModel (){
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.EmailAdress).HasMaxLength(250).IsUnicode(false);
            this.ToTable("tEmailAdress");
        }

    }
}