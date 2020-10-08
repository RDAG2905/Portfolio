using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteModel : EntityTypeConfiguration<DenuncianteDto>
    {
        public DenuncianteModel(){
            this.HasKey(t => t.DenuncianteId);
            this.Property(t => t.DenuncianteId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.tipoDenunciante).HasMaxLength(2).IsUnicode(false);
            this.Property(t => t.Deleted);
            this.Property(t => t.apellido).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.direccion).HasMaxLength(100).IsUnicode(false);
            this.Property(t => t.email).HasMaxLength(100).IsUnicode(false);
            this.Property(t => t.linea).HasMaxLength(15).IsUnicode(false);
            this.Property(t => t.nombre).HasMaxLength(50).IsUnicode(false);
            this.Property(t => t.NroCliente).HasMaxLength(20).IsUnicode(false);
            this.Property(t => t.NroDocumento).HasMaxLength(15).IsUnicode(false); ;
            this.Property(t => t.Observaciones).HasMaxLength(500).IsUnicode(false);
            this.Property(t => t.Telefono).HasMaxLength(20).IsUnicode(false);
            this.Property(t => t.tipoDocumento);            
            this.Property(t => t.IdGrupo).IsOptional();
           
            
            //HasMany(e => e.Grupos)
            //    .WithOptional(e => e.DenunciantePrincipal)
            //    .HasForeignKey(e => e.DenunciantePrincipal_DenuncianteId);
           
            this.ToTable("tDenunciantes");
        }
    }
}

