using Greco2.Models.DenChLogger;
using Greco2.Models.Evento;
using Greco2.Models.Grupo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciaDto
    {
        //public int DenunciaId { get; set; }
        //public DateTime CREATIONDATE { get; set; }
        //public string CREATIONPERSON { get; set; }
        //public int DENUNCIANTE_ID { get; set; }
        //public int? CONCILIACION_ID { get; set; }
        //public int? IMPUTACION_ID { get; set; }
        //public int? SANCION_ID { get; set; }
        //public int? ESTUDIO_ID { get; set; }

        //public DateTime FSELLOGCIADC { get; set; }
        //public DateTime FSELLOCIA { get; set; }

        //public bool INACTIVO { get; set; }
        //public int? MODALIDADGESTION { get; set; }

        //public string OBSERVACIONES { get; set; }
        //public int? ORGANISMO_ID { get; set; }

        //public int? RESP_EXT_ID { get; set; }
        //public int? RESP_INT_ID { get; set; }
        //public int? SERV_DEN_ID { get; set; }
        //public int? TIPO_PRO_ID { get; set; }
        //public int? SUBTIPO_PRO_ID { get; set; }
        //public int? RECLAMO_ID { get; set; }
        //public int? EXPEDIENTE_ID { get; set; }

        //public int? RESULTADO_ID { get; set; }
        //public bool? DELETED { get; set; }
        //public int? ETAPA_ID { get; set; }

        //public string OBJETORECLAMO { get; set; }
        //public string FECHARESULTADO { get; set; }
        //public int? PARENTDENUNCIAID { get; set; }
        //public int? MOTIVOBAJA_ID { get; set; }
        //public string TRAMITECRM { get; set; }

        //public string ECMID { get; set; }
        //public int? grupoId { get; set; }
        //public string nroClienteContrato { get; set; }
        //public int? mediadorId { get; set; }
        //public int? domicilioMediadorId { get; set; }
        //public string reclamoRelacionado { get; set; }

        //public string fechaHomologacion { get; set; }
        //public string nroGestionCoprec { get; set; }
        //public string honorariosCoprec { get; set; }
        //public string fechaGestionHonorarios { get; set; }
        //public string montoAcordado { get; set; }
        //public string arancel { get; set; }
        //public string fechaGestionArancel { get; set; }
        //public string agendaCoprec { get; set; }

        public int DenunciaId { get; set; }
        public DateTime CREATIONDATE { get; set; }
        public string CREATIONPERSON { get; set; }
        public int DENUNCIANTE_ID { get; set; }
        public int? CONCILIACION_ID { get; set; }
        public int? IMPUTACION_ID { get; set; }
        public int? SANCION_ID { get; set; }
        public int? ESTUDIO_ID { get; set; }

        public DateTime FSELLOGCIADC { get; set; }
        public DateTime FSELLOCIA { get; set; }

        public bool INACTIVO { get; set; }
        public int? MODALIDADGESTION { get; set; }

        public string OBSERVACIONES { get; set; }
        public int? ORGANISMO_ID { get; set; }

        public int? RESP_EXT_ID { get; set; }
        public int? RESP_INT_ID { get; set; }
        public int? SERV_DEN_ID { get; set; }
        public int? TIPO_PRO_ID { get; set; }
        public int? SUBTIPO_PRO_ID { get; set; }
        public int? RECLAMO_ID { get; set; }
        public int? EXPEDIENTE_ID { get; set; }

        public int? RESULTADO_ID { get; set; }
        public bool? DELETED { get; set; }
        public int? ETAPA_ID { get; set; }

        public string OBJETORECLAMO { get; set; }
        //public string FECHARESULTADO { get; set; }
        public DateTime? FECHARESULTADO { get; set; }
        public int? PARENTDENUNCIAID { get; set; }
        public int? MOTIVOBAJA_ID { get; set; }
        public string TRAMITECRM { get; set; }

        public string ECMID { get; set; }
        public int? grupoId { get; set; }
        public string nroClienteContrato { get; set; }
        public int? mediadorId { get; set; }
        public int? domicilioMediadorId { get; set; }
        public string reclamoRelacionado { get; set; }

        public DateTime? fechaHomologacion { get; set; }
        public string nroGestionCoprec { get; set; }
        public string honorariosCoprec { get; set; }
        public DateTime? fechaGestionHonorarios { get; set; }
        public string montoAcordado { get; set; }
        public string arancel { get; set; }
        public DateTime? fechaGestionArancel { get; set; }
        public string agendaCoprec { get; set; }
        //public string fechaHomologacion { get; set; }
        //public string nroGestionCoprec { get; set; }
        //public string honorariosCoprec { get; set; }
        //public string fechaGestionHonorarios { get; set; }
        //public string montoAcordado { get; set; }
        //public string arancel { get; set; }
        //public string fechaGestionArancel { get; set; }
        //public string agendaCoprec { get; set; }

        //public ICollection<DenChLoggerDto> denChloggers { get; set; }
        //public ICollection<EventoDto> eventos { get; set; }
        //public ICollection<TextoDenunciaDto> textosDenuncia { get; set; }
        //public DenuncianteDto Denunciante { get; set; }
        //public EstudioDto Estudio { get; set; }
        //public ExpedienteDto Expediente { get; set; }
        //public MediadorDto Mediador { get; set; }
        //public ModalidadGestionDto ModalidadGestion { get; set; }
        //public MotivoDeBajaDto MotivoDeBaja { get; set; }
        //public OrganismoDto Organismo { get; set; }
        //public ReclamoDto Reclamo { get; set; }
        //public ResponsableDto Responsable { get; set; }
        //public ServicioDto Servicio { get; set; }
        //public SubEstadoDto SubEstado { get; set; }
        //public SubTipoProcesoDto SubTipoProceso { get; set; }
        //public TipoProcesoDto TipoProceso { get; set; }
        //public DomicilioMediadorDto DomicilioMediador { get; set; }
        //public ResultadoDto Resultado { get; set; }



    }
}

