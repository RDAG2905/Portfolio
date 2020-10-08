using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Reportes
{
    public class ReporteSabanaSP
    {
        public int DenunciaId { get; set; }
        public string TipoProceso { get; set; }
        public string EtapaProcesal { get; set; }
        public string UsuarioCreador { get; set; }
        public string nroExpediente { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string tipoDenunciante { get; set; } //individual/Colectivo
        public string nroDocumento { get; set; }
        public string numeroDeLinea { get; set; }
        public string tramiteCRM { get; set; }
        public string fechaCreacion { get; set; }
        public string notificacion { get; set; }
        public string notificacionGcia { get; set; }
        public string Organismo { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Region { get; set; }
        public string Responsable { get; set; }
        public string Estudio { get; set; }
        public string modalidadGestion { get; set; }
        public string subTipoProceso { get; set; }
        public string Servicio { get; set; }
        public string motivoReclamo { get; set; }
        public string EstadoConciliacion { get; set; } //Estado Actual
        public string fechaResultado { get; set; }
        public string denunciaPreventiva { get; set; }
        public string denunciaFormal { get; set; }

        
        
        
        
        
    }
}