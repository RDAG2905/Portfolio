using Greco2.Model;
using Greco2.Models.Estudios;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estudio
{
    public class EstudioCommandService
    {
        EstudioDto estudioModificado;
        EstudioDto estudioOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public EstudioDto updateEstudio(EstudioDto estudioDto)
        {
            estudioModificado = estudioDto;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                EstudioDto estudio = context.getEstudios(true).Where(t => t.Id == estudioDto.Id).FirstOrDefault();
                estudioOriginal = estudio;

                prepararCambios(estudioModificado, estudioOriginal, context);
                var idestudioAModificarr = estudio.Id;
                estudio.Nombre = estudioDto.Nombre.Trim().ToUpper();
                estudio.Deleted = estudioDto.Deleted;

                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                return estudio;
            }

        }

        private List<CommonChangeLoggerDto> prepararCambios(EstudioDto modificado, EstudioDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "ESTUDIO", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "ESTUDIO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }

            return listLoggers;
        }

        public EstudioDto createEstudio(string Nombre)
        {
            var estudio = new EstudioDto();
            estudio.Nombre = Nombre.Trim().ToUpper();            
            estudio.Deleted = false;
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {             
                    context.Add(estudio);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "ESTUDIO", "Se ha creado el Estudio", null, estudio.Nombre, usuario, estudio.Id);
                    context.Add(accion);
                    context.SaveChanges();
                    return estudio;
                
            }
        }

        public bool existeEstudio(string unNombre)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Estudios.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));

            }
        }

        public bool existeOtroEstudioIgual(EstudioDto estudio)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Estudios.Any(x => String.Equals(x.Nombre.Trim(), estudio.Nombre.Trim()) && x.Id != estudio.Id);

            }
        }

        public string EliminarEstudio(int id)
        {
              using (NuevoDbContext context = new NuevoDbContext())
              {
                    EstudioDto estudio = context.getEstudios(true)
                                                  .Where(t => t.Id == id)
                                                  .FirstOrDefault();
                    context.Remove(estudio);
                    context.SaveChanges();
                    return ("Registro eliminado con éxito");

              }
        }

        //public tieneRelaciones() {
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        EstudioDto estudio = context.
        //                                    .Where(t => t.Id == id)
        //                                    .FirstOrDefault();
        //        context.Remove(estudio);
        //        context.SaveChanges();
        //        return ("Registro eliminado con éxito");

        //    }

        //}
    }
}