using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.Organismo
{
    public class OrganismoCommandService
    {
        OrganismoDto OrganismoModificado;
        OrganismoDto OrganismoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public List<OrganismoSP> updateOrganismo(OrganismoDto OrganismoDto)
        {
            List<OrganismoSP> lista = new List<OrganismoSP>();
            OrganismoModificado = OrganismoDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                OrganismoDto Organismo = context.getOrganismos(true).Where(t => t.Id == OrganismoDto.Id).FirstOrDefault();
                OrganismoOriginal = Organismo;

                prepararCambios(OrganismoModificado, OrganismoOriginal, context);
                var idOrganismoAModificarr = Organismo.Id;
                Organismo.Nombre = OrganismoDto.Nombre.Trim().ToUpper();
                Organismo.Activo = OrganismoDto.Activo;
                Organismo.Provincia_Id = OrganismoDto.Provincia_Id;
                Organismo.Localidad_Id = OrganismoDto.Localidad_Id;
                Organismo.Region_Id = OrganismoDto.Region_Id;
                
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                lista = context.Database
                        .SqlQuery<OrganismoSP>("GetOrganismosPorId @id", new SqlParameter("@id", Organismo.Id))
                        .ToList();
                
            }
            return lista;

        }

        private List<CommonChangeLoggerDto> prepararCambios(OrganismoDto modificado, OrganismoDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Activo != original.Activo)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha modificado el campo de Activación", original.Activo ? "Inactivo" : "Activo", modificado.Activo ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            if (modificado.Provincia_Id != original.Provincia_Id)
            {
                var provinciaNueva = (modificado != null && modificado.Provincia_Id > 0) ? context.Provincias.Where(r => r.Id == modificado.Provincia_Id).FirstOrDefault().Nombre : "";
                var provinciaAnterior = (original != null && original.Provincia_Id > 0) ? context.Provincias.Where(r => r.Id == original.Provincia_Id).FirstOrDefault().Nombre : "";
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha cambiado la Provincia", provinciaAnterior, provinciaNueva, usuario, modificado.Id);
                listLoggers.Add(logger3);
            }
            if (modificado.Localidad_Id != original.Localidad_Id)
            {
                var localidadNueva = (modificado != null && modificado.Localidad_Id > 0) ? context.Localidades.Where(r => r.Id == modificado.Localidad_Id).FirstOrDefault().Nombre : "";
                var localidadAnterior = (original != null && original.Localidad_Id > 0) ? context.Localidades.Where(r => r.Id == original.Localidad_Id).FirstOrDefault().Nombre : "";
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha cambiado la Localidad", localidadAnterior, localidadNueva, usuario, modificado.Id);
                listLoggers.Add(logger4);
            }
            if (modificado.Region_Id != original.Region_Id)
            {
                var regionNueva = (modificado != null && modificado.Region_Id > 0) ? context.Regiones.Where(r => r.Id == modificado.Region_Id).FirstOrDefault().Nombre : "";
                var regionAnterior = (original != null && original.Region_Id > 0) ? context.Regiones.Where(r => r.Id == original.Region_Id).FirstOrDefault().Nombre : "";
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha cambiado la Región", regionAnterior,regionNueva, usuario, modificado.Id);
                listLoggers.Add(logger4);
            }

            return listLoggers;
        }

        public List<OrganismoSP> createOrganismo(string Nombre, int provinciaId, int localidadId, int regionId)
        {

            var organismo = new OrganismoDto();
            organismo.Nombre = Nombre.Trim().ToUpper();
            organismo.Provincia_Id = provinciaId;
            organismo.Localidad_Id = localidadId;
            organismo.Region_Id = regionId;
            organismo.Activo = false;
            List<OrganismoSP> lista = new List<OrganismoSP>();
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(organismo);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha creado el Organismo", null, organismo.Nombre, usuario, organismo.Id);
                context.Add(accion);
                context.SaveChanges();
                lista = context.Database
                        .SqlQuery<OrganismoSP>("GetOrganismosPorId @id", new SqlParameter("@id", organismo.Id))
                        .ToList();                
            }
            return lista;
        }

        public bool existeOrganismo(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Organismos.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }
        public bool existeOtroOrganismoIgual(string unNombre, int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Organismos.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.Id != id);

            }
        }

    }
}