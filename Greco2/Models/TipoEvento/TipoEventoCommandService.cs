using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.TipoEvento
{
    public class TipoEventoCommandService
    {
        TipoEventoDto TipoEventoModificado;
        TipoEventoDto TipoEventoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public TipoEventoDto updateTipoEvento(TipoEventoDto TipoEventoDto)
        {
            TipoEventoModificado = TipoEventoDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                TipoEventoDto TipoEvento = context.getTiposDeEventos(true).Where(t => t.Id == TipoEventoDto.Id).FirstOrDefault();
                TipoEventoOriginal = TipoEvento;

                prepararCambios(TipoEventoModificado, TipoEventoOriginal, context);
                var idTipoEventoAModificarr = TipoEvento.Id;
                TipoEvento.Nombre = TipoEventoDto.Nombre.Trim().ToUpper();
                TipoEvento.Deleted = TipoEventoDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return TipoEvento;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(TipoEventoDto modificado, TipoEventoDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "TIPO EVENTO", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "TIPO EVENTO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public TipoEventoDto createTipoEvento(string Nombre)
        {
            var TipoEvento = new TipoEventoDto();
            TipoEvento.Nombre = Nombre.Trim().ToUpper();
            TipoEvento.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(TipoEvento);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "TIPO EVENTO", "Se ha creado el de Tipo de Evento", null, TipoEvento.Nombre, usuario, TipoEvento.Id);
                context.Add(accion);
                context.SaveChanges();
                return TipoEvento;
            }
        }

        public bool existeTipoEvento(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.TiposDeEventos.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }
        public bool existeOtroTipoEventoIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.TiposDeEventos.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }
    }
}