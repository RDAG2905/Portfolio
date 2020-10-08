using Greco2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Expediente
{
    public class ExpedienteCommandService
    {
        public ExpedienteDto crearExpediente(string unExpediente)
        {
            ExpedienteDto expedienteDto = new ExpedienteDto();
            expedienteDto.Numero = unExpediente;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                context.Add(expedienteDto);
                context.SaveChanges();
                return expedienteDto;
            }

        }
    }
}