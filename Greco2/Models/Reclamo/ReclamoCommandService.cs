using Greco2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Reclamo
{
    public class ReclamoCommandService
    {
        public ReclamoCommandService() {
                    
        }

        public ReclamoDto crearReclamo(int? motivoReclamoId) {
            ReclamoDto reclamo = new ReclamoDto();
            reclamo.Id_Motivo_Reclamo = motivoReclamoId;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(reclamo);
                context.SaveChanges();
                return reclamo;
            }
            
        }

        
    }
}