using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class MediadorCommandService
    {
        MediadorDto MediadorModificado;
        MediadorDto MediadorOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public MediadorDto updateMediador(MediadorDto MediadorDto)
        {
            MediadorModificado = MediadorDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                MediadorDto Mediador = context.getMediadores(true).Where(t => t.Id == MediadorDto.Id).FirstOrDefault();
                MediadorOriginal = Mediador;

                prepararCambios(MediadorModificado, MediadorOriginal, context);
                var idMediadorAModificarr = Mediador.Id;
                Mediador.Nombre = MediadorDto.Nombre.Trim().ToUpper();
                Mediador.Activo = MediadorDto.Activo;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return Mediador;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(MediadorDto modificado, MediadorDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Activo != original.Activo)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "MEDIADOR", "Se ha modificado el campo de Activación", original.Activo ? "Activo" : "Inactivo", modificado.Activo ? "Activo" : "Inactivo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "MEDIADOR", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public MediadorDto createMediador(string Nombre)
        {
            var Mediador = new MediadorDto();
            Mediador.Nombre = Nombre.Trim().ToUpper();
            Mediador.Activo = true;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(Mediador);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "MEDIADOR", "Se ha creado el Mediador", null, Mediador.Nombre, usuario, Mediador.Id);
                context.Add(accion);
                context.SaveChanges();
                return Mediador;
            }
        }

        public bool existeMediador(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Mediadores.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }

        public bool existeOtroMediadorIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Mediadores.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }
    }
}