using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greco2.Model;
using Greco2.Models.Denunciante;
using Greco2.Models.Log;
using Microsoft.Ajax.Utilities;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteCommandService
    {
       
        List<CommonChangeLoggerDto> listLoggers = null;

        public DenuncianteDto getDenuncianteEditado(DenuncianteDto denuncianteDto)
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {

                DenuncianteDto existingTest = context.getDenunciantes(true).Where(t => t.DenuncianteId == denuncianteDto.DenuncianteId).FirstOrDefault();

                prepararCambios(denuncianteDto, existingTest, context);

                existingTest.nombre = denuncianteDto.nombre;
                existingTest.apellido = denuncianteDto.apellido;
                existingTest.NroDocumento = denuncianteDto.NroDocumento;

                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();

                var idEditado = denuncianteDto.DenuncianteId;
                var denunciante = context.Denunciantes.Where(e => e.DenuncianteId == idEditado).FirstOrDefault();
                return denunciante;
            }

        }


        public void updateDenunciante(DenuncianteDto denuncianteDto) {

            using (NuevoDbContext context = new NuevoDbContext()) {
               
                DenuncianteDto existingTest = context.getDenunciantes(true).Where(t=>t.DenuncianteId == denuncianteDto.DenuncianteId).FirstOrDefault();

                prepararCambios(denuncianteDto, existingTest, context);

                existingTest.nombre = denuncianteDto.nombre;
                existingTest.apellido = denuncianteDto.apellido;
                existingTest.NroDocumento =denuncianteDto.NroDocumento;
                
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                
            }

        }


        private List<CommonChangeLoggerDto> prepararCambios(DenuncianteDto modificado, DenuncianteDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.apellido != original.apellido)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha modificado el Apellido", original.apellido, modificado.apellido, usuario, modificado.DenuncianteId);
                listLoggers.Add(logger1);
            }
            if (modificado.nombre != original.nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha modificado el Nombre", original.nombre, modificado.nombre, usuario, modificado.DenuncianteId);
                listLoggers.Add(logger2);
            }
            if (modificado.NroDocumento != original.NroDocumento)
            {
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha modificado el Nro. de Documento", original.NroDocumento, modificado.NroDocumento, usuario, modificado.DenuncianteId);
                listLoggers.Add(logger3);
            }

            return listLoggers;
        }

        public void deleteDenunciante(DenuncianteDto denuncianteDto)
        {
            var usuario = HttpContext.Current.User.Identity.Name;

            using (NuevoDbContext context = new NuevoDbContext())
            {
                //load task from database
                DenuncianteDto existingTest = context.getDenunciantes(true)
                                              .Where(t => t.DenuncianteId == denuncianteDto.DenuncianteId)
                                              .FirstOrDefault();

                context.Remove(existingTest);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha eliminado al Denunciante", "Denunciante : " + denuncianteDto.apellido + "," + denuncianteDto.nombre + "," + " dni: " + denuncianteDto.NroDocumento,"ELIMINADO", usuario, denuncianteDto.DenuncianteId);
                context.Add(accion);
                context.SaveChanges();
            }
        }

        public int? createDenunciante(DenuncianteDto test) {

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (!existeDenunciante(test, context))
                {
                    context.Add(test);
                    context.SaveChanges();
                    return test.DenuncianteId;
                    //return context.Denunciantes.ToList().Where(d => d.DenuncianteId == test.DenuncianteId).Single() ;
                }
                else
                {
                    return null;
                }

            }
            
        }

        public DenuncianteDto crearDenunciante(DenuncianteDto test)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                    context.Add(test);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha creado al Denunciante", null,"Denunciante" + test.apellido + "," + test.nombre + "," + " dni: " + test.NroDocumento, usuario, test.DenuncianteId);
                    context.Add(accion);
                    context.SaveChanges();
                return test;
            }

        }

        public bool existeDenunciante(DenuncianteDto den,NuevoDbContext context) {
            return context.Denunciantes.Where(d => d.NroDocumento == den.NroDocumento
                                       && d.apellido == den.apellido
                                       && d.nombre == den.nombre).Any();
                                       
            
        }

        public bool existeOtroDenuncianteIgual(int id,string dni)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Denunciantes.Any(x => String.Equals(x.NroDocumento.Trim(), dni.Trim()) && x.DenuncianteId != id);

            }
        }

        public bool existeDni(string dni)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Denunciantes.Any(x => String.Equals(x.NroDocumento.Trim(), dni.Trim()));

            }
        }

    }
}