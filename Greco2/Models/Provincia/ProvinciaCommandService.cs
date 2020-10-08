using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Provincia
{
    public class ProvinciaCommandService
    {
        ProvinciaDto provinciaModificado;
        ProvinciaDto provinciaOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public ProvinciaDto updateProvincia(ProvinciaDto ProvinciaDto)
        {
            provinciaModificado = ProvinciaDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                ProvinciaDto provincia = context.getProvincias(true).Where(t => t.Id == ProvinciaDto.Id).FirstOrDefault();
                provinciaOriginal = provincia;

                prepararCambios(provinciaModificado, provinciaOriginal, context);
                var idprovinciaAModificarr = provincia.Id;
                provincia.Nombre = ProvinciaDto.Nombre.Trim().ToUpper();
                provincia.Deleted = ProvinciaDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return provincia;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(ProvinciaDto modificado, ProvinciaDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "PROVINCIA", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "PROVINCIA", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public ProvinciaDto createProvincia(string Nombre)
        {
            var provincia = new ProvinciaDto();
            provincia.Nombre = Nombre.Trim().ToUpper();
            provincia.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(provincia);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "PROVINCIA", "Se ha creado la PROVINCIA", null, provincia.Nombre, usuario, provincia.Id);
                context.Add(accion);
                context.SaveChanges();
                return provincia;
            }
        }

        public bool existeProvincia(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Provincias.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }

        public bool existeOtraProvinciaIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Provincias.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }

    }
}