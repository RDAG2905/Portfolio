using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.Estado
{
    public class SubEstadoCommandService
    {
        SubEstadoDto SubEstadoModificado;
        SubEstadoDto SubEstadoOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public List<SubEstadoSP> updateSubEstado(SubEstadoDto SubEstadoDto)
        {
            SubEstadoModificado = SubEstadoDto;
            List<SubEstadoSP> lista = new List<SubEstadoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {

                SubEstadoDto SubEstado = context.GetSubEstados(true).Where(t => t.Id == SubEstadoDto.Id).FirstOrDefault();
                SubEstadoOriginal = SubEstado;

                prepararCambios(SubEstadoModificado, SubEstadoOriginal, context);
                var idSubEstadoAModificarr = SubEstado.Id;
                SubEstado.Nombre = SubEstadoDto.Nombre.Trim().ToUpper();
                SubEstado.Deleted = SubEstadoDto.Deleted;
                SubEstado.CierraDenuncia = SubEstadoDto.CierraDenuncia;
                SubEstado.EstadoId = SubEstadoDto.EstadoId;
                
                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                lista = context.Database
                        .SqlQuery<SubEstadoSP>("GetSubEstadosPorId @id", new SqlParameter("@id", SubEstado.Id))
                        .ToList();
            }
            return lista;
        }

        private List<CommonChangeLoggerDto> prepararCambios(SubEstadoDto modificado, SubEstadoDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            if (modificado.EstadoId != original.EstadoId)
            {
                //var estadoNuevo = (modificado != null && modificado.EstadoId > 0) ? context.SubEstados.Where(r => r.Id == modificado.EstadoId).FirstOrDefault().Nombre : "";
                //var estadoAnterior = (original != null && original.EstadoId > 0) ? context.SubEstados.Where(r => r.Id == original.EstadoId).FirstOrDefault().Nombre : "";
                var estadoNuevo = (modificado != null && modificado.EstadoId > 0) ? context.Estados.Where(r => r.Id == modificado.EstadoId).FirstOrDefault().TipoEstado : "";
                var estadoAnterior = (original != null && original.EstadoId > 0) ? context.Estados.Where(r => r.Id == original.EstadoId).FirstOrDefault().TipoEstado : "";
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha cambiado la Etapa Procesal del Estado: " + modificado.Nombre, estadoAnterior, estadoNuevo, usuario, modificado.Id);
                listLoggers.Add(logger3);
            }
            if (modificado.CierraDenuncia != original.CierraDenuncia)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha modificado el campo CierraDenuncia", original.CierraDenuncia ? "Inactivo" : "Activo", modificado.CierraDenuncia ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            return listLoggers;
        }

        public List<SubEstadoSP> createSubEstado(SubEstadoDto SubEstado)
        //public List<SubEstadoDto> createSubEstado(SubEstadoDto SubEstado)
        {

            List<SubEstadoSP> lista = new List<SubEstadoSP>();
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(SubEstado);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha creado el SubEstado", null, SubEstado.Nombre, usuario, SubEstado.Id);
                context.Add(accion);
                context.SaveChanges();
                //var sub = context.SubEstados.Where(x => x.Id == SubEstado.Id).FirstOrDefault();
                //lista.Add(sub);
                lista = context.Database
                  .SqlQuery<SubEstadoSP>("GetSubEstadosPorId @motivoId", new SqlParameter("@motivoId", SubEstado.Id))
                  .ToList();

            }
            return lista;
        }

        public bool existeSubEstado(string unNombre,int etapaProcesalId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.SubEstados.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.EstadoId == etapaProcesalId);

            }
        }

        public bool existeOtroSubEstadoIgual(string unNombre, int id,int etapaProcesalId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.SubEstados.Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()) && x.EstadoId == etapaProcesalId  && x.Id != id );

            }
        }
    }
}