using Greco2.Model;
using Greco2.Models.MotivoDeReclamo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{
    public class DenunciaQueryService
    {
        public bool laDenunciaEstaCerrada;

        public DenunciaQueryService()
        {

        }

        public List<DenunciaDto> getDenuncias()
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenuncias().ToList();
            }
        }

        public DenunciaDto GetDenunciaById(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var denuncia = context.getDenuncias().Where(t => t.DenunciaId == id).FirstOrDefault();
                this.laDenunciaEstaCerrada = false;
                //this.laDenunciaEstaCerrada = (denuncia.CONCILIACION_ID > 0) ? context.SubEstados.Where(x => x.Id == denuncia.CONCILIACION_ID).FirstOrDefault().CierraDenuncia : false;
                return denuncia;
                
            }

        }

        public int? getMotivoDeReclamoPorReclamo(int? reclamoId) {
            MotivoDeReclamoDto motivoDeReclamo = new MotivoDeReclamoDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
               
                var reclamo = context.Reclamos.Where(t => t.Id == reclamoId).FirstOrDefault();
                if (reclamo != null) { 
                    motivoDeReclamo = context.MotivosDeReclamo.Where(m => m.Id == reclamo.Id_Motivo_Reclamo).FirstOrDefault();
                }
                
                if (motivoDeReclamo != null)
                {
                    return motivoDeReclamo.Id;
                }
                else {
                    return null;
                }
                
            }
            
        }

        public string getNumeroExpediente(int? expedienteId)
        {
            ExpedienteDto expediente = new ExpedienteDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                expediente = context.Expedientes.Where(t => t.Id == expedienteId).FirstOrDefault();               
            }
            if (expediente != null)
            {
                return expediente.Numero;
            }
            else {
                return null;
            }
        }


    }
}