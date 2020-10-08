using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class EventoModel : EntityTypeConfiguration<EventoDto>
    {
        public EventoModel()
        {
            this.HasKey(t => t.EventoId);
            this.Property(t => t.EventoId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.DenunciaId);
            this.Property(t => t.DenunciaHistId);
            this.Property(t => t.Fecha);
            this.Property(t => t.Organismo_Id);
            this.Property(t => t.ResExId);
            this.Property(t => t.ResIntId);
            this.Property(t => t.Observacion).IsUnicode(false).HasMaxLength(500);
            this.Property(t => t.Deleted);
            this.Property(t => t.FECHACREACION);
            this.Property(t => t.PRESENCIAL);
            this.Property(t => t.CREATIONPERSON).IsUnicode(false).HasMaxLength(30);
            this.Property(t => t.ASISTENCIA);
            this.Property(t => t.REQUERIMIENTOINFORME);
            this.Property(t => t.CONTESTADO);
            this.Property(t => t.SOLUCIONADO);
            this.Property(t => t.TipoEventoId);
            this.ToTable("tEventos");
        }
    }
}