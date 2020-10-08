using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeBaja
{
    public class MotivoDeBajaCommandService
    {
        MotivoDeBajaDto motivoModificado;
        MotivoDeBajaDto motivoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public MotivoDeBajaDto updateMotivo(MotivoDeBajaDto MotivoDeBajaDto)
        {
            motivoModificado = MotivoDeBajaDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                MotivoDeBajaDto motivo = context.getMotivos(true).Where(t => t.Id == MotivoDeBajaDto.Id).FirstOrDefault();
                motivoOriginal = motivo;

                prepararCambios(motivoModificado, motivoOriginal, context);
                var idmotivoAModificarr = motivo.Id;
                motivo.Nombre = MotivoDeBajaDto.Nombre;
                motivo.Deleted = MotivoDeBajaDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return motivo;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(MotivoDeBajaDto modificado, MotivoDeBajaDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE BAJA", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE BAJA", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public MotivoDeBajaDto createMotivo(string Nombre)
        {
            var motivo = new MotivoDeBajaDto();
            motivo.Nombre = Nombre.ToUpper();
            motivo.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(motivo);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE BAJA", "Se ha creado el Motivo de Baja", null, motivo.Nombre, usuario, motivo.Id);
                context.Add(accion);
                context.SaveChanges();
                return motivo;
            }
        }

        public bool existeMotivoDeBaja(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.MotivosDeBaja.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }

        public bool existeOtroMotivoDeBajaIgual(string unNombre,int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.MotivosDeBaja.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }


    }
}