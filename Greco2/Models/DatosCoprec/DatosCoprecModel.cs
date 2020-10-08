using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.DatosCoprec
{
    public class DatosCoprecModel : EntityTypeConfiguration<DatosCoprecDto>
    {
        public DatosCoprecModel() 
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.honorariosCoprec);
            this.Property(t => t.nroGestion);
            this.Property(t => t.fechaGestionHonorarios);
            this.Property(t => t.montoAcordado);
            this.Property(t => t.arancel);
            this.Property(t => t.fechaGestionArancel);
            this.Property(t => t.DenunciaId);
            this.ToTable("tDatosCoprec");
        }
    }
}