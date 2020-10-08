using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.DatosCoprec;
using Greco2.Models.Denuncia;
using Greco2.Models.Denunciante;
using Greco2.Models.Evento;
using Greco2.Models.Expediente;
using Greco2.Models.Grupo;
using Greco2.Models.Historial;
using Greco2.Models.Reclamo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using System.Web.Mvc;
using System.Web.UI;

namespace Greco2.Controllers
{
    
    
    public class DenunciasController : Controller
    {
        // GET: Denuncias
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult Index()
        {
            return View();
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult TraerGrupoDenunciantes(int? grupoId) {
            GrupoSP grupo = new GrupoSP();
            if (grupoId != null && grupoId > 0)
            {               
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    List<DenuncianteGrupoSP> grupoDenunciantes = context.Database
                                                            .SqlQuery<DenuncianteGrupoSP>("GetGrupoDenunciantesPorIdGrupo @grupoId", new SqlParameter("@grupoId", grupoId))
                                                            .ToList();
                    if (grupoDenunciantes != null && grupoDenunciantes.Any())
                    {
                        grupo.Id = grupoDenunciantes.First().GrupoDto_Id;
                        grupo.grupoDenunciantes = grupoDenunciantes;

                    }
                   
                }
                
            }
            return PartialView("ListGrupoDenunciantesSP", grupo);
           
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult TraerDatosCoprec(int? idDatosCoprec)
        {
            if (idDatosCoprec == null || !(idDatosCoprec > 0))
            {
                return PartialView("DatosCoprec", new DatosCoprecDto());
            }
            else
            {
                DatosCoprecDto datosCoprec = new DatosCoprecDto();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    datosCoprec = context.DatosCoprec
                                         .Where(d => d.Id == idDatosCoprec)
                                         .FirstOrDefault();
                }
                return PartialView("DatosCoprec",datosCoprec);
            }
            
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public async Task<ActionResult> GetDatosCoprec(int? idDenuncia)
        {
            if (idDenuncia == null || idDenuncia == 0)
            {
                return  PartialView("DatosCoprec", new DatosCoprec());
            }
            else
            {
                DatosCoprec datosCoprec = new DatosCoprec();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    datosCoprec = await context.Database
                                                .SqlQuery<DatosCoprec>("select nroGestionCoprec,honorariosCoprec,fechaGestionHonorarios,montoAcordado,arancel,fechaGestionArancel from tdenuncias where DenunciaId = @denunciaId",
                                                new SqlParameter("@denunciaId", idDenuncia)).FirstOrDefaultAsync();
                    var denuncia = await context.Denuncias.Where(x => x.DenunciaId == idDenuncia).FirstOrDefaultAsync();
                    if (denuncia.DELETED == true)
                    {
                        return PartialView("DatosCoprecReadOnly", datosCoprec);
                    }
                }
               
                return PartialView("DatosCoprec", datosCoprec);
            }

        }

        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //public async Task<ActionResult> GuardarDatosCoprec(DatosCoprecDto datosCoprec)
        //{
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {

        //        context.Add(datosCoprec);
        //        await context.SaveChangesAsync();
        //        var denuncia = await context.Database.ExecuteSqlCommandAsync("UpdateDenunciaDatosCoprec @datosCoprecId,@denunciaId", new SqlParameter("@datosCoprecId", datosCoprec.Id), new SqlParameter("@denunciaId", datosCoprec.DenunciaId));


        //    }
        //        return PartialView("DatosCoprec",datosCoprec);
        //}

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> GuardarDatosCoprec(DatosCoprec datosCoprec)
        {
         var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            if (!String.IsNullOrEmpty(usuario))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    var NroGestion = (datosCoprec.NroGestionCoprec == null)?"":datosCoprec.NroGestionCoprec;
                    var Honorarios = (datosCoprec.HonorariosCoprec == null) ? "":datosCoprec.HonorariosCoprec;
                    var FechaGestionHonorarios = datosCoprec.FechaGestionHonorarios;/*(datosCoprec.FechaGestionHonorarios == null)? "":datosCoprec.FechaGestionHonorarios.ToString();*/
                    var MontoAcordado = (datosCoprec.MontoAcordado == null)?"":datosCoprec.MontoAcordado;
                    var Arancel = (datosCoprec.Arancel == null)?"":datosCoprec.Arancel;
                    var FechaGestionArancel = datosCoprec.FechaGestionArancel;
                    var IdDenuncia = datosCoprec.DenunciaId;

                    if (datosCoprec.DenunciaId > 0)
                    {
                        DenunciaDto denunciaAntesDelCambio = await context.getDenuncias(true).Where(x => x.DenunciaId == IdDenuncia).FirstOrDefaultAsync();
                        if (FechaGestionHonorarios != null && FechaGestionArancel != null)
                        {
                            context.Database.ExecuteSqlCommand("UpdateDatosCoprecDenuncia @NroGestion,@Honorarios,@FechaGestionHonorarios,@MontoAcordado,@Arancel,@FechaGestionArancel,@IdDenuncia",
                                                        new SqlParameter("@NroGestion", NroGestion),
                                                        new SqlParameter("@Honorarios", Honorarios),
                                                        new SqlParameter("@FechaGestionHonorarios", FechaGestionHonorarios),
                                                        new SqlParameter("@MontoAcordado", MontoAcordado),
                                                        new SqlParameter("@Arancel", Arancel),
                                                        new SqlParameter("@FechaGestionArancel", FechaGestionArancel),
                                                        new SqlParameter("@IdDenuncia", IdDenuncia));
                        } else
                            if (FechaGestionHonorarios == null && FechaGestionArancel != null) {
                            context.Database.ExecuteSqlCommand("UpdateDatosCoprecSinFechaGestionHonorarios @NroGestion,@Honorarios,@MontoAcordado,@Arancel,@FechaGestionArancel,@IdDenuncia",
                                                        new SqlParameter("@NroGestion", NroGestion),
                                                        new SqlParameter("@Honorarios", Honorarios),                                 
                                                        new SqlParameter("@MontoAcordado", MontoAcordado),
                                                        new SqlParameter("@Arancel", Arancel),
                                                        new SqlParameter("@FechaGestionArancel", FechaGestionArancel),
                                                        new SqlParameter("@IdDenuncia", IdDenuncia));
                        }
                        else
                            if (FechaGestionHonorarios != null && FechaGestionArancel == null) {
                            context.Database.ExecuteSqlCommand("UpdateDatosCoprecSinFechaGestionArancel @NroGestion,@Honorarios,@FechaGestionHonorarios,@MontoAcordado,@Arancel,@IdDenuncia",
                                                       new SqlParameter("@NroGestion", NroGestion),
                                                       new SqlParameter("@Honorarios", Honorarios),
                                                       new SqlParameter("@FechaGestionHonorarios", FechaGestionHonorarios),
                                                       new SqlParameter("@MontoAcordado", MontoAcordado),
                                                       new SqlParameter("@Arancel", Arancel),
                                                       new SqlParameter("@IdDenuncia", IdDenuncia));
                        }
                        else
                            if (FechaGestionHonorarios == null && FechaGestionArancel == null)
                        {
                            context.Database.ExecuteSqlCommand("UpdateDatosCoprecSinFechas @NroGestion,@Honorarios,@MontoAcordado,@Arancel,@IdDenuncia",
                                                        new SqlParameter("@NroGestion", NroGestion),
                                                        new SqlParameter("@Honorarios", Honorarios),
                                                        new SqlParameter("@MontoAcordado", MontoAcordado),
                                                        new SqlParameter("@Arancel", Arancel),
                                                        new SqlParameter("@IdDenuncia", IdDenuncia));
                        }

                        DatosCoprec datosCoprecActualizados = await context.Database
                                                .SqlQuery<DatosCoprec>("select nroGestionCoprec,honorariosCoprec,fechaGestionHonorarios,montoAcordado,arancel,fechaGestionArancel from tdenuncias where DenunciaId = @denunciaId",
                                                new SqlParameter("@denunciaId", IdDenuncia)).FirstOrDefaultAsync();

                        
                        AuditarDatosCoprec(denunciaAntesDelCambio, datosCoprecActualizados, context, usuario);
                        //return Json(datosCoprecActualizados);
                        //return PartialView("DatosActualizados");
                        return PartialView("DatosCoprec", datosCoprecActualizados);

                    }
                    else {
                        return RedirectToAction("SesionCulminada", "Home", null);
                    }
                }
            }
            else {
                return RedirectToAction("SesionCulminada","Home",null);
            }
            
            //return PartialView("DatosCoprec", datosCoprec);
        }
        

        private void AuditarDatosCoprec(DenunciaDto oldDenuncia,DatosCoprec datosCoprecActualizados, NuevoDbContext context,string usuario) {
            var command = new DenunciaCommandService();

            var nroGestionCoprecAnterior = (oldDenuncia.nroGestionCoprec != null) ? oldDenuncia.nroGestionCoprec : "";
            var nroGestionCoprecActual = (datosCoprecActualizados.NroGestionCoprec != null) ? datosCoprecActualizados.NroGestionCoprec : "";

            if (!String.Equals(nroGestionCoprecAnterior, nroGestionCoprecActual)) {               
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
                nroGestionCoprecAnterior, nroGestionCoprecActual, "Se ha modificado el Nro de Gestión Coprec", usuario, oldDenuncia.DenunciaId);
            }

            var honorariosCoprecAnterior = (oldDenuncia.honorariosCoprec != null) ? oldDenuncia.honorariosCoprec : "";
            var honorariosCoprecActual = (datosCoprecActualizados.HonorariosCoprec != null) ? datosCoprecActualizados.HonorariosCoprec : "";

            if (!String.Equals(honorariosCoprecAnterior, honorariosCoprecActual))
            {
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
                honorariosCoprecAnterior, honorariosCoprecActual, "Se ha modificado el campo Honorarios Coprec", usuario, oldDenuncia.DenunciaId);
            }

            var fechaGestionHonorariosAnterior = (oldDenuncia.fechaGestionHonorarios != null) ? oldDenuncia.fechaGestionHonorarios.ToString() : "";
            var fechaGestionHonorariosActual = (datosCoprecActualizados.FechaGestionHonorarios != null) ? datosCoprecActualizados.FechaGestionHonorarios.ToString(): "";

            if (!String.Equals(fechaGestionHonorariosAnterior, fechaGestionHonorariosActual))
            {
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
               fechaGestionHonorariosAnterior, fechaGestionHonorariosActual, "Se ha modificado la Fecha Gestión Honorarios  Coprec", usuario, oldDenuncia.DenunciaId);
            }

            var montoAcordadoAnterior = (oldDenuncia.montoAcordado != null) ? oldDenuncia.montoAcordado : "";
            var montoAcordadoActual = (datosCoprecActualizados.MontoAcordado != null) ? datosCoprecActualizados.MontoAcordado : "";

            if (!String.Equals(montoAcordadoAnterior, montoAcordadoActual))
            {
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
                montoAcordadoAnterior, montoAcordadoActual, "Se ha modificado el Monto Acordado en los Datos Coprec", usuario, oldDenuncia.DenunciaId);
            }

            var arancelAnterior = (oldDenuncia.arancel != null) ? oldDenuncia.arancel : "";
            var arancelActual = (datosCoprecActualizados.Arancel != null) ? datosCoprecActualizados.Arancel : "";

            if (!String.Equals(arancelAnterior, arancelActual))
            {
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
                arancelAnterior, arancelActual, "Se ha modificado el Arancel en los Datos Coprec", usuario, oldDenuncia.DenunciaId);
            }

            var fechaGestionArancelAnterior = (oldDenuncia.fechaGestionArancel != null) ? oldDenuncia.fechaGestionArancel.ToString() : "";
            var fechaGestionArancelActual = (datosCoprecActualizados.FechaGestionArancel != null) ? datosCoprecActualizados.FechaGestionArancel.ToString() : "";

            if (!String.Equals(fechaGestionArancelAnterior, fechaGestionArancelActual))
            {
                command.loguearModificaciones(context, DateTime.Now, "DENUNCIA",
                fechaGestionArancelAnterior, fechaGestionArancelActual, "Se ha modificado la Fecha Gestión Arancel en los datos Coprec", usuario, oldDenuncia.DenunciaId);
            }
        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public async Task<ActionResult> EditarDatosCoprec(DatosCoprecDto datosCoprec)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                DatosCoprecDto datosCoprecDB = context.getDatosCoprec(true)
                                        .Where(d => d.Id == datosCoprec.Id)
                                        .FirstOrDefault();

                
                await context.SaveChangesAsync();
            }
            return PartialView("DatosCoprec", datosCoprec);
        }



        //[HttpPost]
        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //public ActionResult crearGrupo(List<List<string>> lista) {
        //    List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
        //    GrupoDto grupo = new GrupoDto();
        //    DenuncianteDto denuncianteDB = new DenuncianteDto();
        //    DenuncianteDto denunciante = new DenuncianteDto();
        //    string nombre;
        //    string apellido;
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        foreach (var item in lista)
        //        {
        //            nombre = item[1];
        //            apellido = item[2];
        //            if (context.Denunciantes.Any(d => d.nombre == nombre && d.apellido == apellido))
        //            {
        //                denuncianteDB = context.Denunciantes.Where(d => d.nombre == nombre && d.apellido == apellido).FirstOrDefault();
        //                denunciantes.Add(denuncianteDB);
        //            }
        //            else
        //            {                     
        //                denunciante.nombre = item[1].ToUpper();
        //                denunciante.apellido = item[2].ToUpper();

        //                if (!String.IsNullOrWhiteSpace(item[3]))
        //                {
        //                    denunciante.NroDocumento = Convert.ToString(item[3]);
        //                }
        //                context.Add(denunciante);
        //                context.SaveChanges();
        //                denunciante = context.Denunciantes.Find(denunciante.DenuncianteId);
        //                denunciantes.Add(denunciante);
        //            }
        //        }
        //        grupo.grupoDenunciantes = denunciantes;

        //        grupo.DenunciantePrincipal = denunciantes[0];

        //        context.Add(grupo);
        //        context.SaveChanges();
        //    }

        //    return PartialView("ListGrupoDenunciantes",grupo);

        //}
        [HttpPost]
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult crearGrupo(List<List<string>> lista)
        {
            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            GrupoDto grupo = new GrupoDto();
            DenuncianteDto denuncianteDB = new DenuncianteDto();
            DenuncianteDto denunciante = new DenuncianteDto();
            string nombre;
            string apellido;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in lista)
                {
                    nombre = item[1];
                    apellido = item[2];
                    if (context.Denunciantes.Any(d => d.nombre == nombre && d.apellido == apellido))
                    {
                        denuncianteDB = context.Denunciantes.Where(d => d.nombre == nombre && d.apellido == apellido).FirstOrDefault();
                        denunciantes.Add(denuncianteDB);
                    }
                    else
                    {
                        denunciante.nombre = item[1].ToUpper();
                        denunciante.apellido = item[2].ToUpper();

                        if (!String.IsNullOrWhiteSpace(item[3]))
                        {
                            denunciante.NroDocumento = Convert.ToString(item[3]);
                        }
                        context.Add(denunciante);
                        context.SaveChanges();
                        denunciante = context.Denunciantes.Find(denunciante.DenuncianteId);
                        denunciantes.Add(denunciante);
                    }
                }
                //vieja solución
                //grupo.grupoDenunciantes = denunciantes;
                //grupo.DenunciantePrincipal = denunciantes[0];
                grupo.IdDenunciantePrincipal = denunciantes[0].DenuncianteId;
                context.Add(grupo);
                context.SaveChanges();

                //nueva solución
                var idGrupo = grupo.Id;
                foreach (var item in denunciantes)
                {
                    var fechaCreacion = DateTime.Now;
                    context.Database
                    .ExecuteSqlCommand("Insert into GrupoDenunciantesRel values(@GrupoDto_Id,@DenuncianteId,@fechaCreacion)"
                    , new SqlParameter("@GrupoDto_Id", idGrupo)
                    , new SqlParameter("@DenuncianteId", item.DenuncianteId)
                    , new SqlParameter("@fechaCreacion", fechaCreacion));
                }
                
                
            }

            return PartialView("ListGrupoDenunciantes", grupo);

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarEvento(int id) {
            EventoCommandService evc = new EventoCommandService();
            evc.deleteEvento(id);

            return Json("Registro eliminado con éxito");          
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEvento([Bind(Include = "DenunciaId,Fecha,tipoEventoId,CONTESTADO,SOLUCIONADO,REQUERIMIENTOINFORME,observacion,ResIntId")]EventoDto evento) {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                
                    EventoCommandService evc = new EventoCommandService();
                    evento.FECHACREACION = DateTime.Now;
                    evento.Deleted = false;
                    evento.CREATIONPERSON = usuario; 
                    evc.createEvento(evento,usuario);
                
                return PartialView("EventosSP", this.getEventos(evento.DenunciaId));
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarEventoEditado([Bind(Include= "EventoId,DenunciaId,Fecha,tipoEventoId,CONTESTADO,SOLUCIONADO,REQUERIMIENTOINFORME,observacion,ResIntId,Deleted")]EventoDto evento)
        {
            EventoCommandService evc = new EventoCommandService();
            evc.updateEvento(evento);

            return PartialView("EventosSP", this.getEventos(evento.DenunciaId));
        }

        public  List<EventoSP> getEventos(int denunciaId)
        {
            
            List<EventoSP> lista = new List<EventoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista =  context.Database
                                       .SqlQuery<EventoSP>("GetEventsByDenunciaId @denunciaId", new SqlParameter("@denunciaId", denunciaId))
                                       .ToList();
                
            }
            return lista;
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarDelGrupoDenunciantePrincipal(int id, string grupoId,int? denunciaId)
        {

            GrupoSP grupo = new GrupoSP();
            
            var idGrupo = Convert.ToInt32(grupoId);
            using (NuevoDbContext context = new NuevoDbContext())
            {

              context.Database.ExecuteSqlCommand("delete from GrupoDenunciantesRel where GrupoDto_Id = @GrupoDto_Id and DenuncianteDto_DenuncianteId = @DenuncianteId", new SqlParameter("@GrupoDto_Id", idGrupo), new SqlParameter("@DenuncianteId", id));
                
                    List<DenuncianteGrupoSP> grupoDenunciantes = context.Database
                                                           .SqlQuery<DenuncianteGrupoSP>("GetGrupoDenunciantesPorIdGrupo @grupoId", new SqlParameter("@grupoId", grupoId))
                                                           .ToList();
                    grupo.grupoDenunciantes = grupoDenunciantes;
                    grupo.IdDenunciantePrincipal = grupoDenunciantes[0].DenuncianteId;
                    grupo.Id = grupoDenunciantes[0].GrupoDto_Id;

                    if (denunciaId != null) { 
                        context.Database.ExecuteSqlCommand("update tDenuncias set DENUNCIANTE_ID = @denuncianteId where DenunciaId = @denunciaId", new SqlParameter("@denuncianteId", grupo.IdDenunciantePrincipal), new SqlParameter("@denunciaId", denunciaId));
                    }
                
                await context.SaveChangesAsync();

                // auditoria

                var fechaCreacionGrupoAuditoria = DateTime.Now;
                var objetoModificado = "GRUPO";
                var descripcionObjeto = "El Denunciante De la Carátula con Id : " + id + " ha sido Eliminado del grupo Id : " + grupoId;
                var valorAnterior = "";
                var valorActual = "";
                var usuario = System.Web.HttpContext.Current.User.Identity.Name;

                context.Database
                .ExecuteSqlCommand("Insert into tCommonChLogger values(@fechaCreacionGrupo,@objetoModificado,@descripcionObjeto,@valorAnterior,@valorActual,@usuario,@GrupoDto_Id)"
                , new SqlParameter("@fechaCreacionGrupo", fechaCreacionGrupoAuditoria)
                , new SqlParameter("@objetoModificado", objetoModificado)
                , new SqlParameter("@descripcionObjeto", descripcionObjeto)
                , new SqlParameter("@valorAnterior", valorAnterior)
                , new SqlParameter("@valorActual", valorActual)
                , new SqlParameter("@usuario", usuario)
                , new SqlParameter("@GrupoDto_Id", grupoId));

            }
            return PartialView("ListGrupoDenunciantesSP",grupo);
            //return Json("El Registro ha sido eliminado del Grupo");
        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuitarDelGrupo(int id,string grupoId)
        {
            
            
            var idGrupo = Convert.ToInt32(grupoId);
            using (NuevoDbContext context = new NuevoDbContext())
            {
               
                context.Database.ExecuteSqlCommand("delete from GrupoDenunciantesRel where GrupoDto_Id = @GrupoDto_Id and DenuncianteDto_DenuncianteId = @DenuncianteId", new SqlParameter("@GrupoDto_Id", idGrupo), new SqlParameter("@DenuncianteId", id));
                await context.SaveChangesAsync();

                // auditoria

                var fechaCreacionGrupoAuditoria = DateTime.Now;
                var objetoModificado = "GRUPO";
                var descripcionObjeto = "El Denunciante con Id : " + id + " ha sido Eliminado del grupo Id : " + grupoId ;
                var valorAnterior = "";
                var valorActual = "";
                var usuario = System.Web.HttpContext.Current.User.Identity.Name;

                context.Database
                .ExecuteSqlCommand("Insert into tCommonChLogger values(@fechaCreacionGrupo,@objetoModificado,@descripcionObjeto,@valorAnterior,@valorActual,@usuario,@GrupoDto_Id)"
                , new SqlParameter("@fechaCreacionGrupo", fechaCreacionGrupoAuditoria)
                , new SqlParameter("@objetoModificado", objetoModificado)
                , new SqlParameter("@descripcionObjeto", descripcionObjeto)
                , new SqlParameter("@valorAnterior", valorAnterior)
                , new SqlParameter("@valorActual", valorActual)
                , new SqlParameter("@usuario", usuario)
                , new SqlParameter("@GrupoDto_Id", grupoId));

            }
            return Json("El Registro ha sido eliminado del Grupo");
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearDenuncia([Bind(Include = "CREATIONDATE,CREATIONPERSON,DENUNCIANTE_ID,CONCILIACION_ID,ESTUDIO_ID,FCREACION,FSELLOGCIADC,FSELLOCIA,MODALIDADGESTION,OBSERVACIONES,ORGANISMO_ID,RESP_EXT_ID,RESP_INT_ID,SERV_DEN_ID,TIPO_PRO_ID,SUBTIPO_PRO_ID,RECLAMO_ID,EXPEDIENTE_ID,NROCAJAEXTERNO,NROCAJAINTERNO,FNOTIFRESOL,DESCRIPCIONCOSTAS,FACREDPAGOCOSTAS,MONTOCOSTAS,PAGOCOSTAS,RESULTADO_ID,DELETED,ETAPA_ID,MONTOOTROS,MONTOSELLADO,MONTOTASA,NROOBLEA,MOTIVOBORRADO,OBJETORECLAMO,FECHARESULTADO,INACTIVO,PARENTDENUNCIAID,MOTIVOBAJA_ID,TRAMITECRM,tipoEvento,reqInforme,fechaVencimiento,horaVencimiento,grupoId,nroClienteContrato,mediadorId,domicilioMediadorId,reclamoRelacionado,responsableId,responsableEvento,fechaHomologacion,nroGestionCoprec,honorariosCoprec,fechaGestionHonorarios,montoAcordado,arancel,fechaGestionArancel,EXPEDIENTE")] Models.Denuncia.DenunciaModelView denunciaModelView)
        {
            DenunciaCommandService dcs = new DenunciaCommandService();
            EventoCommandService evc = new EventoCommandService();
            DenunciaDto denuncia = new DenunciaDto();
            EventoDto evento = new EventoDto();
            
            denuncia.ETAPA_ID = denunciaModelView.ETAPA_ID;
            denuncia.TIPO_PRO_ID = denunciaModelView.TIPO_PRO_ID;
            denuncia.CREATIONDATE = denunciaModelView.CREATIONDATE;
            denuncia.CREATIONPERSON = denunciaModelView.CREATIONPERSON;
            denuncia.DENUNCIANTE_ID = denunciaModelView.DENUNCIANTE_ID;
            denuncia.TRAMITECRM = denunciaModelView.TRAMITECRM;
            denuncia.FSELLOCIA = DateTime.Parse(denunciaModelView.FSELLOCIA);
            denuncia.FSELLOGCIADC = DateTime.Parse(denunciaModelView.FSELLOGCIADC);
            if (!String.IsNullOrEmpty(denunciaModelView.EXPEDIENTE))
            {
                ExpedienteCommandService expedienteCS = new ExpedienteCommandService();
                var expedienteDto = expedienteCS.crearExpediente(denunciaModelView.EXPEDIENTE);
                denuncia.EXPEDIENTE_ID = expedienteDto.Id;
            }
            
            denuncia.ORGANISMO_ID = denunciaModelView.ORGANISMO_ID;
            denuncia.ESTUDIO_ID = denunciaModelView.ESTUDIO_ID;
            denuncia.MODALIDADGESTION = denunciaModelView.MODALIDADGESTION;
            denuncia.SUBTIPO_PRO_ID = denunciaModelView.SUBTIPO_PRO_ID;
            denuncia.SERV_DEN_ID = denunciaModelView.SERV_DEN_ID;
            if (denunciaModelView.RECLAMO_ID != null) { 
                ReclamoCommandService reclamoCS = new ReclamoCommandService();
                var reclamo = reclamoCS.crearReclamo(denunciaModelView.RECLAMO_ID);
                    denuncia.RECLAMO_ID = reclamo.Id;
            }
            
            denuncia.CONCILIACION_ID = denunciaModelView.CONCILIACION_ID;
            denuncia.OBJETORECLAMO = denunciaModelView.OBJETORECLAMO;
            
            if(denunciaModelView.FECHARESULTADO != null) {
                denuncia.FECHARESULTADO = DateTime.Parse(denunciaModelView.FECHARESULTADO);
            };
            denuncia.RESP_INT_ID = denunciaModelView.responsableId;
            if (!String.IsNullOrWhiteSpace(denunciaModelView.grupoId))
            {
                denuncia.grupoId = Convert.ToInt32(denunciaModelView.grupoId);
            }
            denuncia.nroClienteContrato = denunciaModelView.nroClienteContrato;
            denuncia.mediadorId = denunciaModelView.mediadorId;
            denuncia.domicilioMediadorId = denunciaModelView.domicilioMediadorId;
            denuncia.reclamoRelacionado = denunciaModelView.reclamoRelacionado;
            
            if (denunciaModelView.fechaHomologacion != null) {
                denuncia.fechaHomologacion = DateTime.Parse(denunciaModelView.fechaHomologacion);
            }
            denuncia.nroGestionCoprec = denunciaModelView.nroGestionCoprec;
            denuncia.honorariosCoprec = denunciaModelView.honorariosCoprec;
            if (denunciaModelView.fechaGestionHonorarios!=null) {
                denuncia.fechaGestionHonorarios = DateTime.Parse(denunciaModelView.fechaGestionHonorarios);
            };            
            denuncia.montoAcordado = denunciaModelView.montoAcordado;
            denuncia.arancel = denunciaModelView.arancel;
            if (denunciaModelView.fechaGestionArancel!=null) {
                denuncia.fechaGestionArancel = DateTime.Parse(denunciaModelView.fechaGestionArancel);
            }
            DenunciaDto nuevaDenuncia = dcs.CreateDenuncia(denuncia);
            evento.FECHACREACION = denunciaModelView.CREATIONDATE;
            
            evento.TipoEventoId = denunciaModelView.tipoEvento;
            evento.REQUERIMIENTOINFORME = denunciaModelView.reqInforme;
            
            evento.Fecha = DateTime.Parse(denunciaModelView.fechaVencimiento + ' ' + denunciaModelView.horaVencimiento);
            evento.DenunciaId = nuevaDenuncia.DenunciaId;
            evento.CONTESTADO = 0;
           
            evento.ResIntId = denunciaModelView.responsableEvento;
            evc.createEvento(evento, denunciaModelView.responsableEvento);
            
            DenunciaModelView dm = this.GetDenunciasModel(nuevaDenuncia.DenunciaId);
            ViewBag.saveDenunciaResult = "Denuncia creada exitosamente";

            return PartialView("DatosDenunciaBis", dm);

        }

        //se llevó al FiltroDenunciasController
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public async Task<ActionResult> GetDenuncias()
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
             
                lista = await context.Database
                        .SqlQuery<ListaSabanaSP>("GetListaSabana")
                        .ToListAsync();

            }
            return PartialView("ListSabanaSP", lista);
        }



        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public DenunciaModelView GetDenunciasModel(int id)
        {
            DenunciaQueryService dqs = new DenunciaQueryService();
            DenuncianteQueryService denqs = new DenuncianteQueryService();
            DenunciaDto denuncia = dqs.GetDenunciaById(id);
            DenunciaModelView dm = new DenunciaModelView(denuncia.ORGANISMO_ID, denuncia.SERV_DEN_ID, denuncia.ETAPA_ID, denuncia.TIPO_PRO_ID);
            EventoQueryService eqs = new EventoQueryService();
            var eventosDenuncia = eqs.getEventos(denuncia.DenunciaId);
            var denunciante = denqs.GetDenuncianteById(denuncia.DENUNCIANTE_ID);
            dm.denuncia = denuncia;
            dm.denunciante = denunciante;
            /*dm.laDenunciaEstaCerrada = dqs.laDenunciaEstaCerrada;*///Para Cerrar la Denuncia.
            //dm.datosDenunciante = denunciante.nombre + ' ' + denunciante.apellido + ' ' + denunciante.NroDocumento.ToString() ;
            //dm.dniDenunciante = denunciante.NroDocumento.ToString();
            //dm.apellidoDenunciante = denunciante.apellido;
            //dm.nombreDenunciante = denunciante.nombre;
            dm.RESP_INT_ID = denuncia.RESP_INT_ID;
            dm.estadoSeleccionado = denuncia.ETAPA_ID.ToString();
            dm.organismoSeleccionado = denuncia.ORGANISMO_ID.ToString();
            dm.estudioSeleccionado = denuncia.ESTUDIO_ID.ToString();
            dm.modalidadGestionSeleccionada = denuncia.MODALIDADGESTION.ToString();
            dm.subTipoProcesoSeleccionado = denuncia.SUBTIPO_PRO_ID.ToString();
            dm.servicioSeleccionado = denuncia.SERV_DEN_ID.ToString();
            dm.motivoReclamoSeleccionado = (denuncia.RECLAMO_ID > 0) ? dqs.getMotivoDeReclamoPorReclamo(denuncia.RECLAMO_ID).ToString() : "";
            dm.EXPEDIENTE = (denuncia.EXPEDIENTE_ID > 0) ? dqs.getNumeroExpediente(denuncia.EXPEDIENTE_ID) : "";
            //dm.motivoReclamoSeleccionado = denuncia.RECLAMO_ID.ToString();
            dm.tipoProcesoSeleccionado = denuncia.TIPO_PRO_ID.ToString();
            dm.subEstadoSeleccionado = denuncia.CONCILIACION_ID.ToString();
            dm.mediadorSeleccionado = denuncia.mediadorId.ToString();
            dm.domicilioMediadorSeleccionado = denuncia.domicilioMediadorId.ToString();
            dm.responsableSeleccionado = denuncia.RESP_INT_ID.ToString();
            dm.eventos = eventosDenuncia;
            //dm.FECHARESULTADO = denuncia.FECHARESULTADO.Value.ToShortDateString();.ToString("dd MMMM yyyy hh:mm:ss");
            dm.FECHARESULTADO = (denuncia.FECHARESULTADO!= null)?denuncia.FECHARESULTADO.Value.ToString("yyyy-MM-dd"):null;


            dm.denuncianteIndividual = dm.denuncia.grupoId == null;
            dm.fechaHomologacion = (denuncia.fechaHomologacion!=null)?denuncia.fechaHomologacion.Value.ToString("yyyy-MM-dd"):null;
            dm.nroGestionCoprec = denuncia.nroGestionCoprec;
            dm.honorariosCoprec = denuncia.honorariosCoprec;
            dm.fechaGestionHonorarios = (denuncia.fechaGestionHonorarios != null)?denuncia.fechaGestionHonorarios.Value.ToString("yyyy-MM-dd"):null;
            dm.montoAcordado = denuncia.montoAcordado;
            dm.arancel = denuncia.arancel;
            dm.fechaGestionArancel = (denuncia.fechaGestionArancel!= null)?denuncia.fechaGestionArancel.Value.ToString("yyyy-MM-dd"):null;
            dm.Inactivo = denuncia.INACTIVO;
            dm.DELETED = denuncia.DELETED; 
            return dm;
         
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult GetDenunciaById(int id)
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            DenunciaModelView dm = this.GetDenunciasModel(id);
            dm.ecmUrl = (string)Session["urlECM"];
            DenunciasContainerModel model = new DenunciasContainerModel(dm);
            Session["DenunciaActual"] = id;

            if (dm.DELETED == true) {
                return View("DenunciaEdiciónEliminada", model);
            }
            return View("DenunciaEdiciónBis", model);
            
        }

        [ActionAuthorize(Roles = "ESTUDIO")]
        public ActionResult GetDenunciaByIdExternos(int id)
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            DenunciaModelView dm = this.GetDenunciasModel(id);
            DenunciasContainerModel model = new DenunciasContainerModel(dm);
            Session["DenunciaActual"] = id;

            if ((int?)Session["estudioExternoId"] != null)
            {
                if (dm.denuncia.ESTUDIO_ID != (int)Session["estudioExternoId"])
                {
                    return View("DenunciaNoAutorizada");
                }
            }
            else {
                return RedirectToAction("SesionCulminada", "Home",null);
            }
            
            return View("DenunciaEdiciónExternos", model);
            
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult GetDatosDenunciaById(int id)
        {

            DenunciaModelView dm = this.GetDenunciasModel(id);
            Session["DenunciaActual"] = id;
            ViewBag.saveDenunciaResult = "La Denuncia ha sido formalizada";
            return PartialView("DatosDenunciaBis", dm);
            //return PartialView("DatosDenunciaFormalizada",dm);
            
        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult GetNuevaDenuncia(string tipoProceso)
        {
            
            //DenunciaModelView dm = new DenunciaModelView(null,null,null,Convert.ToInt32(tipoProceso));
            DenunciaModelView dm = new DenunciaModelView(Convert.ToInt32(tipoProceso));
            dm.tipoProcesoSeleccionado = tipoProceso;
            dm.denuncianteIndividual = true;
            dm.RESP_INT_ID = this.getResponsableIdByUserName();

            DenunciasContainerModel model = new DenunciasContainerModel(dm);
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            model.denunciasModelView.denuncia.CREATIONPERSON = (string)Session["usuarioLogueado"];
            return View("DenunciaEdiciónBis",model);
            
        }

        public int? getResponsableIdByUserName() {
            //var usuario = (string)Session["usuarioLogueado"];
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext()) {
               var idResponsable = context.Responsables.Where(x=>String.Equals(x.UmeId.Trim(), usuario.Trim())).FirstOrDefault().Id;
                return idResponsable;
            }
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDenuncia([Bind(Include = "DenunciaId,CREATIONDATE,CREATIONPERSON,DENUNCIANTE_ID,CONCILIACION_ID,ESTUDIO_ID,FCREACION,FSELLOGCIADC,FSELLOCIA,MODALIDADGESTION,OBSERVACIONES,ORGANISMO_ID,RESP_EXT_ID,RESP_INT_ID,SERV_DEN_ID,TIPO_PRO_ID,SUBTIPO_PRO_ID,RECLAMO_ID,EXPEDIENTE_ID,NROCAJAEXTERNO,NROCAJAINTERNO,FNOTIFRESOL,DESCRIPCIONCOSTAS,FACREDPAGOCOSTAS,MONTOCOSTAS,PAGOCOSTAS,RESULTADO_ID,DELETED,ETAPA_ID,MONTOOTROS,MONTOSELLADO,MONTOTASA,NROOBLEA,MOTIVOBORRADO,OBJETORECLAMO,FECHARESULTADO,INACTIVO,PARENTDENUNCIAID,MOTIVOBAJA_ID,TRAMITECRM,grupoId,nroClienteContrato,mediadorId,domicilioMediadorId,reclamoRelacionado,idDatosCoprec,responsableId,fechaHomologacion,nroGestionCoprec,honorariosCoprec,fechaGestionHonorarios,montoAcordado,arancel,fechaGestionArancel,EXPEDIENTE,MotivoReclamoId")] DenunciaModelView denunciaModelView)/*Models.Denuncia.*/
        {
            
            //if (ModelState.IsValid)
            //{
            DenunciaCommandService dcs = new DenunciaCommandService();
            DenunciaDto denuncia = new DenunciaDto();
            denuncia.DenunciaId = denunciaModelView.DenunciaId;
            denuncia.ETAPA_ID = denunciaModelView.ETAPA_ID;
            denuncia.TIPO_PRO_ID = denunciaModelView.TIPO_PRO_ID;
            denuncia.CREATIONDATE = denunciaModelView.CREATIONDATE;
            denuncia.CREATIONPERSON = denunciaModelView.CREATIONPERSON;
            denuncia.OBJETORECLAMO = denunciaModelView.OBJETORECLAMO;
            denuncia.DENUNCIANTE_ID = denunciaModelView.DENUNCIANTE_ID;
            denuncia.TRAMITECRM = denunciaModelView.TRAMITECRM;
            denuncia.FSELLOCIA = DateTime.Parse(denunciaModelView.FSELLOCIA);
            denuncia.FSELLOGCIADC = DateTime.Parse(denunciaModelView.FSELLOGCIADC);
            //denuncia.EXPEDIENTE_ID = denunciaModelView.EXPEDIENTE_ID;
            var expediente = denunciaModelView.EXPEDIENTE;
            denuncia.ORGANISMO_ID = denunciaModelView.ORGANISMO_ID;
            denuncia.ESTUDIO_ID = denunciaModelView.ESTUDIO_ID;
            denuncia.MODALIDADGESTION = denunciaModelView.MODALIDADGESTION;
            denuncia.SUBTIPO_PRO_ID = denunciaModelView.SUBTIPO_PRO_ID;
            denuncia.SERV_DEN_ID = denunciaModelView.SERV_DEN_ID;
            //denuncia.RECLAMO_ID = denunciaModelView.RECLAMO_ID;
            var motivoDeReclamo = denunciaModelView.MotivoReclamoId;
            denuncia.CONCILIACION_ID = denunciaModelView.CONCILIACION_ID;
            //denuncia.FECHARESULTADO = denunciaModelView.FECHARESULTADO;
            denuncia.FECHARESULTADO = null;
            if(!String.IsNullOrEmpty(denunciaModelView.FECHARESULTADO)){
                denuncia.FECHARESULTADO = DateTime.Parse(denunciaModelView.FECHARESULTADO); };
            denuncia.RESP_INT_ID = denunciaModelView.responsableId;
            if (!String.IsNullOrWhiteSpace(denunciaModelView.grupoId))
            {
                denuncia.grupoId = Convert.ToInt32(denunciaModelView.grupoId);
            }
            denuncia.nroClienteContrato = denunciaModelView.nroClienteContrato;
            denuncia.mediadorId = denunciaModelView.mediadorId;
            denuncia.domicilioMediadorId = denunciaModelView.domicilioMediadorId;
            denuncia.reclamoRelacionado = denunciaModelView.reclamoRelacionado;
            //denuncia.idDatosCoprec = denunciaModelView.idDatosCoprec;
            denuncia.fechaHomologacion = null;
            if (!String.IsNullOrEmpty(denunciaModelView.fechaHomologacion))
            {
                denuncia.fechaHomologacion = DateTime.Parse(denunciaModelView.fechaHomologacion);
            };
            denuncia.nroGestionCoprec = denunciaModelView.nroGestionCoprec;
            denuncia.honorariosCoprec = denunciaModelView.honorariosCoprec;
            denuncia.fechaGestionHonorarios = null;
            if (!String.IsNullOrEmpty(denunciaModelView.fechaGestionHonorarios))
            {
                denuncia.fechaGestionHonorarios = DateTime.Parse(denunciaModelView.fechaGestionHonorarios);
            };
            denuncia.montoAcordado = denunciaModelView.montoAcordado;
            denuncia.arancel = denunciaModelView.arancel;
            denuncia.fechaGestionArancel = null;
            if (!String.IsNullOrEmpty(denunciaModelView.fechaGestionArancel))
            {
                denuncia.fechaGestionArancel = DateTime.Parse(denunciaModelView.fechaGestionArancel);
            };
            DenunciaDto nuevaDenuncia = dcs.updateDenuncia(denuncia,expediente,motivoDeReclamo);
            DenunciaModelView dm = this.GetDenunciasModel(nuevaDenuncia.DenunciaId);
            ViewBag.saveDenunciaResult = "La Denuncia ha sido Actualizada";

            return PartialView("DatosDenunciaBis", dm);
            //return Json(dm);

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public ActionResult EditarDenunciaCerrada(int? DenunciaId,int? CONCILIACION_ID) {
            DenunciaCommandService dcs = new DenunciaCommandService();
            var helper = dcs.updateEstadoConciliacion(DenunciaId,CONCILIACION_ID);
            return Json(helper);
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> GetDenunciante(int? id)
        {
            var denunciante = new DenuncianteDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {                
                 denunciante = context.Denunciantes.Where(e => e.DenuncianteId == id).FirstOrDefault();                
            }
            return PartialView("Denunciante",denunciante);
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearDenuncianteDenuncia(DenuncianteDto denunciante)/*(string nombre, string apellido, int? dni)*/
        {           
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (!String.IsNullOrEmpty(denunciante.nombre) && !String.IsNullOrEmpty(denunciante.apellido) && esValidoDni(denunciante.NroDocumento))
                {

                    if (String.IsNullOrEmpty(denunciante.NroDocumento))
                    {
                        if (context.Denunciantes.Any(e => String.Equals(e.apellido.Trim(), denunciante.apellido.Trim()) && (String.Equals(e.nombre.Trim(), denunciante.nombre.Trim()))))
                        {
                            var unDenunciante = context.Denunciantes.Where(e => String.Equals(e.apellido.Trim(), denunciante.apellido.Trim()) && (String.Equals(e.nombre.Trim(), denunciante.nombre.Trim()))).FirstOrDefault();

                            return RedirectToAction("GetDenuncianteDenunciaById", new { id = unDenunciante.DenuncianteId });
                        }
                        else {
                            denunciante.Deleted = false;
                            DenuncianteCommandService dcs = new DenuncianteCommandService();
                            var creado = dcs.crearDenunciante(denunciante);
                           
                            return RedirectToAction("GetDenuncianteDenunciaById", new { id = creado.DenuncianteId });
                        }

                    }
                    else
                        if ((!String.IsNullOrEmpty(denunciante.NroDocumento) && (context.Denunciantes.Any(e => String.Equals(e.NroDocumento.Trim(), denunciante.NroDocumento.Trim())))))
                        {
                            return Json("<div class='alert alert-danger text-center'>Ya existe un Denunciante con el mismo Nro. de Documento</div>");
                        }
                        else {
                            denunciante.Deleted = false;
                            DenuncianteCommandService dcs = new DenuncianteCommandService();
                            var dCreado = dcs.crearDenunciante(denunciante);

                            return RedirectToAction("GetDenuncianteDenunciaById", new { id = dCreado.DenuncianteId });
                        }
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>Los datos ingresados son inválidos</div>");
                }
            }
            
        }

        private bool esValidoDni(string dni) {
            bool success = true;
            if (!String.IsNullOrEmpty(dni))
            {
                int number;
                success = int.TryParse(dni, out number);
                //estaOk = (success && (dni.Length < 9));
            }
            return success;
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuscarDenuncianteDenuncia(string nombre, string apellido, string dni)
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                var nDni = dni;
                if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido)&& !String.IsNullOrEmpty(dni))
                {
                    
                    
                    lista = await context.Denunciantes.Where(m => String.Equals(m.NroDocumento,nDni)
                                                               && m.apellido.Contains(apellido)
                                                               && m.nombre.Contains(nombre)).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido))
                {
                    lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)
                                                               && m.nombre.Contains(nombre)).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(dni))
                {
                    
                    lista = await context.Denunciantes.Where(m => m.nombre.Contains(nombre)
                                                               && String.Equals(m.NroDocumento.Trim(),nDni.Trim())).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(dni))
                {
                    
                    lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)
                                                               && String.Equals(m.NroDocumento.Trim(), nDni.Trim())).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Denunciantes.Where(m => m.nombre.Contains(nombre)).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(apellido))
                {
                    lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)).ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(dni))
                {
                    
                    lista = await context.Denunciantes.Where(m => String.Equals(m.NroDocumento.Trim(),nDni.Trim())).ToListAsync();
                }

            }
            return PartialView("Denunciantes", lista);
        }

        //[HttpPost]
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public async Task<ActionResult> GetDenuncianteDenunciaById(int? id)
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Denunciantes.Where(d=>d.DenuncianteId == id).ToListAsync();
            }
            return PartialView("Denunciantes", lista);
        }



        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //[HttpPost]

        //public Boolean ExisteElDni(string dni, int id)
        //{
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {                
        //       var existe = context.Denunciantes.Any(d => String.Equals(d.NroDocumento,dni) && d.DenuncianteId != id);
        //       return existe;                     
        //    }

        //}

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]

        public ActionResult ExisteElDni(string dni, int id)
        {
            if (!String.IsNullOrEmpty(dni))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    var existe = context.Denunciantes.Any(d => String.Equals(d.NroDocumento, dni) && d.DenuncianteId != id);
                    if (existe)
                    {
                        return Json("<div class='alert alert-danger text-center'>Ya existe un denunciante con el mismo Nro. de Documento</div>");
                    }
                }
            }
            return JavaScript("guardarDenuncianteEdicion()");
            //return Json("<div class='alert alert-success text-center'>dni OK</div>");

        }

        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> guardarDenuncianteEdicion(int id, string nombre, string apellido, string dni)
        //{
        //    DenuncianteDto denunciante = new DenuncianteDto();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {

        //            denunciante = context.getDenunciantes(true)
        //                                .Where(t => t.DenuncianteId == id)
        //                                .FirstOrDefault();
        //            denunciante.nombre = nombre.ToUpper();
        //            denunciante.apellido = apellido.ToUpper();
        //            denunciante.NroDocumento = dni;

        //            context.SaveChanges();
        //            var idEditado = denunciante.DenuncianteId;
        //            denunciante = await context.Denunciantes.Where(e => e.DenuncianteId == idEditado).FirstOrDefaultAsync();

        //    }

        //    return Json(denunciante);

        //}

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> guardarDenuncianteEdicion(int id, string nombre, string apellido, string dni)
        {
            DenuncianteDto denunciante = new DenuncianteDto();
            denunciante.DenuncianteId = id;
            denunciante.nombre = nombre.ToUpper();
            denunciante.apellido = apellido.ToUpper();
            denunciante.NroDocumento = dni;
            DenuncianteCommandService dcs = new DenuncianteCommandService();
            var denuncianteEditado = dcs.getDenuncianteEditado(denunciante);
            
            return Json(denuncianteEditado);

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> guardarDenuncianteGrupoEdicion(int id, string nombre, string apellido, string dni)
        {
            DenuncianteDto denunciante = new DenuncianteDto();
            denunciante.DenuncianteId = id;
            denunciante.nombre = nombre.ToUpper();
            denunciante.apellido = apellido.ToUpper();
            denunciante.NroDocumento = dni;
            DenuncianteCommandService dcs = new DenuncianteCommandService();
            var denuncianteEditado = dcs.getDenuncianteEditado(denunciante);

            return Json(denuncianteEditado);
        }


        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> guardarDenuncianteGrupoEdicion(int id, string nombre, string apellido, string dni)
        //{
        //    string nrodDni = Convert.ToString(dni);
        //    DenuncianteDto denunciante = new DenuncianteDto();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {

        //            denunciante = context.getDenunciantes(true)

        //                                                          .Where(t => t.DenuncianteId == id)
        //                                                          .FirstOrDefault();
        //            denunciante.nombre = nombre.ToUpper();
        //            denunciante.apellido = apellido.ToUpper();
        //            denunciante.NroDocumento = nrodDni;

        //            context.SaveChanges();
        //            var idEditado = denunciante.DenuncianteId;
        //            denunciante = await context.Denunciantes.Where(e => e.DenuncianteId == idEditado).FirstOrDefaultAsync();

        //    }
        //    return Json(denunciante);

        //}


        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //public void ExportListToCSV()
        //{

        //    StringWriter sw = new StringWriter();


        //    sw.WriteLine("DenunciaId,\"Nro Expediente\",\"Fecha\",\"Organismo\",\"Servicio\",\"Estudio\",\"Denunciante\",\"Estado Actual\",\"Tipo Proceso\"");
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment;filename=Exported_Users.csv");
        //    Response.HeaderEncoding = System.Text.Encoding.Default;
        //    Response.ContentType = "text/csv;charset=utf-8";

        //    List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {

        //        lista = context.Database
        //                .SqlQuery<ListaSabanaSP>("GetListaSabana")
        //                .ToList();

        //    }
        //    foreach (var line in lista)
        //    {
        //        //sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
        //        sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
        //                                   line.DenunciaId,
        //                                   line.Expediente,
        //                                   line.Fecha_Creacion,
        //                                   line.Organismo,
        //                                   line.Servicio,
        //                                   line.Estudio,
        //                                   line.Apellido + ' ' + line.Nombre ,
        //                                   line.Estado_Actual,
        //                                   line.Tipo_Proceso
        //                                   ), System.Text.Encoding.UTF8);
        //    }

        //    Response.Write(sw.ToString());

        //    Response.End();

        //}


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public void ExportListToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();

            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {

                lista = context.Database
                        .SqlQuery<ListaSabanaSP>("GetListaSabana")
                        .ToList();

            }
            grid.DataSource = lista;

            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Exported.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public async Task<ActionResult> GetHistory(int id) {
            using (NuevoDbContext context = new NuevoDbContext()) {
             var historial = await context.Database
                             .SqlQuery<CambiosDenuncia>("CambiosPorIdDenuncia @denunciaId",
                             new SqlParameter("@denunciaId", id))
                             .ToListAsync() ;
                return PartialView("HistorialDenuncia",historial);
            }

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult formalizar([Bind(Include = "DenunciaId,DENUNCIANTE_ID,CONCILIACION_ID,ESTUDIO_ID,FCREACION,FSELLOGCIADC,FSELLOCIA,MODALIDADGESTION,OBSERVACIONES,ORGANISMO_ID,RESP_EXT_ID,RESP_INT_ID,SERV_DEN_ID,TIPO_PRO_ID,SUBTIPO_PRO_ID,RECLAMO_ID,EXPEDIENTE_ID,NROCAJAEXTERNO,NROCAJAINTERNO,FNOTIFRESOL,DESCRIPCIONCOSTAS,FACREDPAGOCOSTAS,MONTOCOSTAS,PAGOCOSTAS,RESULTADO_ID,DELETED,ETAPA_ID,MONTOOTROS,MONTOSELLADO,MONTOTASA,NROOBLEA,MOTIVOBORRADO,OBJETORECLAMO,FECHARESULTADO,INACTIVO,PARENTDENUNCIAID,MOTIVOBAJA_ID,TRAMITECRM,grupoId,nroClienteContrato,mediadorId,domicilioMediadorId,reclamoRelacionado,responsableId,fechaHomologacion,nroGestionCoprec,honorariosCoprec,fechaGestionHonorarios,montoAcordado,arancel,fechaGestionArancel")] Models.Denuncia.DenunciaDto preventiva) {

            DenunciaCommandService service = new DenunciaCommandService();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            var idDenuncia = service.formalizarDenuncia(preventiva,usuario);
            return RedirectToAction("GetDatosDenunciaById",new {@id = idDenuncia });
            
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult GetConfimarEliminarDenuncia() {
            return PartialView("EliminarDenuncia",new EliminarDenunciaModelView());
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarDenuncia(int id,int? motivoBajaId)
        {
            DenunciaCommandService dcs = new DenunciaCommandService();
            dcs.EliminarDenuncia(id, motivoBajaId);
            return Json("Denuncia Eliminada correctamente",JsonRequestBehavior.AllowGet);
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult GetMotivoDeBorrado(int? id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var motivoId = context.Denuncias.Where(x => x.DenunciaId == id).FirstOrDefault().MOTIVOBAJA_ID;
                var motivoDeBaja = context.MotivosDeBaja.Where(x => x.Id == motivoId).FirstOrDefault().Nombre;
                return Json("Motivo de Borrado: " + motivoDeBaja, JsonRequestBehavior.AllowGet);
            }
                
        }
    }
}