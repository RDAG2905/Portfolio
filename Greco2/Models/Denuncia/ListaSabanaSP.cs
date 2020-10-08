using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class ListaSabanaSP
    {
        public int DenunciaId { get; set; }
        [DisplayName("Nro. Expediente")]
        //public string EXPEDIENTE_ID{ get; set; }
        public string Expediente{ get; set; }
        [DisplayName("Fecha Creacion")]
        //public string Fecha_Creacion { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public string Organismo { get; set; }        
        public string Servicio { get; set; }
        public string Estudio { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Denunciante { get; set; }
        public string Denunciante_Individual { get; set; }
        [DisplayName("Etapa Procesal")]
        public string Etapa_Procesal { get; set; }
        [DisplayName("Estado Actual")]
        public string Estado_Actual { get; set; }
        public string Estado_Anterior { get; set; }
        [DisplayName("Tipo Proceso")]
        public string Tipo_Proceso { get; set; }
        
        public string fechaNotifDesde { get; set; }
        public string fechaNotifHasta { get; set; }
        public string fechaNotifGciaDesde { get; set; }
        public string fechaNotifGciaHasta { get; set; }
        //public DateTime FSELLOCIA { get; set; }
        //public DateTime FSELLOGCIADC { get; set; }
        public DateTime Fecha_Notificacion { get; set; }
        public DateTime Fecha_Notificacion_Gcia { get; set; }
        public DateTime Fecha_Cambio { get; set; }
        public string Usuario_Creador { get; set; }
        public int? ETAPA_ID { get; set; }
        public int? ORGANISMO_ID { get; set; }/*organismoId*/
        public int? Region_Id { get; set; }
        public int? Provincia_Id { get; set; }
        public int? Localidad_Id { get; set; }
        public string Responsable_Interno { get; set; }
        public int? SERV_DEN_ID { get; set; }
        //public int? DenunciaId { get; set; }
        public int? ESTUDIO_ID { get; set; }
        public int? TIPO_PRO_ID { get; set; }

        //public int? Nro_Documento { get; set; }
        public string Nro_Documento { get; set; }
        public int? CONCILIACION_ID { get; set; }
        public int? PARENTDENUNCIAID { get; set; }
        public string Nro_Linea { get; set; }
        public string Tramite_CRM { get; set; }
        public int? RECLAMO_ID { get; set; }
        public int? RESP_INT_ID { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Region { get; set; }
        public string Responsable { get; set; } //(u582027)apellido,nombre
        [DisplayName("Modalidad de Gestión")]
        public string Modalidad_Gestion { get; set; }
        [DisplayName("Sub tipo de Proceso")]
        public string Sub_Tipo_Proceso { get; set; }
        //public string FECHARESULTADO { get; set; }
        public DateTime? FECHARESULTADO { get; set; }
        //public string Fecha_Resultado { get; set; }
        public DateTime? Fecha_Resultado { get; set; }
        public string Motivo_Reclamo { get; set; }
        public int? Denuncia_Preventiva { get; set; }
        public int? Denuncia_Formal { get; set; }
        public bool? INACTIVO { get; set; }
        public bool? DELETED { get; set; }
        public string Mediador { get; set; }
        public string Matricula { get; set; }
        public string Domicilio_Mediador { get; set; }
        //public string Fecha_Homologacion { get; set; }
        public DateTime? Fecha_Homologacion { get; set; }
        public string Nro_Gestion_Coprec { get; set; }
        public string Honorarios_Coprec { get; set; }
        //public string Fecha_Gestion_Honorarios { get; set; }
        public DateTime? Fecha_Gestion_Honorarios { get; set; }
        public string Monto_Acordado { get; set; }
        public string Arancel { get; set; }
        //public string Fecha_Gestion_Arancel { get; set; }
        public DateTime? Fecha_Gestion_Arancel { get; set; }
        public string Agenda_Coprec { get; set; }
        public DateTime? FSELLOCIA { get; set; }
        public string ResponsableApellido { get; set; }
        public string ResponsableNombre { get; set; }

    }
}