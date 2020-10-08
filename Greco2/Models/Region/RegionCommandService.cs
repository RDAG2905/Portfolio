using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Region
{
    public class RegionCommandService
    {
        RegionDto RegionModificado;
        RegionDto RegionOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public RegionDto updateRegion(RegionDto RegionDto)
        {
            RegionModificado = RegionDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                RegionDto Region = context.getRegiones(true).Where(t => t.Id == RegionDto.Id).FirstOrDefault();
                RegionOriginal = Region;

                prepararCambios(RegionModificado, RegionOriginal, context);
                var idRegionAModificarr = Region.Id;
                Region.Nombre = RegionDto.Nombre.Trim().ToUpper();
                Region.Deleted = RegionDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return Region;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(RegionDto modificado, RegionDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "REGION", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "REGION", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public RegionDto createRegion(string Nombre)
        {
            var Region = new RegionDto();
            Region.Nombre = Nombre.ToUpper();
            Region.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(Region);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "REGION", "Se ha creado la Región", null, Region.Nombre, usuario, Region.Id);
                context.Add(accion);
                context.SaveChanges();
                return Region;
            }
        }

        public bool existeRegion(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Regiones.Any(x => String.Equals(x.Nombre.Trim(),unNombre.Trim()));

            }
        }
        public bool existeOtraRegionIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Regiones.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }

    }
}