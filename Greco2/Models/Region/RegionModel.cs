using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Region
{
    public class RegionModel : EntityTypeConfiguration<RegionDto>
    {
        public RegionModel() {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Nombre).HasMaxLength(30).IsUnicode(false);
            this.Property(t => t.Deleted);
            this.ToTable("tRegiones");

        }
    }
}