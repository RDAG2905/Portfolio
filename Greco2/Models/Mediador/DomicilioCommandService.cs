using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.Mediador
{
    public class DomicilioCommandService
    {
        DomicilioMediadorDto DomicilioMediadorModificado;
        DomicilioMediadorDto DomicilioMediadorOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public List<DomicilioMediadorSP> updateDomicilioMediador(DomicilioMediadorDto DomicilioMediadorDto)
        {
            DomicilioMediadorModificado = DomicilioMediadorDto;
            List<DomicilioMediadorSP> lista = new List<DomicilioMediadorSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {

                DomicilioMediadorDto DomicilioMediador = context.getDomiciliosMediadores(true).Where(t => t.Id == DomicilioMediadorDto.Id).FirstOrDefault();
                DomicilioMediadorOriginal = DomicilioMediador;

                prepararCambios(DomicilioMediadorModificado, DomicilioMediadorOriginal, context);
                DomicilioMediador.Domicilio = DomicilioMediadorDto.Domicilio.Trim().ToUpper();
                DomicilioMediador.MediadorId = DomicilioMediadorDto.MediadorId;
                DomicilioMediador.Activo = DomicilioMediadorDto.Activo;
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomicilioMediadorPorId @id", new SqlParameter("@id", DomicilioMediadorOriginal.Id))
                        .ToList();
                
            }
            return lista;
        }

        private List<CommonChangeLoggerDto> prepararCambios(DomicilioMediadorDto modificado, DomicilioMediadorDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Activo != original.Activo)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "DOMICILIO MEDIADOR", "Se ha modificado el campo de Activación", original.Activo ? "Activo" : "Inactivo", modificado.Activo ? "Activo" : "Inactivo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Domicilio != original.Domicilio)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "DOMICILIO MEDIADOR", "Se ha modificado el Domicilio", original.Domicilio, modificado.Domicilio, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            if (modificado.MediadorId != original.MediadorId)
            {
                var mediadorNuevo = (modificado != null && modificado.MediadorId > 0) ? context.Mediadores.Where(r => r.Id == modificado.MediadorId).FirstOrDefault().Nombre : "";
                var mediadorAnterior = (original != null && original.MediadorId > 0) ? context.Mediadores.Where(r => r.Id == original.MediadorId).FirstOrDefault().Nombre : "";
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "DOMICILIO MEDIADOR", "Se ha cambiado el Mediador", mediadorAnterior, mediadorNuevo, usuario, modificado.Id);
                listLoggers.Add(logger3);
            }
           
            return listLoggers;
        }

        public List<DomicilioMediadorSP> createDomicilioMediador(DomicilioMediadorDto DomicilioMediador)
        
        {

            List<DomicilioMediadorSP> lista = new List<DomicilioMediadorSP>();
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(DomicilioMediador);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "DOMICILIO MEDIADOR", "Se ha creado el Domicilio del Mediador", null, DomicilioMediador.Domicilio, usuario, DomicilioMediador.Id);
                context.Add(accion);
                context.SaveChanges();
               
                lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomicilioMediadorPorId @id", new SqlParameter("@id", DomicilioMediador.Id))
                        .ToList();
                
            }
            return lista;
        }

        public bool existeDomicilioMediador(string unNombre,int mediadorId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.DomiciliosMediadores.Any(x => String.Equals(x.Domicilio.Trim(), unNombre.Trim()) && x.MediadorId == mediadorId );

            }
        }

        public bool existeOtroDomicilioIgual(string unNombre, int id,int mediadorId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.DomiciliosMediadores.Any(x => String.Equals(x.Domicilio.Trim(), unNombre.Trim()) && x.MediadorId == mediadorId  && x.Id != id);

            }
        }
    }
}