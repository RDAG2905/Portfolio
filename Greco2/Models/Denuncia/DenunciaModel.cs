using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciaModel : EntityTypeConfiguration<DenunciaDto>
    {
        public DenunciaModel()
        {
            this.HasKey(t => t.DenunciaId);
            this.Property(t => t.DenunciaId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.CREATIONDATE);
            this.Property(t => t.CREATIONPERSON).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.DenunciaId);
            this.Property(t => t.CONCILIACION_ID);
            this.Property(t => t.IMPUTACION_ID);
            this.Property(t => t.SANCION_ID);
            this.Property(t => t.ESTUDIO_ID);

            this.Property(t => t.FSELLOGCIADC);
            this.Property(t => t.FSELLOCIA);
            this.Property(t => t.INACTIVO);

            this.Property(t => t.MODALIDADGESTION);

            this.Property(t => t.OBSERVACIONES).IsUnicode(false).HasMaxLength(250);
            this.Property(t => t.ORGANISMO_ID);

            this.Property(t => t.RESP_EXT_ID);
            this.Property(t => t.RESP_INT_ID);
            this.Property(t => t.SERV_DEN_ID);
            this.Property(t => t.TIPO_PRO_ID);
            this.Property(t => t.SUBTIPO_PRO_ID);
            this.Property(t => t.RECLAMO_ID);
            this.Property(t => t.EXPEDIENTE_ID);

            this.Property(t => t.RESULTADO_ID);
            this.Property(t => t.DELETED);
            this.Property(t => t.ETAPA_ID);

            this.Property(t => t.OBJETORECLAMO).IsUnicode(false).HasMaxLength(255);
            this.Property(t => t.FECHARESULTADO);
            this.Property(t => t.PARENTDENUNCIAID);
            this.Property(t => t.MOTIVOBAJA_ID);
            this.Property(t => t.TRAMITECRM).IsUnicode(false).HasMaxLength(255);

            this.Property(t => t.ECMID).HasMaxLength(50);
            this.Property(t => t.grupoId);
            this.Property(t => t.nroClienteContrato).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.mediadorId);
            this.Property(t => t.domicilioMediadorId);
            this.Property(t => t.reclamoRelacionado).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.fechaHomologacion);
            this.Property(t => t.nroGestionCoprec).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.honorariosCoprec).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.fechaGestionHonorarios)/*.IsUnicode(false).HasMaxLength(50)*/;
            this.Property(t => t.montoAcordado).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.arancel).IsUnicode(false).HasMaxLength(50);
            this.Property(t => t.fechaGestionArancel);

            this.Property(t => t.agendaCoprec).HasMaxLength(500).IsUnicode(false);
            //HasMany(e => e.denChloggers)
            //    .WithOptional(e => e.Denuncia)
            //    .HasForeignKey(e => e.ObjetoId);
            //HasMany(e => e.eventos)
            //    .WithRequired(e => e.Denuncia)
            //    .HasForeignKey(e => e.DenunciaId);
            //HasMany(e => e.eventos)
            //    .WithOptional(e => e.Denuncia)
            //    .HasForeignKey(e => e.DenunciaId);
            //HasMany(e => e.textosDenuncia)
            //    .WithOptional(e => e.Denuncia)
            //    .HasForeignKey(e => e.DenunciaId);

            this.ToTable("tDenuncias");

        }
    }
}

