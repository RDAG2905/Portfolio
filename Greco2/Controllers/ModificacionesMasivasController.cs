using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Estado;
using Greco2.Models.Estudios;
using Greco2.Models.Evento;
using Greco2.Models.ModalidadGestión;
using Greco2.Models.Region;
using Greco2.Models.Responsables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{

    [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
    public class ModificacionesMasivasController : Controller
    {
        // GET: ModificacionesMasivas
        public ActionResult Index()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View();
        }

        public ActionResult FiltrosCambiosMasivos()
        {
            return View("FiltrosCambiosMasivos", new FiltroDenunciasModelView());
        }

        public PartialViewResult GetFiltroDenuncias() {
            return PartialView("FiltroDenunciasPartialView", new FiltroDenunciasModelView());
        }

        public PartialViewResult GetFiltroAgenda()
        {
            return PartialView("FiltroAgendaPartialView", new AgendaModelView());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEstudios()
        {
            
            var estudios = new List<EstudioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                estudios = context.Estudios.OrderBy(x => x.Nombre).ToList();
            }
            return Json(estudios);
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetResponsables()
        {
            
                var responsables = new List<ResponsableDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    responsables = context.Responsables.Where(x=>x.TipoResponsable == "RI").OrderBy(x => x.Apellido).ToList();
                }
                return Json(responsables);
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEstadosConciliacion()
        {                       
                var subEstados = new List<string>();
                IEnumerable<SelectListItem> estadosConciliacion;
                using (NuevoDbContext context = new NuevoDbContext())
                {                   
                    subEstados = context.Database
                                        .SqlQuery<string>("select distinct nombre from tSubEstados where EstadoId = 1 order by nombre")
                                        .ToList();
                }
                estadosConciliacion = subEstados.Select(p => new SelectListItem { Text = p });
                return Json(estadosConciliacion);
          

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetModalidadesDeGestion()
        {
          
                var modalidadesDeGestion = new List<ModalidadGestionDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    modalidadesDeGestion = context.ModalidadesDeGestion.OrderBy(x => x.Nombre).ToList();
                }
                return Json(modalidadesDeGestion);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetRegiones()
        {
           
                var regiones = new List<RegionDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    regiones = context.Regiones.OrderBy(x => x.Nombre).ToList();
                }
                return Json(regiones);
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEstudios(int? criterio,int[] lista) {
           
            string idDenuncias = string.Join(",", lista);
            List<ListaSabanaMM> resultados = new List<ListaSabanaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext()) {
                resultados = await context.Database
                                          .SqlQuery<ListaSabanaMM>("UpdateEstudios @IdDenuncias,@regionId,@user"
                                          , new SqlParameter("@IdDenuncias",idDenuncias)
                                          , new SqlParameter("@regionId", criterio)
                                          , new SqlParameter("@user",usuario)).ToListAsync();                       
            }
            return PartialView("ListaSabanaResultMM", resultados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateResponsables(int? criterio, int[] lista)
        {
            string idDenuncias = string.Join(",", lista);
            List<ListaSabanaMM> resultados = new List<ListaSabanaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext())
            {
             resultados = await context.Database
                       .SqlQuery<ListaSabanaMM>("exec UpdateResponsables @IdDenuncias,@responsableId,@user"
                       , new SqlParameter("@IdDenuncias", idDenuncias)
                       , new SqlParameter("@responsableId", criterio)
                       , new SqlParameter("@user",usuario)
                       ).ToListAsync();
            }
            return PartialView("ListaSabanaResultMM", resultados);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEstadosConciliacion(string criterio, int[] lista)
        {
            string idDenuncias = string.Join(",", lista);
            List<ListaSabanaMM> resultados = new List<ListaSabanaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //traer todos los estados anteriores con su id Denuncia
                //luego loguear las modificaciones
                //creaer el sp para el logueo masivo de cambios.. Aplicar este proceso para todos los cambios masivos.
                //posiblemente insertar en una tabla intermedia los estados anteriores con la denuncia. 
                //insert into en una suconsulta sobre el lista de string de denuncias   
                resultados =  await context.Database
                       .SqlQuery<ListaSabanaMM>("exec UpdateEstadosConciliacion @IdDenuncias,@subEstado,@user"
                       , new SqlParameter("@IdDenuncias", idDenuncias)
                       , new SqlParameter("@subEstado", criterio)
                       , new SqlParameter("@user",usuario)
                       ).ToListAsync();
                
            }
            return PartialView("ListaSabanaResultMM",resultados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateModalidadesGestion(int? criterio, int[] lista)
        {
            string idDenuncias = string.Join(",", lista);
            List<ListaSabanaMM> resultados = new List<ListaSabanaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext())
            {
                resultados = await context.Database
                       .SqlQuery<ListaSabanaMM>("UpdateModalidadesGestion @IdDenuncias,@modalidadId,@user"
                       , new SqlParameter("@IdDenuncias", idDenuncias)
                       , new SqlParameter("@modalidadId", criterio)
                       , new SqlParameter("@user",usuario))
                       .ToListAsync();
            }
            return PartialView("ListaSabanaResultMM", resultados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateRegiones(int? criterio, int[] lista)
        {
            string idDenuncias = string.Join(",", lista);
            List<ListaSabanaMM> resultados = new List<ListaSabanaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext())
            {
                resultados = await context.Database
                       .SqlQuery<ListaSabanaMM>("UpdateRegiones @IdDenuncias,@regionId,@user"
                       , new SqlParameter("@IdDenuncias", idDenuncias)
                       , new SqlParameter("@regionId", criterio)
                       , new SqlParameter("@user", usuario))
                       .ToListAsync();
            }
            return PartialView("ListaSabanaResultMM", resultados);
           
        }
        //return Json(new { criterio, lista });
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateResponsableEventos(int? criterio, int[] lista)
        {
            string idEventos = string.Join(",", lista);
            List<AgendaMM> resultados = new List<AgendaMM>();
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            //var usuario = Session["usuarioLogueado"];
            using (NuevoDbContext context = new NuevoDbContext())
            {
                resultados = await context.Database
                       .SqlQuery<AgendaMM>("UpdateResponsableEventos @IdEventos,@responsableId,@user"
                       , new SqlParameter("@IdEventos", idEventos)
                       , new SqlParameter("@responsableId", criterio)
                       , new SqlParameter("@user", usuario))
                       .ToListAsync();
            }
            return PartialView("ListAgendaResultMM", resultados);
        }
        
    } 
}