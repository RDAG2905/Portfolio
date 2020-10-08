using Greco2.Model;
using Greco2.Models.DenChLogger;
using Greco2.Models.Denuncia;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.Validators
{
    public class DenunciasChangeValidator
    {
        public DenunciasChangeValidator(DenunciaDto oldDenuncia, DenunciaDto newDenuncia,string user) {
            usuario = user;
            objetoId = oldDenuncia.DenunciaId;
            etapaIdAnterior = oldDenuncia.ETAPA_ID;
            tipoProcesoIdAnterior = oldDenuncia.TIPO_PRO_ID;
            denuncianteIdAnterior = oldDenuncia.DENUNCIANTE_ID;
            tramiteCRMAnterior = oldDenuncia.TRAMITECRM;
            objetoReclamoAnterior = oldDenuncia.OBJETORECLAMO;
            fechaNotificacionAnterior = oldDenuncia.FSELLOCIA;
            fechaNotificacionGciAnterior = oldDenuncia.FSELLOGCIADC;
            expedienteIdAnterior = oldDenuncia.EXPEDIENTE_ID;
            responsableIdAnterior = oldDenuncia.RESP_INT_ID;
            organismoIdAnterior = oldDenuncia.ORGANISMO_ID;
            estudioIdAnterior = oldDenuncia.ESTUDIO_ID;
            modalidadGestionIdAnterior = oldDenuncia.MODALIDADGESTION;
            subTipoProcesoAnteriorId = oldDenuncia.SUBTIPO_PRO_ID;
            servicioIdAnterior = oldDenuncia.SERV_DEN_ID;
            reclamoIdAnterior = oldDenuncia.RECLAMO_ID;   // retocar al modificar el modelo
            conciliacionIdAnterior = oldDenuncia.CONCILIACION_ID;
            fechaResultadoAnterior = oldDenuncia.FECHARESULTADO.ToString();
            grupoIdAnterior = oldDenuncia.grupoId;
            nroClienteContratoAnterior = oldDenuncia.nroClienteContrato;
            mediadorIdAnterior = oldDenuncia.mediadorId;
            domicilioMediadorIdAnterior = oldDenuncia.domicilioMediadorId;
            reclamoRelacionadoAnterior = oldDenuncia.reclamoRelacionado;
            //idDatosCoprecAnterior = oldDenuncia.idDatosCoprec;
            fechaHomologacionAnterior = oldDenuncia.fechaHomologacion.ToString();
            nroGestionAnterior = oldDenuncia.nroGestionCoprec;
            honorarioAnterior = oldDenuncia.honorariosCoprec;
            montoAcordadoAnterior = oldDenuncia.montoAcordado;
            fechaGestionHonorariosAnterior = oldDenuncia.fechaGestionHonorarios.ToString();
            arancelAnterior = oldDenuncia.arancel;
            fechaGestionArancelAnterior = oldDenuncia.fechaGestionArancel.ToString();

            etapaIdActual = newDenuncia.ETAPA_ID;
            tipoProcesoIdActual = newDenuncia.TIPO_PRO_ID;
            denuncianteIdActual = newDenuncia.DENUNCIANTE_ID;
            tramiteCRMActual = newDenuncia.TRAMITECRM;
            objetoReclamoActual = newDenuncia.OBJETORECLAMO;
            fechaNotificacionActual = newDenuncia.FSELLOCIA;
            fechaNotificacionGciActual = newDenuncia.FSELLOGCIADC;
            expedienteIdActual = newDenuncia.EXPEDIENTE_ID;
            responsableIdActual = newDenuncia.RESP_INT_ID;
            organismoIdActual = newDenuncia.ORGANISMO_ID;
            estudioIdActual = newDenuncia.ESTUDIO_ID;
            modalidadGestionIdActual = newDenuncia.MODALIDADGESTION;
            subTipoProcesoActualId = newDenuncia.SUBTIPO_PRO_ID;
            servicioIdActual = newDenuncia.SERV_DEN_ID;
            reclamoIdActual = newDenuncia.RECLAMO_ID; // retocar al modificar el modelo
            conciliacionIdActual = newDenuncia.CONCILIACION_ID;
            fechaResultadoActual = newDenuncia.FECHARESULTADO.ToString();
            grupoIdActual = newDenuncia.grupoId;
            nroClienteContratoActual = newDenuncia.nroClienteContrato;
            mediadorIdActual = newDenuncia.mediadorId;
            domicilioMediadorIdActual = newDenuncia.domicilioMediadorId;
            reclamoRelacionadoActual = newDenuncia.reclamoRelacionado;
            //idDatosCoprecActual = newDenuncia.idDatosCoprec;
            fechaHomologacionActual = newDenuncia.fechaHomologacion.ToString();
            nroGestionActual = newDenuncia.nroGestionCoprec;
            honorarioActual = newDenuncia.honorariosCoprec;
            montoAcordadoActual = newDenuncia.montoAcordado;
            fechaGestionHonorariosActual = newDenuncia.fechaGestionHonorarios.ToString();
            arancelActual = newDenuncia.arancel;
            fechaGestionArancelActual = newDenuncia.fechaGestionArancel.ToString();
        }
        string usuario;
        int objetoId;
        string objetoModificado = "DENUNCIA";
        // valores Anteriores
        int? etapaIdAnterior;
        int? tipoProcesoIdAnterior;
        int? denuncianteIdAnterior;
        string tramiteCRMAnterior;
        string objetoReclamoAnterior;
        DateTime fechaNotificacionAnterior;
        DateTime fechaNotificacionGciAnterior;
        int? expedienteIdAnterior;
        int? responsableIdAnterior;
        int? organismoIdAnterior;
        int? estudioIdAnterior;
        int? modalidadGestionIdAnterior;
        int? subTipoProcesoAnteriorId;
        int? servicioIdAnterior;
        int? reclamoIdAnterior;
        int? conciliacionIdAnterior;
        string fechaResultadoAnterior;
        int? grupoIdAnterior;
        string nroClienteContratoAnterior;
        int? mediadorIdAnterior;
        int? domicilioMediadorIdAnterior;
        string reclamoRelacionadoAnterior;
        //int? idDatosCoprecAnterior;
        string fechaHomologacionAnterior;
        string nroGestionAnterior;
        string honorarioAnterior;
        string montoAcordadoAnterior;
        string fechaGestionHonorariosAnterior;
        string arancelAnterior;
        string fechaGestionArancelAnterior;
        // valores Actuales
        int? etapaIdActual;
        int? tipoProcesoIdActual;
        int? denuncianteIdActual;
        string tramiteCRMActual;
        string objetoReclamoActual;
        DateTime fechaNotificacionActual;
        DateTime fechaNotificacionGciActual;
        int? expedienteIdActual;
        int? organismoIdActual;
        int? responsableIdActual;
        int? estudioIdActual;
        int? modalidadGestionIdActual;
        int? subTipoProcesoActualId;
        int? servicioIdActual;
        int? reclamoIdActual;
        int? conciliacionIdActual;
        string fechaResultadoActual;
        int? grupoIdActual;
        string nroClienteContratoActual;
        int? mediadorIdActual;
        int? domicilioMediadorIdActual;
        string reclamoRelacionadoActual;
        //int? idDatosCoprecActual;
        string fechaHomologacionActual;
        string nroGestionActual;
        string honorarioActual;
        string montoAcordadoActual;
        string fechaGestionHonorariosActual;
        string arancelActual;
        string fechaGestionArancelActual;


        public void registrarCambios(NuevoDbContext context) {
            if(sonDistintos(etapaIdAnterior,etapaIdActual)) {
                // Faltan varios campos de la denuncia.
                var etapaProcesalAnterior = (etapaIdAnterior > 0)?context.Estados.Where(x => x.Id == etapaIdAnterior).FirstOrDefault().TipoEstado:"" ;
                var etapaProcesalActual = (etapaIdActual > 0)?context.Estados.Where(x => x.Id == etapaIdActual).FirstOrDefault().TipoEstado:"";
                if (sonDistintos(etapaProcesalAnterior, etapaProcesalActual)) { 
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                    etapaProcesalAnterior, etapaProcesalActual, "Se ha modificado la Etapa Procesal",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(tipoProcesoIdAnterior,tipoProcesoIdActual))
            {
                var tipoProcesoAnterior = (tipoProcesoIdAnterior > 0)? context.TiposDeProceso.Where(x => x.Id == tipoProcesoIdAnterior).FirstOrDefault().Nombre:"";
                var tipoProcesoActual = (tipoProcesoIdActual > 0)? context.TiposDeProceso.Where(x => x.Id == tipoProcesoIdActual).FirstOrDefault().Nombre:"";
                if (sonDistintos(tipoProcesoAnterior,tipoProcesoActual)) { 
                loguearModificaciones(context, DateTime.Now,objetoModificado, 
                    tipoProcesoAnterior, tipoProcesoActual,"Se ha modificado el Tipo de Proceso",
                    this.usuario,this.objetoId);
                }
            }
            if (sonDistintos(denuncianteIdAnterior, denuncianteIdActual)) {
                var denuncianteAnterior = context.Denunciantes.Where(x => x.DenuncianteId == denuncianteIdAnterior).FirstOrDefault();
                var datosDenuncianteAnterior = (denuncianteAnterior != null)? denuncianteAnterior.apellido + " " + denuncianteAnterior.nombre:"";
                var denuncianteActual = context.Denunciantes.Where(x => x.DenuncianteId == denuncianteIdActual).FirstOrDefault();
                var datosDenuncianteActual = (denuncianteActual != null)?denuncianteActual.apellido + " " + denuncianteActual.nombre:"";

                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    datosDenuncianteAnterior, datosDenuncianteActual, "Se ha modificado al Denunciante",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(tramiteCRMAnterior,tramiteCRMActual))
            {
                var anterior = (tramiteCRMAnterior != null) ? tramiteCRMAnterior.ToString() : "";
                var actual = (tramiteCRMActual != null) ? tramiteCRMActual.ToString() : "";
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    anterior, actual, "Se ha modificado el Tramite CRM",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(objetoReclamoAnterior, objetoReclamoActual))
            {             
                var anterior = (objetoReclamoAnterior != null) ? objetoReclamoAnterior.ToString() : "";
                var actual = (objetoReclamoActual != null) ? objetoReclamoActual.ToString() : "";
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    anterior,actual, "Se ha modificado el Nro. de Linea",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(fechaNotificacionAnterior,fechaNotificacionActual))
            {
                var fechaAnterior = (fechaNotificacionAnterior != null) ? fechaNotificacionAnterior.ToString():"";
                var fechaActual = (fechaNotificacionActual != null) ? fechaNotificacionActual.ToString() : "";
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    fechaAnterior, fechaActual, "Se ha modificado la Fecha de Notificación",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(fechaNotificacionGciAnterior, fechaNotificacionGciActual))
            {
                var fechaAnterior = (fechaNotificacionGciAnterior != null) ? fechaNotificacionGciAnterior.ToString() : "";
                var fechaActual = (fechaNotificacionGciActual != null) ? fechaNotificacionGciActual.ToString() : "";
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    fechaAnterior, fechaActual, "Se ha modificado la Fecha de Notificación Gcia",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(expedienteIdAnterior, expedienteIdActual))
            {
                var expedienteAnterior = (expedienteIdAnterior != null)?context.Expedientes.Where(exp => exp.Id == expedienteIdAnterior).FirstOrDefault().Numero:"";
                var expedienteActual = (expedienteIdActual != null)?context.Expedientes.Where(exp => exp.Id == expedienteIdActual).FirstOrDefault().Numero:"";
                //var expedienteAnterior = (expedienteIdAnterior == null)?"": expedienteIdAnterior.ToString();
                //var expedienteActual = (expedienteIdActual == null) ? "" : expedienteIdActual.ToString();
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    expedienteAnterior, expedienteActual, "Se ha modificado el Expediente",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(responsableIdAnterior,responsableIdActual))
            {
                var responsableAnterior = (responsableIdAnterior > 0)? context.Database.SqlQuery<string>("select nombre from tResponsables where Id = @responsableAnteriorId",
                    new SqlParameter("@responsableAnteriorId",responsableIdAnterior)).FirstOrDefault(): "";
                var responsableActual = (responsableIdActual > 0)? context.Database.SqlQuery<string>("select nombre from tResponsables where Id = @responsableActualId",
                    new SqlParameter("@responsableActualId", responsableIdActual)).FirstOrDefault():"";

                if (sonDistintos(responsableAnterior,responsableActual)) { 
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                    responsableAnterior, responsableActual, "Se ha modificado al Responsable",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(organismoIdAnterior,organismoIdActual))
            {
                var organismoAnterior = (organismoIdAnterior > 0)?context.Organismos.Where(x => x.Id == organismoIdAnterior).FirstOrDefault().Nombre:"";
                var organismoActual = (organismoIdActual> 0)?context.Organismos.Where(x => x.Id == organismoIdActual).FirstOrDefault().Nombre:"";
                if (sonDistintos(organismoAnterior, organismoActual)) { 
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    organismoAnterior, organismoActual, "Se ha modificado el Organismo",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(estudioIdAnterior, estudioIdActual))
            {
                var estudioAnterior = "";
                if (estudioIdAnterior.HasValue) {
                    var estudio = context.Estudios.Where(x => x.Id == estudioIdAnterior.Value).FirstOrDefault();
                    estudioAnterior = (estudio != null )?estudio.Nombre:"";
                }
                //var estudioAnterior = (estudioIdAnterior != null && estudioIdAnterior > 0)?context.Estudios.Where(x => x.Id == estudioIdAnterior).FirstOrDefault().Nombre:"";

                var estudioActual = (estudioIdActual != null && estudioIdActual > 0)? context.Estudios
                                                                      .Where(x => x.Id == estudioIdActual).FirstOrDefault().Nombre:"";
                if(sonDistintos(estudioAnterior,estudioActual)) { 
                loguearModificaciones(context, DateTime.Now,objetoModificado,
                    estudioAnterior, estudioActual, "Se ha modificado el Estudio",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(modalidadGestionIdAnterior, modalidadGestionIdActual))
            {
                var modalidadGestionAnterior = (modalidadGestionIdAnterior > 0)?context.ModalidadesDeGestion.
                    Where(x => x.Id == modalidadGestionIdAnterior).FirstOrDefault().Nombre:"";
                var modalidadGestionActual = (modalidadGestionIdActual > 0)?context.ModalidadesDeGestion
                    .Where(x => x.Id == modalidadGestionIdActual).FirstOrDefault().Nombre:"";

                if (sonDistintos(modalidadGestionAnterior, modalidadGestionActual)) { 
                    loguearModificaciones(context, DateTime.Now,objetoModificado,
                    modalidadGestionAnterior.ToString(), modalidadGestionActual, "Se ha modificado la Modalidad de Gestión",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(subTipoProcesoAnteriorId,subTipoProcesoActualId))
            {
                var subTipoProcesoAnterior = (subTipoProcesoAnteriorId > 0)?context.SubTiposDeProceso
                    .Where(x => x.Id == subTipoProcesoAnteriorId).FirstOrDefault().Nombre:"";
                var subTipoProcesoActual = (subTipoProcesoActualId > 0)?context.SubTiposDeProceso
                    .Where(x => x.Id == subTipoProcesoActualId).FirstOrDefault().Nombre:"";

                if (sonDistintos(subTipoProcesoAnterior,subTipoProcesoActual)) { 
                    loguearModificaciones(context, DateTime.Now,objetoModificado,
                    subTipoProcesoAnterior.ToString(),subTipoProcesoActual, "Se ha modificado el subTipo de Proceso",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(servicioIdAnterior,servicioIdActual))
            {
                var servicioAnterior = "";
                if (servicioIdAnterior.HasValue) {
                    var servicio = context.Servicios.Where(x => x.Id == servicioIdAnterior.Value).FirstOrDefault();
                    servicioAnterior = (servicio != null)?servicio.Nombre:"";
                } 
                //var servicioAnterior = (servicioIdAnterior != null && servicioIdAnterior > 0)?context.Servicios.Where(x => x.Id == servicioIdAnterior).FirstOrDefault().Nombre : "";

                var servicioActual = (servicioIdActual != null && servicioIdActual > 0)?context.Servicios
                    .Where(x => x.Id == servicioIdActual).FirstOrDefault().Nombre:"";
                if (sonDistintos(servicioAnterior, servicioActual)) { 
                    loguearModificaciones(context, DateTime.Now,objetoModificado,
                    servicioAnterior, servicioActual, "Se ha modificado el Servicio",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(reclamoIdAnterior, reclamoIdActual))
            {
                // primero actualizar el modelo de datos.
                var ReclamoAnterior = context.Reclamos.Where(reclamo => reclamo.Id == reclamoIdAnterior).FirstOrDefault();
                var ReclamoActual = context.Reclamos.Where(reclamo => reclamo.Id == reclamoIdActual).FirstOrDefault();


                var motivoReclamoAnterior = (ReclamoAnterior != null)?context.MotivosDeReclamo
                                                  .Where(x => x.Id == ReclamoAnterior.Id_Motivo_Reclamo)
                                                  .FirstOrDefault().Nombre:"";
                var motivoReclamoActual = (ReclamoActual != null)?context.MotivosDeReclamo
                                                  .Where(x => x.Id == ReclamoActual.Id_Motivo_Reclamo)
                                                  .FirstOrDefault().Nombre:"";

                if (sonDistintos(motivoReclamoAnterior, motivoReclamoActual)) { 
                    loguearModificaciones(context, DateTime.Now, objetoModificado,
                    motivoReclamoAnterior.ToString(), motivoReclamoActual, "Se ha modificado el motivo De Reclamo",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(conciliacionIdAnterior,conciliacionIdActual))
            {
                var estadoConciliacionAnterior = (conciliacionIdAnterior > 0)?context.SubEstados
                    .Where(x => x.Id == conciliacionIdAnterior).FirstOrDefault().Nombre:"";
                var estadoConciliacionActual = (conciliacionIdActual > 0)? context.SubEstados
                    .Where(x => x.Id == conciliacionIdActual).FirstOrDefault().Nombre:"";

                if (sonDistintos(estadoConciliacionAnterior,estadoConciliacionActual)) { 
                    loguearModificaciones(context, DateTime.Now, objetoModificado,
                    estadoConciliacionAnterior, estadoConciliacionActual, "Se ha modificado el Estado de Conciliación",
                    this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(fechaResultadoAnterior, fechaResultadoActual))
            {              
                    var fechaAnterior = (fechaResultadoAnterior != null)? fechaResultadoAnterior:"";
                    var fechaActual = (fechaResultadoActual != null) ? fechaResultadoActual : "";
                    loguearModificaciones(context, DateTime.Now, objetoModificado,
                    fechaAnterior, fechaActual, "Se ha modificado la fecha Resultado",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(grupoIdAnterior, grupoIdActual))
            {
                var grupoAnterior = (grupoIdAnterior > 0) ? grupoIdAnterior.ToString():"" ;
                var grupoActual = (grupoIdActual > 0) ? grupoIdActual.ToString():"";

              if (sonDistintos(grupoAnterior,grupoActual)) { 
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                grupoAnterior,grupoActual, "Se ha modificado el grupo de Denunciantes",
                this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(nroClienteContratoAnterior,nroClienteContratoActual))
            {
                var nroAnterior = (nroClienteContratoAnterior != null) ? nroClienteContratoAnterior :"";
                var nroActual = (nroClienteContratoActual != null) ? nroClienteContratoActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                nroAnterior, nroActual, "Se ha modificado el Nro. Cliente Contrato",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(mediadorIdAnterior, mediadorIdActual))
            {
                var mediadorAnterior = (mediadorIdAnterior > 0)?context.Mediadores.Where(x => x.Id == mediadorIdAnterior).FirstOrDefault().Nombre:"";
                var mediadorActual = (mediadorIdActual > 0)?context.Mediadores.Where(x => x.Id == mediadorIdActual).FirstOrDefault().Nombre:"";

                if (sonDistintos(mediadorAnterior, mediadorActual)) { 
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                mediadorAnterior,mediadorActual, "Se ha modificado el Mediador",
                this.usuario, this.objetoId);
                }
            }
            if (sonDistintos(domicilioMediadorIdAnterior,domicilioMediadorIdActual))
            {
                var domicilioMediadorAnterior = (domicilioMediadorIdAnterior>0)?context.DomiciliosMediadores.Where(x => x.Id == domicilioMediadorIdAnterior).FirstOrDefault().Domicilio:"";
                var domicilioMediadorActual = (domicilioMediadorIdActual>0)?context.DomiciliosMediadores.Where(x => x.Id == domicilioMediadorIdActual).FirstOrDefault().Domicilio:"";

                if (sonDistintos(domicilioMediadorAnterior,domicilioMediadorActual))
                    loguearModificaciones(context, DateTime.Now, objetoModificado,
                    domicilioMediadorAnterior, domicilioMediadorActual, "Se ha modificado el Domicilio del Mediador",
                    this.usuario, this.objetoId);
            }
            if (sonDistintos(reclamoRelacionadoAnterior, reclamoRelacionadoActual))
            {
                var reclamoAnterior = (reclamoRelacionadoAnterior!= null) ? reclamoRelacionadoAnterior :"";
                var reclamoActual = (reclamoRelacionadoActual != null) ? reclamoRelacionadoActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                reclamoAnterior, reclamoRelacionadoActual, "Se ha modificado el Reclamo Relacionado",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(fechaHomologacionAnterior,fechaHomologacionActual))
            {
                var fechaAnterior = (fechaHomologacionAnterior != null) ? fechaHomologacionAnterior :"";
                var fechaActual = (fechaHomologacionActual != null) ? fechaHomologacionActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                fechaAnterior, fechaActual, "Se ha modificado la Fecha de Homologación",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(nroGestionAnterior,nroGestionActual))
            {
                var nroAnterior = (nroGestionAnterior != null) ? nroGestionAnterior:"";
                var nroActual = (nroGestionActual != null) ? nroGestionActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                nroAnterior,nroActual, "Se ha modificado el Nro. de Gestión",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(honorarioAnterior,honorarioActual))
            {
                var anterior = (honorarioAnterior != null) ? honorarioAnterior:"";
                var actual = (honorarioActual != null) ? honorarioActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                anterior.ToString(), actual.ToString(), "Se han modificado los honorarios",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(fechaGestionHonorariosAnterior,fechaGestionHonorariosActual))
            {
                var anterior = (fechaGestionHonorariosAnterior != null) ? fechaGestionHonorariosAnterior : "";
                var actual = (fechaGestionHonorariosActual != null) ? fechaGestionHonorariosActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                anterior.ToString(),actual.ToString(), "Se ha modificado la fecha Gestión de honorarios",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(montoAcordadoAnterior, montoAcordadoActual))
            {
                var anterior = (montoAcordadoAnterior != null) ? montoAcordadoAnterior : "";
                var actual = (montoAcordadoActual != null) ? montoAcordadoActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                anterior.ToString(),actual.ToString(), "Se ha modificado el Monto Acordado",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(arancelAnterior,arancelActual))
            {
                var anterior = (arancelAnterior != null) ? arancelAnterior : "";
                var actual = (arancelActual != null) ? arancelActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                anterior.ToString(), actual.ToString(), "Se ha modificado el Arancel",
                this.usuario, this.objetoId);
            }
            if (sonDistintos(fechaGestionArancelAnterior, fechaGestionArancelActual))
            {
                var anterior = (fechaGestionArancelAnterior != null) ? fechaGestionArancelAnterior : "";
                var actual = (fechaGestionArancelActual != null) ? fechaGestionArancelActual : "";
                loguearModificaciones(context, DateTime.Now, objetoModificado,
                anterior.ToString(), actual.ToString(), "Se ha modificado la fecha Gestión del Arancel",
                this.usuario, this.objetoId);
            }
            // aplicar sobre todos los campos de Coprec

        }

        public bool sonDistintos(string a,string b) {
            return a != b;
        }

        public bool sonDistintos(int? a, int? b){
            return a != b;
        }

        public bool sonDistintos(int a, int b)
        {
            return a != b;
        }

        public bool sonDistintos(DateTime a,DateTime b)
        {
            return a != b;
        }

        private void loguearModificaciones(NuevoDbContext context, DateTime fechaCambio,
            string objetoModificado, string valorAnterior, string valorActual,
            string descripcion, string usuario, int objetoId)
        {
            var logger = new DenChLoggerDto();
            logger.FechaCambio = fechaCambio;
            logger.ObjetoModificado = objetoModificado;
            logger.Descripcion = descripcion;
            logger.ValorAnterior = valorAnterior;
            logger.ValorActual = valorActual;
            logger.Usuario = usuario;
            logger.ObjetoId = objetoId;
            context.Add(logger);
            context.SaveChanges();
            //context.Database
            //.ExecuteSqlCommand("Insert into tDenChLogger values(@fechaCambio,@objetoModificado,@descripcion,@valorAnterior,@valorActual,@usuario,@objetoId)"
            //, new SqlParameter("@fechaCambio", fechaCambio)
            //, new SqlParameter("@objetoModificado", objetoModificado)
            //, new SqlParameter("@descripcion", descripcion)
            //, new SqlParameter("@valorAnterior", valorAnterior)
            //, new SqlParameter("@valorActual", valorActual)
            //, new SqlParameter("@usuario", usuario)
            //, new SqlParameter("@objetoId", objetoId));
        }
    }
}