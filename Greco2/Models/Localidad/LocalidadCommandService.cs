using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Greco2.Models.Localidad
{
    public class LocalidadCommandService
    {
        LocalidadDto elementoModificado;
        LocalidadDto elementoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public async Task<List<LocalidadSP>> updatelocalidad(LocalidadDto LocalidadDto)
        {
            elementoModificado = LocalidadDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                LocalidadDto localidad = context.getLocalidades(true).Where(t => t.Id == LocalidadDto.Id).FirstOrDefault();
                elementoOriginal = localidad;
                
                prepararCambios(elementoModificado, elementoOriginal, context);
                var idlocalidadAModificar = localidad.Id;
                localidad.Nombre = LocalidadDto.Nombre.Trim().ToUpper();
                localidad.Deleted = LocalidadDto.Deleted;
                localidad.ProvinciaId = LocalidadDto.ProvinciaId;
                
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return await context.Database
                        .SqlQuery<LocalidadSP>("GetLocalidadesPorId @id", new SqlParameter("@id", idlocalidadAModificar))
                        .ToListAsync();
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(LocalidadDto modificado, LocalidadDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "LOCALIDAD", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "LOCALIDAD", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            
            if (modificado.ProvinciaId != original.ProvinciaId)
            {
                var provinciaNueva = (modificado != null && modificado.ProvinciaId > -1) ? context.Provincias.Where(r => r.Id == modificado.ProvinciaId).FirstOrDefault().Nombre : "";
                var provinciaAnterior = (original != null && original.ProvinciaId > -1) ? context.Provincias.Where(r => r.Id == original.ProvinciaId).FirstOrDefault().Nombre : "";
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "LOCALIDAD", "Se ha cambiado la Provincia", provinciaAnterior, provinciaNueva, usuario, modificado.Id);
                listLoggers.Add(logger4);
            }
            
            return listLoggers;
        }




        public void deletelocalidad(int id)
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {

                LocalidadDto localidad = context.getLocalidades(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                context.Remove(localidad);
                context.SaveChanges();
            }
        }



        public List<LocalidadSP> createlocalidad(LocalidadDto localidad)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            List<LocalidadSP> lista = new List<LocalidadSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(localidad);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "LOCALIDAD", "Se ha creado una Localidad", null, localidad.Nombre, usuario, localidad.Id);
                context.Add(accion);
                context.SaveChanges();
                return lista = context.Database
                        .SqlQuery<LocalidadSP>("GetLocalidadesPorId @id", new SqlParameter("@id", localidad.Id))
                        .ToList();
                
            }
        }

        public bool existeLocalidad(string unNombre,int provinciaId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Localidades.Any(x =>String.Equals(x.Nombre.Trim(),unNombre.Trim())&& x.ProvinciaId == provinciaId);

            }
        }

        public bool existeOtraLocalidadIgual(string unNombre, int id,int provinciaId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Localidades.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) 
                                                && x.ProvinciaId == provinciaId 
                                                && x.Id != id);

            }
        }
    }
}