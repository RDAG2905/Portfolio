using Greco2.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Greco2.Models.Evento
{
    public class EventoQueryService
    {
        public EventoQueryService()
        {

        }

        public List<EventoSP> getEventos(int idDenuncia)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.Database
                                       .SqlQuery<EventoSP>("GetEventsByDenunciaId @denunciaId", new SqlParameter("@denunciaId", idDenuncia))
                                       .Where(x => !x.Deleted).ToList();
                //return context.getEventos(true).Where(e => e.DenunciaId == idDenuncia).ToList();
            }
        }

        public EventoDto GetEventoById(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getEventos().Where(t => t.EventoId == id).FirstOrDefault();
            }

        }
    }
}