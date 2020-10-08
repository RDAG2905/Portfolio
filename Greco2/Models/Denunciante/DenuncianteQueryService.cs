using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greco2.Model;
using Greco2.Models.Denunciante;

namespace Greco2.Models.Denunciante
{
    public class DenuncianteQueryService
    {

        public DenuncianteQueryService() {
 
        }

        public List<DenuncianteDto> getDenunciantes() {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenunciantes().ToList();
            }
        }

        public DenuncianteDto GetDenuncianteById(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenunciantes().Where(t=>t.DenuncianteId == id).FirstOrDefault();
            }
            
        }

        public DenuncianteDto GetDenunciantePorNombreyApellido(string nombre,string apellido)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenunciantes().Where(t => t.nombre == nombre  &&  t.apellido == apellido ).FirstOrDefault();
            }

        }
        public DenuncianteDto GetDenunciantePorDni(int? dni)
        {
            var xDni = (dni != null) ? Convert.ToString(dni) : "";
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenunciantes().Where(t => String.Equals(t.NroDocumento.Trim(),xDni.Trim())).FirstOrDefault();
            }

        }
        public DenuncianteDto GetDenunciantePorNombreApellidoyDni(string nombre, string apellido,int? dni)
        {
            var xDni = (dni != null) ? Convert.ToString(dni) : "";
            using (NuevoDbContext context = new NuevoDbContext())
            {
                return context.getDenunciantes().Where(t => String.Equals(t.nombre.Trim(),nombre.Trim()) && String.Equals(t.apellido.Trim(),apellido.Trim()) && String.Equals(t.NroDocumento.Trim(),xDni.Trim())).FirstOrDefault();
            }

        }

    }
}