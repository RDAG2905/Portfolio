using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Servicio
{
    public class ServicioCommandService
    {
        ServicioDto ServicioModificado;
        ServicioDto ServicioOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public ServicioDto updateServicio(ServicioDto ServicioDto)
        {
            ServicioModificado = ServicioDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                ServicioDto Servicio = context.getServicios(true).Where(t => t.Id == ServicioDto.Id).FirstOrDefault();
                ServicioOriginal = Servicio;

                prepararCambios(ServicioModificado, ServicioOriginal, context);
                var idServicioAModificarr = Servicio.Id;
                Servicio.Nombre = ServicioDto.Nombre.Trim().ToUpper();
                Servicio.Deleted = ServicioDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return Servicio;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(ServicioDto modificado, ServicioDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "SERVICIO", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "SERVICIO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public ServicioDto createServicio(string Nombre)
        {
            var Servicio = new ServicioDto();
            Servicio.Nombre = Nombre.Trim().ToUpper();
            Servicio.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(Servicio);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "SERVICIO", "Se ha creado el Servicio", null, Servicio.Nombre, usuario, Servicio.Id);
                context.Add(accion);
                context.SaveChanges();
                return Servicio;
            }
        }

        public bool existeServicio(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Servicios.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }

        public bool existeOtroServicioIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Servicios.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }
    }
}