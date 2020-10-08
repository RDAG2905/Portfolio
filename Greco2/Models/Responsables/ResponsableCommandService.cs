using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Responsables
{
    public class ResponsableCommandService
    {
        ResponsableDto ResponsableModificado;
        ResponsableDto ResponsableOriginal;
        List<CommonChangeLoggerDto> listLoggers = null;

        public List<ResponsableDto> updateResponsable(ResponsableDto ResponsableDto)
        {
            ResponsableModificado = ResponsableDto;
            List<ResponsableDto> lista = new List<ResponsableDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {

                ResponsableDto Responsable = context.getResponsables(true).Where(t => t.Id == ResponsableDto.Id).FirstOrDefault();
                ResponsableOriginal = Responsable;

                prepararCambios(ResponsableModificado, ResponsableOriginal, context);
                var idResponsableAModificarr = Responsable.Id;
                Responsable.Nombre = ResponsableDto.Nombre.Trim();
                Responsable.Apellido = ResponsableDto.Apellido.Trim();
                Responsable.UmeId = ResponsableDto.UmeId.ToLower();
                Responsable.TipoResponsable = ResponsableDto.TipoResponsable;
                Responsable.Rol = ResponsableDto.Rol;
                Responsable.Estudio_Id = ResponsableDto.Estudio_Id;
                Responsable.Deleted = ResponsableDto.Deleted;
                //save changes to database
                context.SaveChanges();
                context.CommonChangeLogger.AddRange(listLoggers);
                context.SaveChanges();
                lista.Add(Responsable);
                //lista = context.Database
                //        .SqlQuery<ResponsableDto>("GetResponsableDtoorId @id", new SqlParameter("@id", Responsable.Id))
                //        .ToList();
            }
            return lista;
        }

        private List<CommonChangeLoggerDto> prepararCambios(ResponsableDto modificado, ResponsableDto original, NuevoDbContext context)
        {
            var usuario = HttpContext.Current.User.Identity.Name;
            listLoggers = new List<CommonChangeLoggerDto>();
            if (modificado.Deleted != original.Deleted)
            {
                var logger1 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el campo de Activación", original.Deleted ? "Inactivo" : "Activo", modificado.Deleted ? "Inactivo" : "Activo", usuario, modificado.Id);
                listLoggers.Add(logger1);
            }
            if (modificado.Nombre != original.Nombre)
            {
                var logger2 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el Nombre", original.Nombre, modificado.Nombre, usuario, modificado.Id);
                listLoggers.Add(logger2);
            }
            if (modificado.Apellido != original.Apellido)
            {
                var logger3 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el Apellido", original.Apellido, modificado.Apellido, usuario, modificado.Id);
                listLoggers.Add(logger3);
            }
            if (modificado.UmeId != original.UmeId)
            {
                var logger4 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el Usuario", original.UmeId, modificado.UmeId, usuario, modificado.Id);
                listLoggers.Add(logger4);
            }
            if (modificado.TipoResponsable != original.TipoResponsable)
            {
                var logger5 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el Tipo de Responsable", original.TipoResponsable, modificado.TipoResponsable, usuario, modificado.Id);
                listLoggers.Add(logger5);
            }
            if (modificado.Rol != original.Rol)
            {
                var logger7 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha modificado el Rol", original.Rol, modificado.Rol, usuario, modificado.Id);
                listLoggers.Add(logger7);
            }
            if (modificado.Estudio_Id != original.Estudio_Id)
            {
                var estudioNuevo = (modificado != null && modificado.Estudio_Id > 0) ? context.Estudios.Where(r => r.Id == modificado.Estudio_Id).FirstOrDefault().Nombre : "";
                var estudioAnterior = (original != null && original.Estudio_Id > 0) ? context.Estudios.Where(r => r.Id == original.Estudio_Id).FirstOrDefault().Nombre : "";
                var logger6 = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha cambiado el Estudio", estudioAnterior, estudioNuevo, usuario, modificado.Id);
                listLoggers.Add(logger6);
            }
            
            return listLoggers;
        }

        public List<ResponsableDto> createResponsable(ResponsableDto Responsable)
        {

            List<ResponsableDto> lista = new List<ResponsableDto>();
            var usuario = HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(Responsable);
                context.SaveChanges();
                var accion = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha creado el Responsable", null, Responsable.Nombre + ' ' + Responsable.Apellido , usuario, Responsable.Id);
                context.Add(accion);
                context.SaveChanges();
                var sub = context.Responsables.Where(x => x.Id == Responsable.Id).FirstOrDefault();
                lista.Add(sub);

            }
            return lista;
        }

        public bool existeResponsable(string unUsuario)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Responsables.Any(x => String.Equals(x.UmeId.Trim(), unUsuario.Trim()));

            }
        }

        public bool existeOtroResponsableIgual(ResponsableDto responsable)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Responsables.Any(x => String.Equals(x.UmeId.Trim(), responsable.UmeId.Trim()) && x.Id != responsable.Id);

            }
        }
    }
}