using Greco2.Model;
using Greco2.Models.Enum;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime;
using Greco2.Models.Denuncia;
using Greco2.Models.Helper;

namespace Greco2.Models.Evento
{
    public class EventoCommandService
    {
        EventoDto eventoModificado;
        EventoDto eventoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;
        public void updateEvento(EventoDto eventoDto)
        {
            eventoModificado = eventoDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
               EventoDto evento = context.getEventos(true).Where(t => t.EventoId == eventoDto.EventoId).FirstOrDefault();
                eventoOriginal = evento;
                //eventoOriginal.EventoId = -1;
                prepararCambios(eventoModificado, eventoOriginal, context);
                var idEventoAModificarr = evento.EventoId;
                evento.CONTESTADO = eventoDto.CONTESTADO;
                evento.TipoEventoId = eventoDto.TipoEventoId;
                evento.Fecha = eventoDto.Fecha;
                evento.REQUERIMIENTOINFORME = eventoDto.REQUERIMIENTOINFORME;
                evento.SOLUCIONADO = eventoDto.SOLUCIONADO;
                evento.Observacion = eventoDto.Observacion;
                evento.ResIntId = eventoDto.ResIntId;
                //save changes to database
                context.SaveChanges();              
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(EventoDto modificado, EventoDto original, NuevoDbContext context) {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.CONTESTADO != original.CONTESTADO) {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now,"Evento","Se ha modificado el campo Contestado",(original.CONTESTADO == 1) ? "Si" : "No", (modificado.CONTESTADO == 1) ? "Si" : "No", usuario, modificado.EventoId);
                listLoggers.Add(logger1);
            }
            if (modificado.TipoEventoId != original.TipoEventoId)
            {
                var tipoEventoNuevo = (modificado.TipoEventoId > 0)?context.TiposDeEventos.Where(x => x.Id == modificado.TipoEventoId).FirstOrDefault().Nombre:"";
                var tipoEventoAnterior = (original.TipoEventoId > 0)?context.TiposDeEventos.Where(x => x.Id == original.TipoEventoId).FirstOrDefault().Nombre:"";
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado el tipo de Evento", tipoEventoNuevo, tipoEventoAnterior, usuario, modificado.EventoId);
                listLoggers.Add(logger2);
            }
            if (modificado.Fecha != original.Fecha)
            {
                //var logger3 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado la Fecha de Vencimiento",  original.Fecha, modificado.Fecha, usuario, modificado.EventoId);
                //listLoggers.Add(logger3);
                //var logger3 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado la Fecha de Vencimiento", original.Fecha.ToShortDateString(), modificado.Fecha.ToShortDateString(), usuario, modificado.EventoId);
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado la Fecha de Vencimiento", (original.Fecha!= null)?original.Fecha.Value.ToShortDateString():"", (modificado.Fecha!= null)?modificado.Fecha.Value.ToShortDateString():"", usuario, modificado.EventoId);
                listLoggers.Add(logger3);
            }
            if (modificado.REQUERIMIENTOINFORME != original.REQUERIMIENTOINFORME)
            {
                var reqInformeNuevo = (modificado.REQUERIMIENTOINFORME > 0)?context.ReqsInforme.Where(r => r.Id == modificado.REQUERIMIENTOINFORME).FirstOrDefault().Nombre:"";
                var reqInformeAnterior = (original.REQUERIMIENTOINFORME > 0)?context.ReqsInforme.Where(r => r.Id == original.REQUERIMIENTOINFORME).FirstOrDefault().Nombre:"";
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado el Requerimiento Informe", reqInformeAnterior, reqInformeNuevo, usuario, modificado.EventoId);
                listLoggers.Add(logger4);
            }
            if (modificado.SOLUCIONADO != original.SOLUCIONADO)
            {
                var solucionadoNuevo = (modificado.SOLUCIONADO > 0)?context.Solucionados.Where(r => r.Id == modificado.SOLUCIONADO).FirstOrDefault().Nombre:"";
                var solucionadoAnterior = (original.SOLUCIONADO > 0)?context.Solucionados.Where(r => r.Id == original.SOLUCIONADO).FirstOrDefault().Nombre:"";
                var logger5 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado el campo Solucionado",  solucionadoAnterior, solucionadoNuevo, usuario, modificado.EventoId);
                listLoggers.Add(logger5);
            }
            if (modificado.Observacion != original.Observacion)
            {
                var logger6 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado la Observación",  original.Observacion, modificado.Observacion, usuario, modificado.EventoId);
                listLoggers.Add(logger6);
            }
            if (modificado.ResIntId != original.ResIntId)
            {
                var responsableNuevo = (modificado.ResIntId > 0)?context.Responsables.Where(r => r.Id == modificado.ResIntId).FirstOrDefault().Nombre:"";
                var responsableAnterior = (original.ResIntId > 0)?context.Responsables.Where(r => r.Id == original.ResIntId).FirstOrDefault().Nombre:"";
                var logger7 = new CommonChangeLoggerDto(DateTime.Now, "Evento", "Se ha modificado el campo Solucionado",  responsableAnterior, responsableNuevo, usuario, modificado.EventoId);
                listLoggers.Add(logger7);
            }
            return listLoggers;
        }




        public void deleteEvento(int id)
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load event from database
                EventoDto evento = context.getEventos(true)
                                              .Where(t => t.EventoId == id)
                                              .FirstOrDefault();
                evento.Deleted = true;
                //context.Remove(evento);
                context.SaveChanges();
            }
        }

       

        public void createEvento(EventoDto evento,int? idRes)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var usuario = context.Responsables.Where(x => x.Id == idRes).FirstOrDefault();
                evento.CREATIONPERSON = usuario.UmeId;
                evento.Deleted = false;
                context.Add(evento);
                context.SaveChanges();
            }
        }

        public void createEvento(EventoDto evento,string usuario)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(evento);
                context.SaveChanges();
                var logger = new CommonChangeLoggerDto(DateTime.Now, "EVENTO", "Se ha creado el Evento id : " + evento.EventoId  + " relacionado a la Denuncia id : " + evento.DenunciaId,null,"Datos iniciales: " + "-Fecha Vencimiento : " + evento.Fecha + "Tipo de Evento: " + evento.TipoEventoId + "-Contestado " + ((evento.CONTESTADO == 1)?"SI":"NO"), usuario,evento.EventoId);
                context.Add(logger);
                context.SaveChanges();
            }
        }

        public CourierHelperClass elEventoEsValido(EventoDto evento) {
            List<string> errorMessages = new List<string>();
            DenunciaDto denuncia = new DenunciaDto();
           
            using (NuevoDbContext context = new NuevoDbContext())
            {
                denuncia = context.Denuncias.Where(x=>x.DenunciaId == evento.DenunciaId).FirstOrDefault();               
            }
            bool control = true;
            if (evento.Fecha == null) {
                errorMessages.Add("La Fecha De Vencimiento es Inválida</br>");
                control = false;
            }
            if (denuncia.FSELLOCIA > evento.Fecha) {
                errorMessages.Add("La Fecha de vencimiento no puede ser anterior a la Fecha de Notificación del Reclamo </br>");
                control = false;
            }
            if (evento.ResIntId == null) {
                errorMessages.Add("Seleccione un Responsable</br>");
                control = false;
            }
            if (!(evento.TipoEventoId > 0))
            {
                errorMessages.Add("Seleccione un Tipo de Evento</br>");
                control = false;
            }
            var courier = new CourierHelperClass();
            courier.elObjetoEsVálido = control;
            courier.mensajes = errorMessages;
            return courier;
        }
    }
}