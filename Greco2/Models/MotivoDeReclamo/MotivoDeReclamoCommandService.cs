using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.MotivoDeReclamo
{
    public class MotivoDeReclamoCommandService
    {
        MotivoDeReclamoDto MotivoDeReclamoModificado;
        MotivoDeReclamoDto MotivoDeReclamoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public List<MotivoDeReclamoSP> updateMotivoDeReclamo(MotivoDeReclamoDto MotivoDeReclamoDto)
        {
            MotivoDeReclamoModificado = MotivoDeReclamoDto;
            List<MotivoDeReclamoSP> lista = new List<MotivoDeReclamoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
              
                MotivoDeReclamoDto MotivoDeReclamo = context.getMotivosDeReclamo(true).Where(t => t.Id == MotivoDeReclamoDto.Id).FirstOrDefault();
                MotivoDeReclamoOriginal = MotivoDeReclamo;

                prepararCambios(MotivoDeReclamoModificado, MotivoDeReclamoOriginal, context);
                var idMotivoDeReclamoAModificarr = MotivoDeReclamo.Id;
                MotivoDeReclamo.Nombre = MotivoDeReclamoDto.Nombre.Trim().ToUpper();
                MotivoDeReclamo.Deleted = MotivoDeReclamoDto.Deleted;
                MotivoDeReclamo.servicioId = MotivoDeReclamoDto.servicioId;
                MotivoDeReclamo.tipoProcesoId = MotivoDeReclamoDto.tipoProcesoId;
                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                lista = context.Database
                        .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorId @id", new SqlParameter("@id", MotivoDeReclamo.Id))
                        .ToList();
            }
            return lista;
        }

        private List<CommonChangeLoggerDto> prepararCambios(MotivoDeReclamoDto modificado, MotivoDeReclamoDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            if (modificado.servicioId != original.servicioId)
            {
                var servicioNuevo = (modificado != null && modificado.servicioId > 0) ? context.Servicios.Where(r => r.Id == modificado.servicioId).FirstOrDefault().Nombre : "";
                var servicioAnterior = (original != null && original.servicioId > 0) ? context.Servicios.Where(r => r.Id == original.servicioId).FirstOrDefault().Nombre : "";
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha cambiado el Servicio", servicioAnterior, servicioNuevo, usuario, modificado.Id);
                listLoggers.Add(logger3);
            }
            if (modificado.tipoProcesoId != original.tipoProcesoId)
            {
                var procesoNuevo = (modificado != null && modificado.tipoProcesoId > 0) ? context.TiposDeProceso.Where(r => r.Id == modificado.tipoProcesoId).FirstOrDefault().Nombre : "";
                var procesoAnterior = (original != null && original.tipoProcesoId > 0) ? context.TiposDeProceso.Where(r => r.Id == original.tipoProcesoId).FirstOrDefault().Nombre : "";
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha cambiado el Tipo de Proceso", procesoAnterior, procesoNuevo, usuario, modificado.Id);
                listLoggers.Add(logger4);
            }

            return listLoggers;
        }

        public List<MotivoDeReclamoSP> createMotivoDeReclamo(MotivoDeReclamoDto MotivoDeReclamo)
        {
           
            List<MotivoDeReclamoSP> lista = new List<MotivoDeReclamoSP>();
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(MotivoDeReclamo);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha creado el MotivoDeReclamo", null, MotivoDeReclamo.Nombre, usuario, MotivoDeReclamo.Id);
                context.Add(accion);
                context.SaveChanges();
                lista = context.Database
                  .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorId @motivoId", new SqlParameter("@motivoId", MotivoDeReclamo.Id))
                  .ToList();
                
            }
            return lista;
        }

        public bool existeMotivoDeReclamo(string unNombre,int unServicioId,int unTipoProcesoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.MotivosDeReclamo.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim())
                                                     && x.servicioId == unServicioId 
                                                     && x.tipoProcesoId == unTipoProcesoId
                );

            }
        }

        public bool existeOtroMotivoDeReclamoIgual(string unNombre, int id, int unServicioId, int unTipoProcesoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
              
                return context.MotivosDeReclamo.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim())
                                                     && x.servicioId == unServicioId
                                                     && x.tipoProcesoId == unTipoProcesoId
                                                     && x.Id != id );

            }
        }
    }
}