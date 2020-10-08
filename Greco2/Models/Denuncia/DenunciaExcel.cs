using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciaExcel
    {
        public int DenunciaId;
        public string TipoProceso;
        public string Etapa_Procesal;
        public string usuarioCreador;
        public string EXPEDIENTE_ID;
        public string Denunciante;
        public string DenuncianteIndividual;
        public int? nroDocumento;
        public string nro_Linea;
        public string TRAMITECRM;
        public string FCREACION;
        public DateTime FSELLOCIA;
        public DateTime FSELLOGCIADC;
        public string Organismo;
        public string Localidad;
        public string Provincia;
        public string Region;
        public string Responsable_Interno;
        public string Estudio;
        public string Modalidad_Gestion;
        public string Sub_Tipo_Proceso;
        public string Servicio;
        public string Motivo_Reclamo;
        public string EstadoActual;
        public string FECHARESULTADO;
        public int? denunciaPreventiva;
        public int? denunciaFormal;

    }
}