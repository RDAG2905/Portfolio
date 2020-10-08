using Greco2.Model;
using Greco2.Models.Denunciante;
using Greco2.Models.Estado;
using Greco2.Models.Estudios;
using Greco2.Models.Evento;
using Greco2.Models.Mediador;
using Greco2.Models.ModalidadGestión;
using Greco2.Models.MotivoDeReclamo;
using Greco2.Models.Organismo;
using Greco2.Models.OrganismosEstudiosRel;
using Greco2.Models.ReqInforme;
using Greco2.Models.Responsables;
using Greco2.Models.Servicio;
using Greco2.Models.Solucionado;
using Greco2.Models.TipoEvento;
using Greco2.Models.TipoProceso;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Models.Denuncia
{
    public class DenunciaModelView
    {
        public DenunciaModelView() {
        }

        public DenunciaModelView(int? tipoProcesoId)
        {
            var estados = new List<EstadoDto>();
            //var estado = new EstadoDto();
            var subEstados = new List<SubEstadoDto>();
            var tiposProceso = new List<TipoProcesoDto>();
            var servicios = new List<ServicioDto>();
            var organismos = new List<OrganismoDto>();
            var estudios = new List<EstudioDto>();
            var tipoEventos = new List<TipoEventoDto>();
            var subtipoProcesos = new List<SubTipoProcesoDto>();
            var modalidadesDeGestion = new List<ModalidadGestionDto>();
            var motivosReclamo = new List<MotivoDeReclamoDto>();
            var reqsInforme = new List<ReqInformeDto>();
            var mediadores = new List<MediadorDto>();
            var domicilios = new List<DomicilioMediadorDto>();
            var solucionados = new List<SolucionadoDto>();
            var denuncia = new DenunciaDto();
            var responsables = new List<ResponsableDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                estados = context.Estados.OrderBy(e => e.TipoEstado).ToList();

                subEstados = context.SubEstados.Where(s => s.EstadoId == 1 && !s.Deleted).OrderBy(e => e.Nombre).ToList();
                
                tiposProceso = context.TiposDeProceso.OrderBy(e => e.Nombre).ToList();
                servicios = context.Servicios.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();
                organismos = context.Database.SqlQuery<OrganismoDto>("select * from tOrganismos where Activo=0 order by nombre")
                                                 .ToList();
                
                
                estudios = context.Estudios.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();
                
                tipoEventos = context.TiposDeEventos.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();

                subtipoProcesos = context.SubTiposDeProceso.Where(e => e.Tipo_Id == tipoProcesoId).OrderBy(e => e.Nombre).ToList();
                modalidadesDeGestion = context.ModalidadesDeGestion.OrderBy(e => e.Nombre).ToList();

                
                //motivosReclamo = context.MotivosDeReclamo.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();
                
                reqsInforme = context.ReqsInforme.OrderBy(e => e.Nombre).ToList();
                mediadores = context.Mediadores.Where(e => e.Activo).OrderBy(e => e.Nombre).ToList();
                domicilios = context.DomiciliosMediadores.Where(e => e.Activo).OrderBy(e => e.Domicilio).ToList();
                solucionados = context.Solucionados.OrderBy(e => e.Nombre).ToList();
                responsables = context.Responsables.Where(e => !e.Deleted && String.Equals(e.TipoResponsable, "RI")).OrderBy(e => e.Apellido).ToList();
            }
            this.Estados = estados.Select(p => new SelectListItem { Text = p.TipoEstado, Value = p.Id.ToString()/*,Disabled = (p.Nombre != "Conciliación")*/});
            this.SubEstados = subEstados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tiposProceso = tiposProceso.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Servicios = servicios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tipoEventos = tipoEventos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.modalidadesDeGestion = modalidadesDeGestion.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.subTiposProceso = subtipoProcesos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.motivosDeReclamo = motivosReclamo.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });


            this.reqsInforme = reqsInforme.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.mediadores = mediadores.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.domiciliosMediadores = domicilios.Select(p => new SelectListItem { Text = p.Domicilio, Value = p.Id.ToString() });
            this.solucionados = solucionados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.denuncia = denuncia;
            this.denuncianteIndividual = denuncia.grupoId == null;
            this.RESP_INT_ID = denuncia.RESP_INT_ID;
            this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre, Value = p.Id.ToString() });
            //this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId + ')', Value = p.Id.ToString() });
            this.eventos = new List<EventoSP>();
            this.evento = new EventoDto();

        }

        public DenunciaModelView(int? organismoId, int? servicioId,int? etapaProcesalId,int? tipoProcesoId)
        {
            var estados = new List<EstadoDto>();
            //var estado = new EstadoDto();
            var subEstados = new List<SubEstadoDto>();
            var tiposProceso = new List<TipoProcesoDto>();
            var servicios = new List<ServicioDto>();
            var organismos = new List<OrganismoDto>();
            var estudios = new List<EstudioDto>();
            var tipoEventos = new List<TipoEventoDto>();
            var subtipoProcesos = new List<SubTipoProcesoDto>();
            var modalidadesDeGestion = new List<ModalidadGestionDto>();
            var motivosReclamo = new List<MotivoDeReclamoDto>();
            var reqsInforme = new List<ReqInformeDto>();
            var mediadores = new List<MediadorDto>();
            var domicilios = new List<DomicilioMediadorDto>();
            var solucionados = new List<SolucionadoDto>();
            var denuncia = new DenunciaDto();
            var responsables = new List<ResponsableDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                estados = context.Estados.OrderBy(e=>e.TipoEstado).ToList();
                if (etapaProcesalId != null)
                {
                    subEstados = context.SubEstados.Where(s => s.EstadoId == etapaProcesalId ).OrderBy(e => e.Nombre).ToList();
                }
                else {
                    subEstados = context.SubEstados.Where(s => s.EstadoId == 1 ).OrderBy(e => e.Nombre).ToList();
                    
                    //subEstados = context.Database.SqlQuery<string>("select distinct nombre from tSubEstados where Deleted=0 order by nombre")
                    //                             .ToList();
                }
                
                tiposProceso = context.TiposDeProceso.OrderBy(e => e.Nombre).ToList();
                servicios = context.Servicios.OrderBy(e => e.Nombre).ToList();
                organismos = context.Database.SqlQuery<OrganismoDto>("select * from tOrganismos order by nombre")
                                                 .ToList();
               

                if (organismoId != null)
                {
                    var estudioIdRelacion = context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetOrganismoEstudioRelPorOrganismoId @organismoId", new SqlParameter("@organismoId", organismoId))
                        .ToList().Select(e=>e.EstudioRelacionId);
                    var totalEstudios = context.Estudios.OrderBy(e => e.Nombre).ToList();
                    foreach (var item in totalEstudios) {
                        if (estudioIdRelacion.Contains(item.Id)) {
                            estudios.Add(item);
                        }
                    }
                }
                else {
                    estudios = context.Estudios.OrderBy(e => e.Nombre).ToList();
                }
                nombreOrganismoActual = (organismoId > 0)?context.Organismos.Where(org => org.Id == organismoId).FirstOrDefault().Nombre:"";
                laDenunciaEsDeCoprec = (nombreOrganismoActual.Contains("COPREC")|| nombreOrganismoActual.Contains("Coprec")); 
                tipoEventos = context.TiposDeEventos.Where(e => !e.Deleted).OrderBy(e => e.Nombre).ToList();
               
                subtipoProcesos = context.SubTiposDeProceso.Where(e=>e.Tipo_Id == tipoProcesoId).OrderBy(e => e.Nombre).ToList();
                modalidadesDeGestion = context.ModalidadesDeGestion.OrderBy(e => e.Nombre).ToList();

                if (servicioId != null)
                {
                    motivosReclamo = context.MotivosDeReclamo.Where(m=>m.servicioId == servicioId && m.tipoProcesoId == tipoProcesoId).OrderBy(e => e.Nombre).ToList();
                }
                else {
                    motivosReclamo = context.MotivosDeReclamo.OrderBy(e => e.Nombre).ToList();
                }
                
                reqsInforme = context.ReqsInforme.OrderBy(e => e.Nombre).ToList();
                mediadores = context.Mediadores.OrderBy(e => e.Nombre).ToList();
                domicilios = context.DomiciliosMediadores.OrderBy(e => e.Domicilio).ToList();
                solucionados = context.Solucionados.OrderBy(e => e.Nombre).ToList();
                responsables = context.Responsables.Where(e => String.Equals(e.TipoResponsable, "RI")).OrderBy(e => e.Apellido).ToList();
            }

            this.Estados = estados.Select(p => new SelectListItem { Text = p.TipoEstado, Value = p.Id.ToString()/*,Disabled = (p.Nombre != "Conciliación")*/});
            this.SubEstados = subEstados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tiposProceso = tiposProceso.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Servicios = servicios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.tipoEventos = tipoEventos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.modalidadesDeGestion = modalidadesDeGestion.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.subTiposProceso = subtipoProcesos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.motivosDeReclamo = motivosReclamo.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            

            this.reqsInforme = reqsInforme.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.mediadores = mediadores.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.domiciliosMediadores = domicilios.Select(p => new SelectListItem { Text = p.Domicilio, Value = p.Id.ToString() });
            this.solucionados = solucionados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            this.denuncia = denuncia;
            this.denuncianteIndividual = denuncia.grupoId == null;
            this.RESP_INT_ID = denuncia.RESP_INT_ID;
            this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre , Value = p.Id.ToString() });
            //this.responsables = responsables.Select(p => new SelectListItem { Text = p.Apellido + ',' + p.Nombre + '(' + p.UmeId + ')', Value = p.Id.ToString() });
            this.eventos = new List<EventoSP>();
            this.evento = new EventoDto();
            
        }
        public DenunciaDto denuncia { get; set; }
        public DenuncianteDto denunciante { get; set; }
        //public string datosDenunciante { get; set; }
        //public string nombreDenunciante { get; set; }
        //public string apellidoDenunciante { get; set; }
        //public string dniDenunciante { get; set; }
        public IEnumerable<SelectListItem> Estados { get; set; }
        public string estadoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> SubEstados { get; set; }
        public string subEstadoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> tiposProceso { get; set; }
        public string tipoProcesoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> subTiposProceso { get; set; }
        public string subTipoProcesoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> modalidadesDeGestion { get; set; }
        public string modalidadGestionSeleccionada { get; set; }
        public IEnumerable<SelectListItem> Servicios { get; set; }
        public string servicioSeleccionado { get; set; }
        public IEnumerable<SelectListItem> Organismos { get; set; }
        public string organismoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> Estudios { get; set; }
        public string estudioSeleccionado { get; set; }
        public IEnumerable<SelectListItem> tipoEventos { get; set; }
        public string tipoEventoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> motivosDeReclamo { get; set; }
        public string motivoReclamoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> reqsInforme { get; set; }
        public string reqInformeSeleccionado { get; set; }
        public IEnumerable<SelectListItem> mediadores { get; set; }
        public string mediadorSeleccionado { get; set; }
        public IEnumerable<SelectListItem> domiciliosMediadores { get; set; }
        public string domicilioMediadorSeleccionado { get; set; }
        public IEnumerable<SelectListItem> solucionados { get; set; }
        public string solucionadoSeleccionado { get; set; }
        public IEnumerable<SelectListItem> responsables { get; set; }
        public string responsableSeleccionado { get; set; }
        public Array grupoDenunciantes { get; set; }

        public EventoDto evento { get; set; }
        public int DenunciaId { get; set; } 
        public DateTime CREATIONDATE { get; set; }//hidden
        public string CREATIONPERSON { get; set; } 
        public int DENUNCIANTE_ID { get; set; }//hidden
        public int? CONCILIACION_ID { get; set; }
       
        public int? ESTUDIO_ID { get; set; }
        public string FCREACION { get; set; }
        //public DateTime FSELLOGCIADC { get; set; }
        //public DateTime FSELLOCIA { get; set; }
        public string FSELLOGCIADC { get; set; }
        public string FSELLOCIA { get; set; }

        public int? MODALIDADGESTION { get; set; }
        public int? NROAUDIENCIA { get; set; }
        public int? NROCASO { get; set; }
        public int? NROCUARTOINTERMEDIO { get; set; }
        public string OBSERVACIONES { get; set; }
        public int? ORGANISMO_ID { get; set; }
        public string RECEPCIONTARDIA { get; set; }
        public int? RESP_EXT_ID { get; set; }
        public int? RESP_INT_ID { get; set; }
        public int? SERV_DEN_ID { get; set; }
        public int? TIPO_PRO_ID { get; set; }  //------
        public int? SUBTIPO_PRO_ID { get; set; } //--------
        public int? RECLAMO_ID { get; set; }
        public int? MotivoReclamoId { get; set; }
        public string EXPEDIENTE { get; set; }
        

        public int? RESULTADO_ID { get; set; }
        public bool? DELETED { get; set; }
        public int? ETAPA_ID { get; set; } //
        
        public string OBJETORECLAMO { get; set; }
        public string FECHARESULTADO { get; set; }
        //public DateTime FECHARESULTADO { get; set; }
        public int? PARENTDENUNCIAID { get; set; }
        public int? MOTIVOBAJA_ID { get; set; }
        public string TRAMITECRM { get; set; }
        
        public int tipoEvento { get; set; }
        public int reqInforme { get; set; }
        public string fechaVencimiento { get; set; }
        public string horaVencimiento { get; set; }
        public string grupoId { get; set; }
        public string nroClienteContrato { get; set; }
        public int mediadorId { get; set; }
        public int domicilioMediadorId { get; set; }
        public string reclamoRelacionado { get; set; }
        public string tipoDenuncia { get; set; }
        public bool denuncianteIndividual { get; set; }
        public int? idDatosCoprec { get; set; }
        public bool Inactivo { get; set; }
        //public IEnumerable<EventoDto> eventos { get; set; }
        public IEnumerable<EventoSP> eventos { get; set; }
        public List<ArchivoDto> archivos { get; set; }

        //public int? ECMID { get; set; }
        public string INFORME { get; set; }
        public int? IMPUTACION_ID { get; set; }
        public int? SANCION_ID { get; set; }
        public int? responsableId { get; set; }
        public int? responsableEvento { get; set; }
        public string fechaHomologacion { get; set; }
        public string nroGestionCoprec { get; set; }
        public string honorariosCoprec { get; set; }
        public string fechaGestionHonorarios { get; set; }
        public string montoAcordado { get; set; }
        public string arancel { get; set; }
        public string fechaGestionArancel { get; set; }

        public bool laDenunciaEsDeCoprec { get; set; }
        public string nombreOrganismoActual { get; set; }
        public bool laDenunciaEstaCerrada { get; set; }
        public string ecmUrl { get; set; }

        public void LoadEstudiosSP(int? id,DenunciaModelView model) {
            var estudiosPorOrganismo = new List<EstudioRelSP>();
            var organismoId = Convert.ToInt32(id);
            using (NuevoDbContext context = new NuevoDbContext()) {
                estudiosPorOrganismo = context.Database
                        .SqlQuery<EstudioRelSP>("GetEstudiosRelPorOrganismoId", new SqlParameter("@organismoId", organismoId))
                        .ToList();
            }
            model.Estudios = estudiosPorOrganismo.Select(p => new SelectListItem { Text = p.Estudio, Value = p.EstudioRelacionId.ToString() });
        }
    }
}