using Greco2.Models;
using Greco2.Models.MotivoDeBaja;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using Greco2.Models.Localidad;
using Greco2.Models.Provincia;
using System.Data.SqlClient;
using Greco2.Models.Estudios;
using Greco2.Models.Region;
using Greco2.Models.MotivoDeReclamo;
using Greco2.Models.Organismo;
using Greco2.Models.Mediador;
using Greco2.Models.Servicio;
using Greco2.Models.Estado;
using Greco2.Models.TipoEvento;
using Greco2.Models.Denunciante;
using Greco2.Model;
using Greco2.Models.OrganismosEstudiosRel;
using Greco2.Models.Enum;
using Greco2.Models.Responsables;
using Greco2.Models.Atributos;
using Greco2.Models.Estudio;
using Greco2.Models.Log;
using Greco2.Models.Paginacion;

namespace Greco2.Controllers
{
    [ActionAuthorize(Roles = "ADMINISTRADOR,COORDINADOR,GERENTE")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.usuarioLogueado = Session["usuarioLogueado"];
            ViewBag.nombreUsuario = Session["nombreUsuario"];
            ViewBag.rolUsuario = Session["rolUsuario"];
            return View();
            
        }



        /**************** 
          Motivos de Baja
         ****************/

        public ActionResult GestionarMotivosDeBaja()
        {
            return PartialView("GestionMotivosDeBaja");
        }

        public async Task<ActionResult> ListarMotivosDeBaja(string filtro)
        {
            List<MotivoDeBajaDto> lista = new List<MotivoDeBajaDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {
                    lista = await context.MotivosDeBaja.OrderBy(x=>x.Nombre).ToListAsync();
                }
                else
                {
                    lista = await context.MotivosDeBaja.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
            }
            return PartialView("ListMotivosDeBaja", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMotivoDeBaja(string Nombre)
        {
            List<MotivoDeBajaDto> lista = new List<MotivoDeBajaDto>();
            var mbcs = new MotivoDeBajaCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!mbcs.existeMotivoDeBaja(Nombre))
                {
                    var motivo = mbcs.createMotivo(Nombre);
                    lista.Add(motivo);
                    ViewBag.Success = "Registro guardado correctamente";
                }
            }
            return PartialView("ListMotivosDeBaja", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteMotivoDeBaja(string Nombre)
        {
            var mbcs = new MotivoDeBajaCommandService();
            if (mbcs.existeMotivoDeBaja(Nombre))
            {
                return Json("</br><div class='alert alert-danger text-center'>No es posible concretar la acción. Ya existe El Motivo de Baja</div>");
            }
            else
            {
                return JavaScript("guardarMotivoDeBaja()");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMotivoDeBaja(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
               
                var existe = context.Denuncias.Any(x => x.MOTIVOBAJA_ID == id);
                if (!existe)
                {
                    MotivoDeBajaDto motivo = context.getMotivos(true)
                                                                  .Where(t => t.Id == id)
                                                                  .FirstOrDefault();
                    context.Remove(motivo);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE BAJA", "Se ha Eliminado el Motivo de Baja : " + motivo.Nombre, "EXISTENTE", "ELIMINADO", usuario, motivo.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    return Json("Registro eliminado con éxito");

                }
                else {
                    return Json("El elemento posee Relaciones. No se puede eliminar");
                }
                
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElMotivoDeBaja(int motivoBajaId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.Denuncias.Any(x => x.MOTIVOBAJA_ID == motivoBajaId);
                if (existe)
                {
                    cantidadRelaciones = context.Denuncias.Where(x => x.MOTIVOBAJA_ID == motivoBajaId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Denuncias Relacionadas al Motivo de Baja. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("eliminarMotivo()"); /*return JavaScript("<script>toastr.success('Eliminado OK')</script>")*/
                }

            }


        }



        //[HttpPost]
        public JsonResult EditarMotivoDeBaja(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                MotivoDeBajaDto motivo = context.getMotivos(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(motivo, JsonRequestBehavior.AllowGet);
            }

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]//([Bind(Include = "ID,Name,Email")] Student student//int id,string nombreMotivo,bool motivoActivo
        //public ActionResult guardarMotivoDeBajaEditado(int id, string Nombre, string campoActivacion)
        //{
        //    MotivoDeBajaDto motivo = new MotivoDeBajaDto();
        //    motivo.Id = id;
        //    motivo.Nombre = Nombre.ToUpper();
        //    motivo.Deleted = (campoActivacion == "on");
        //    List<MotivoDeBajaDto> lista = new List<MotivoDeBajaDto>();
        //    var mbcs = new MotivoDeBajaCommandService();
        //    var motivoActualizado = mbcs.updateMotivo(motivo);
        //    lista.Add(motivoActualizado);
        //    return PartialView("ListMotivosDeBaja", lista);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarMotivoDeBajaEditado(int id, string Nombre, string campoActivacion)
        {
            var mbcs = new MotivoDeBajaCommandService();
            if (mbcs.existeOtroMotivoDeBajaIgual(Nombre, id))
            {
                return Json("Verifique los datos ingresados.</br>Existe otro Motivo de Baja con el mismo Nombre.");
            }
            else
            {
                MotivoDeBajaDto motivo = new MotivoDeBajaDto();
                motivo.Id = id;
                motivo.Nombre = Nombre.Trim().ToUpper();
                motivo.Deleted = (campoActivacion == "on");
                List<MotivoDeBajaDto> lista = new List<MotivoDeBajaDto>();
               
                var motivoActualizado = mbcs.updateMotivo(motivo);
                return Json("ACTUALIZADO");
            }
        }

        public ActionResult GetMotivoDeBajaActualizado(int id)
        {
            List<MotivoDeBajaDto> lista = new List<MotivoDeBajaDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var motivo = context.MotivosDeBaja.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(motivo);
                ViewBag.Success = "Motivo Actualizado correctamente";
            }

            return PartialView("ListMotivosDeBaja", lista);

        }


        public ActionResult GestionarTipoEventos()
        {
            return PartialView("GestionTipoEventos");
        }

        // *************** //
        //  Regiones     //
        // *************** //

        public ActionResult GestionarRegiones()
        {
            return PartialView("GestionRegiones");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearRegion(string Nombre)
        {
            List<RegionDto> lista = new List<RegionDto>();
            var rcs = new RegionCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!rcs.existeRegion(Nombre))
                {
                    var region = rcs.createRegion(Nombre);
                    lista.Add(region);
                    ViewBag.Success = "Registro guardado correctamente";
                }
                return PartialView("ListRegiones", lista);
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados son inválidos</div>");
            }
            
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeRegion(string Nombre)
        {
            var rcs = new RegionCommandService();
            if (rcs.existeRegion(Nombre.Trim()))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe una Región con el mismo Nombre</div>");
            }
            else
            {
                return JavaScript("guardarNuevaRegion()");
            }

        }



        public async Task<ActionResult> BuscarRegion(string filtro)
        {
            List<RegionDto> lista = new List<RegionDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {

                    lista = await context.Regiones.OrderBy(x=>x.Nombre).ToListAsync();
                }
                else
                {
                    lista = await context.Regiones.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
            }
            return PartialView("ListRegiones", lista);
        }

        //[HttpPost]
        public JsonResult EditarRegion(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                RegionDto region = context.getRegiones(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                if (region != null)
                {
                    return Json(region, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No se encontraron coincidencias", JsonRequestBehavior.AllowGet);
                }
               
            }
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarRegionEditada(int id, string nombre, string campoActivacion)
        {
            var rcs = new RegionCommandService();

            if (!String.IsNullOrEmpty(nombre))
            {
                if (rcs.existeOtraRegionIgual(nombre, id))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otra Region con el mismo Nombre.");
                }
                else
                {
                    RegionDto region = new RegionDto();
                    region.Id = id;
                    region.Nombre = nombre.Trim().ToUpper();
                    region.Deleted = (campoActivacion == "on");
                    rcs.updateRegion(region);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetRegionActualizada(int id)
        {
            List<RegionDto> lista = new List<RegionDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var region = context.Regiones.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(region);
                ViewBag.Success = "Region Actualizada correctamente";
            }

            return PartialView("ListRegiones", lista);

        }



        public async Task<ActionResult> ListarRegiones()
        {
            List<RegionDto> lista = new List<RegionDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Regiones.OrderByDescending(p => p.Id).ToListAsync();

            }
            return PartialView("ListRegiones", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesLaRegion(int regionId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.Organismos.Any(x => x.Region_Id == regionId);
                if (existe)
                {
                    cantidadRelaciones = context.Organismos.Where(x => x.Region_Id == regionId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Organismos Relacionados a la Región. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("eliminarRegion()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarRegion(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existe = context.Organismos.Any(x => x.Region_Id == id);
                if (!existe)
                {
                    RegionDto region = context.getRegiones(true)
                                                                 .Where(t => t.Id == id)
                                                                 .FirstOrDefault();
                    context.Remove(region);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "REGION", "Se ha Eliminado la Región : " + region.Nombre, "EXISTENTE", "ELIMINADA", usuario, region.Id);
                    context.Add(accion);
                    context.SaveChanges();


                    return Json("Registro eliminado con éxito");

                }
                else {
                    return Json("El elemento posee relaciones. No puede ser eliminado");

                }
                   
            }

        }

        


        // *************** //
        //  Provincias     //
        // *************** //

        public ActionResult GestionarProvincias()
        {
            return PartialView("GestionProvincias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearProvincia(string Nombre)/*[Bind(Include ="Nombre")]MotivoDeBajaDto motivoBaja*/
        {
            List<ProvinciaDto> lista = new List<ProvinciaDto>();
            var pcs = new ProvinciaCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!pcs.existeProvincia(Nombre))
                {
                    var provincia = pcs.createProvincia(Nombre);
                    lista.Add(provincia);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListProvincias", lista);

                }
                else {
                    return Json("<div class='alert alert-danger text-center'>El elemento que intenta crear ya existe</div>");
                }
                
            }
            else
            {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados son Inválidos</div>");
            }

        }
    

    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteProvincia(string Nombre)
        {
            var pcs = new ProvinciaCommandService();
            if (pcs.existeProvincia(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe una Provincia con el mismo Nombre</div>");
            }
            else
            {
                return JavaScript("guardarNuevaProvincia()");
            }

        }


        public async Task<ActionResult> ListarProvincias(string filtro)
        {
            List<ProvinciaDto> lista = new List<ProvinciaDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {
                    lista = await context.Provincias.OrderBy(p => p.Nombre).ToListAsync();
                }
                else
                {
                    lista = await context.Provincias.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
            }
            return PartialView("ListProvincias", lista);
        }


        //[HttpPost]
        public JsonResult EditarProvincia(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                //try
                //{
                    ProvinciaDto provincia = context.getProvincias(true)
                                                  .Where(t => t.Id == id)
                                                  .FirstOrDefault();
                    if (provincia != null) {
                        return Json(provincia, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json("No se encontraron coincidencias", JsonRequestBehavior.AllowGet);
                    }
                
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken] 
        //public ActionResult guardarProvinciaEditada(int id, string nombre, string campoActivacion)
        //{
        //    ProvinciaDto provincia = new ProvinciaDto();
        //    provincia.Id = id;
        //    provincia.Nombre = nombre.Trim().ToUpper();
        //    provincia.Deleted = (campoActivacion == "on");
        //    List<ProvinciaDto> lista = new List<ProvinciaDto>();
        //    var pcs = new ProvinciaCommandService();
        //    var provinciaActualizada = pcs.updateProvincia(provincia);
        //    lista.Add(provinciaActualizada);

        //    return PartialView("ListProvincias", lista);
        //}


        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarProvinciaEditada(int id, string nombre, string campoActivacion)
        {
            var pcs = new ProvinciaCommandService();

            if (!String.IsNullOrEmpty(nombre))
            {
                if (pcs.existeOtraProvinciaIgual(nombre, id))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otra Provincia con el mismo Nombre.");
                }
                else
                {
                    ProvinciaDto provincia = new ProvinciaDto();
                    provincia.Id = id;
                    provincia.Nombre = nombre.Trim().ToUpper();
                    provincia.Deleted = (campoActivacion == "on");
                    pcs.updateProvincia(provincia);

                    return Json("ACTUALIZADO");
                }
            }
            else {
                return Json("Los parámetros enviados son inválidos");
            }
                
        }

        public ActionResult GetProvinciaActualizada(int id)
        {
            List<ProvinciaDto> lista = new List<ProvinciaDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var provincia = context.Provincias.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(provincia);
                ViewBag.Success = "Provincia Actualizada correctamente";
            }

            return PartialView("ListProvincias", lista);

        }

        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesLaProvincia(int provinciaId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                string mensajeLocalidad ="";
                string mensajeOrganismos = "";
                var hayRelaciones = false;
                int cantidadRelacionesLocalidad;
                var existeLocalidad = context.Localidades.Any(x => x.ProvinciaId == provinciaId);
                if (existeLocalidad)
                {
                    hayRelaciones = true;
                    cantidadRelacionesLocalidad = context.Localidades.Where(x => x.ProvinciaId == provinciaId).Count();
                    mensajeLocalidad = "Existen Localidades Relacionadas a la Provincia. Total: " + cantidadRelacionesLocalidad + "</br>";
                    hayRelaciones = true;
                }
                int cantidadRelacionesOrganismos;
                var existeOrganismo = context.Organismos.Any(x => x.Provincia_Id == provinciaId);
                if (existeOrganismo)
                {
                    hayRelaciones = true;
                    cantidadRelacionesOrganismos = context.Organismos.Where(x => x.Provincia_Id == provinciaId).Count();
                    mensajeOrganismos = "Existen Organismos Relacionados a la Provincia. Total: " + cantidadRelacionesOrganismos + "</br>";
                    
                }

                if (hayRelaciones) {
                    return Json("<div class='alert alert-danger text-center'>" + mensajeLocalidad+ mensajeOrganismos + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");

                }
                else
                {
                    return JavaScript("eliminarProvincia()"); /*return JavaScript("<script>toastr.success('Eliminado OK')</script>")*/
                }

            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarProvincia(int provinciaId)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existeLocalidad = context.Localidades.Any(x => x.ProvinciaId == provinciaId);
                var existeOrganismo = context.Organismos.Any(x => x.Provincia_Id == provinciaId);

                if (!existeLocalidad && !existeOrganismo)
                {
                    ProvinciaDto provincia = context.getProvincias(true)
                                              .Where(t => t.Id == provinciaId)
                                              .FirstOrDefault();
                    context.Remove(provincia);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "PROVINCIA", "Se ha Eliminado la Provincia : " + provincia.Nombre,"EXISTENTE","ELIMINADA", usuario, provincia.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    return Json("Registro eliminado con éxito");
                }
                else
                {
                    return Json("El elemento contiene relaciones. No se puede eliminar");

                }
                
            }

        }

       

        // *************** //
        //  Localidades  //
        // *************** //

        public ActionResult GestionarLocalidades()
        {
            var model = new LocalidadModelView();
            NuevoDbContext context = new NuevoDbContext();
            model.Provincias = context.Provincias.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });

            return PartialView("GestionLocalidades", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearLocalidad(string Nombre, int provinciaId)
        {
            List<LocalidadSP> lista = new List<LocalidadSP>();
            var lcs = new LocalidadCommandService();

            if (!String.IsNullOrEmpty(Nombre) && (provinciaId > 0))
            {
                if (!lcs.existeLocalidad(Nombre, provinciaId))
                {
                    var localidad = new LocalidadDto();
                    localidad.Nombre = Nombre.Trim().ToUpper();
                    localidad.ProvinciaId = provinciaId;
                    localidad.Deleted = false;
                    lista = lcs.createlocalidad(localidad);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListLocalidadesSP", lista);
                }
                else {

                    return Json("<div class='alert alert-danger text-center'>La Localidad ya existe. Se ha cancelado la operación</div>");
                }
                
            }
            else
            {
                return Json("<div class='alert alert-warning text-center'>Algunos parámetros no llegaron al Servidor.</div>");
            }

            //var localidad = new LocalidadDto();
            //localidad.Nombre = Nombre.ToUpper();
            //localidad.ProvinciaId = provinciaId;
            //localidad.Deleted = false;
            //LocalidadCommandService lcs = new LocalidadCommandService();
            //var lista = await lcs.createlocalidad(localidad);

            //return PartialView("ListLocalidadesSP", lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeLocalidad(string Nombre,int provinciaSeleccionada)
        {
            var lcs = new LocalidadCommandService();
            if (!String.IsNullOrEmpty(Nombre) && provinciaSeleccionada > 0)
            {
                if (lcs.existeLocalidad(Nombre, provinciaSeleccionada))
                {
                    return Json("<div class='alert alert-danger text-center'>La Provincia ya cuenta con esta Localidad</div>");
                }
                else
                {
                    return JavaScript("guardarLocalidad()");
                }
            }
            else {
                return Json("<div class='alert alert-warning text-center'>Algunos parámetros no llegaron al Servidor.</div>");
            }

        }

        public JsonResult EditarLocalidad(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                LocalidadDto localidad = context.getLocalidades(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(localidad, JsonRequestBehavior.AllowGet);
            }

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]//([Bind(Include = "ID,Name,Email")] Student student//int id,string nombreMotivo,bool motivoActivo
        //public async Task<ActionResult> guardarLocalidadEditada(int id, string nombre, int provinciaSeleccionada, string campoActivacion)
        //{
        //    LocalidadDto localidad = new LocalidadDto();
        //    localidad.Id = id;
        //    localidad.Nombre = nombre.Trim().ToUpper();
        //    localidad.ProvinciaId = provinciaSeleccionada;
        //    localidad.Deleted = (campoActivacion == "on");

        //    LocalidadCommandService lcs = new LocalidadCommandService();
        //    var lista = await lcs.updatelocalidad(localidad);

        //    return PartialView("ListLocalidadesSP", lista);
        //}

        // a prueba
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> guardarLocalidadEditada(int id, string nombre, int provinciaSeleccionada, string campoActivacion)
        {
            LocalidadCommandService lcs = new LocalidadCommandService();

            if (lcs.existeOtraLocalidadIgual(nombre,id,provinciaSeleccionada))
            {
                return Json("Verifique los datos ingresados.</br>Existe otra localidad con el mismo Nombre</br> en esta Provincia");
            }
            else
            {
                LocalidadDto localidad = new LocalidadDto();
                localidad.Id = id;
                localidad.Nombre = nombre.Trim().ToUpper();
                localidad.ProvinciaId = provinciaSeleccionada;
                localidad.Deleted = (campoActivacion == "on");
                
                await lcs.updatelocalidad(localidad);
                return Json("ACTUALIZADO");
            }
        }

        public ActionResult GetLocalidadActualizada(int id)
        {
            List<LocalidadSP> lista = new List<LocalidadSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = context.Database
                        .SqlQuery<LocalidadSP>("GetLocalidadesPorId @Id", new SqlParameter("@Id",id))
                        .ToList();
                  ViewBag.Success = "Motivo Actualizado correctamente";
            }

            return PartialView("ListLocalidadesSP", lista);

        }
        //

        //[HttpPost]
        public ActionResult BuscarLocalidad(string nombre, int? provinciaId)
        {

            List<LocalidadSP> lista = new List<LocalidadSP>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (provinciaId == null && string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                       .SqlQuery<LocalidadSP>("GetLocalidades")
                       .ToList();
                }
                else
                if (provinciaId != null && string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                        .SqlQuery<LocalidadSP>("GetLocalidadesPorProvincia @provinciaId", new SqlParameter("@provinciaId", provinciaId))
                        .ToList();
                }
                else
                if (provinciaId == null && !string.IsNullOrEmpty(nombre))
                {

                    lista = context.Database
                        .SqlQuery<LocalidadSP>("GetLocalidadesPorNombre @nombre", new SqlParameter("@nombre", nombre))
                        .ToList();
                }
                else
                if (provinciaId != null && !string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                    .SqlQuery<LocalidadSP>("GetLocalidadesPorNombreyProvincia @nombre,@provinciaId", new SqlParameter("@nombre", nombre), new SqlParameter("@provinciaId", provinciaId))
                    .ToList();
                }
            }
            lista = lista.OrderBy(x=>x.Nombre).ToList();

            return PartialView("ListLocalidadesSP", lista);
        }


        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesLocalidad(int localidadId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.Organismos.Any(x => x.Localidad_Id == localidadId);
                if (existe)
                {
                    cantidadRelaciones = context.Organismos.Where(x => x.Localidad_Id == localidadId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Organismos Relacionados a la Localidad. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado.");
                }
                else
                {
                    return JavaScript("EliminarLocalidad()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarLocalidad(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existe = context.Organismos.Any(x => x.Localidad_Id == id);
                if (!existe)
                {
                    LocalidadDto localidad = context.getLocalidades(true)
                                             .Where(t => t.Id == id)
                                             .FirstOrDefault();
                    context.Remove(localidad);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "LOCALIDAD", "Se ha Eliminado la Localidad : " + localidad.Nombre, "EXISTENTE", "ELIMINADA", usuario, localidad.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("No se puede eliminar el elemento Seleccionado");
                }
               
            }

        }
        





        // *************** //
        //  Estudios     //
        // *************** //

        public ActionResult GestionarEstudios()
        {
            return PartialView("GestionEstudios");
        }



        public async Task<ActionResult> BuscarEstudio(string filtro)
        {
            List<EstudioDto> lista = new List<EstudioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {
                    lista = await context.Estudios.OrderBy(p => p.Nombre).ToListAsync();
                }
                else
                {
                    lista = await context.Estudios.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
            }
            return PartialView("ListEstudios", lista);
        }


        public async Task<ActionResult> ListarEstudios(string filtro)
        {
            List<EstudioDto> lista = new List<EstudioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Estudios.OrderBy(p => p.Nombre).ToListAsync();
            }
            return PartialView("ListEstudios", lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEstudio(string Nombre)
        {
            List<EstudioDto> lista = new List<EstudioDto>();
            var ecs = new EstudioCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!ecs.existeEstudio(Nombre))
                {
                    var estudio = ecs.createEstudio(Nombre);
                    lista.Add(estudio);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListEstudios", lista);
                }
                else
                {
                    return Json("<div class='alert alert-danger text-center'>Ya existe un Estudio con el mismo Nombre</div>");
                }

            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados al Servidor son inválidos</div>");
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeEstudio(string Nombre)
        {         
            var ecs = new EstudioCommandService();
            if (ecs.existeEstudio(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe un Estudio con el mismo Nombre</div>");
            }
            else {
                return JavaScript("guardarEstudio()");
            }

        }


        private int existeRelacionOrganismoEstudio(int estudioId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidad = 0;
                cantidad = context.Database.SqlQuery<int>("existeRelacionOrganismoEstudioPorEstudio @estudioId", new SqlParameter("@estudioId", estudioId)).FirstOrDefault();
                return cantidad;

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElEstudio(int estudioId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelacionesDenuncia;
                bool hayRelaciones = false;
                var mensajeRelacionDenuncia = "";
                var mensajeRelacionOrganismoEstudio = "";
                var mensajeRelacionResponsable = "";
                var existeDenunciaRelacionada = context.Denuncias.Any(x => x.ESTUDIO_ID == estudioId);
                if (existeDenunciaRelacionada)
                {
                    cantidadRelacionesDenuncia = context.Denuncias.Where(x => x.ESTUDIO_ID == estudioId).Count();
                    mensajeRelacionDenuncia = "Existen Denuncias Relacionadas al Estudio. Total : " + cantidadRelacionesDenuncia + "</br>";
                    hayRelaciones = true;
                }
                var cantidadRelacionesOrganismoEstudio = existeRelacionOrganismoEstudio(estudioId);
                if (cantidadRelacionesOrganismoEstudio > 0) {
                    mensajeRelacionOrganismoEstudio = "Existen Organismos Relacionados al Estudio. Total : " + cantidadRelacionesOrganismoEstudio + "</br>";
                    hayRelaciones = true;
                }
                int cantidadRelacionesResponsable;
                var existeResponsableRelacionado = context.Responsables.Any(x => x.Estudio_Id == estudioId);
                if (existeResponsableRelacionado)
                {
                    cantidadRelacionesResponsable = context.Responsables.Where(x => x.Estudio_Id == estudioId).Count();
                    mensajeRelacionResponsable = "Existen Responsables Relacionados al Estudio. Total : " + cantidadRelacionesResponsable + "</br>";
                    hayRelaciones = true;
                }

                if (hayRelaciones) {

                    return Json("<div class='alert alert-danger text-center'>" + mensajeRelacionDenuncia + mensajeRelacionOrganismoEstudio + mensajeRelacionResponsable + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</div>");
                }
                else
                {
                    return JavaScript("EliminarEstudio()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }
            

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarEstudio(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existeDenunciaRelacionada = context.Denuncias.Any(x => x.ESTUDIO_ID == id);
                var cantidadRelacionesOrganismoEstudio = existeRelacionOrganismoEstudio(id);
                var existeResponsableRelacionado = context.Responsables.Any(x => x.Estudio_Id == id);

                if (!existeDenunciaRelacionada && !existeResponsableRelacionado && (cantidadRelacionesOrganismoEstudio < 1)) {
                    EstudioDto estudio = context.getEstudios(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                    context.Remove(estudio);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "ESTUDIO", "Se ha Eliminado el Estudio : " + estudio.Nombre, "EXISTENTE", "ELIMINADO", usuario, estudio.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    return Json("Registro eliminado con éxito");

                }
                else {
                    return Json("El elemento posee relaciones.No se puede eliminar. ");
                }

                
            }

        }

        //[HttpPost]
        public async Task<JsonResult> EditarEstudio(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                EstudioDto estudio = await context.getEstudios(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefaultAsync();
                return Json(estudio, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]   
        [ValidateAntiForgeryToken]
        public ActionResult guardarEstudioEditado(EstudioDto estudio)
        {
            List<EstudioDto> lista = new List<EstudioDto>();
            var ecs = new EstudioCommandService();
            if (ecs.existeOtroEstudioIgual(estudio))
            {
                return Json("Verifique los datos ingresados.</br>Existe otro Estudio con el mismo Nombre.");
            }
            else {
                var estudioActualizado = ecs.updateEstudio(estudio);
                return Json("ACTUALIZADO");
            }
        }

        public ActionResult GetEstudioActualizado(int id)
        {
            List<EstudioDto> lista = new List<EstudioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var estudio = context.Estudios.Where(e=>e.Id == id).FirstOrDefault();
                lista.Add(estudio);
                ViewBag.Success = "Estudio Actualizado correctamente";
            }

            return PartialView("ListEstudios", lista);
           
        }

        

        /*******************************************
         *          Servicios
         *******************************************/
        public ActionResult GestionarServicios()
        {
            return PartialView("GestionServicios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearServicio(string Nombre)
        {
            List<ServicioDto> lista = new List<ServicioDto>();
            var scs = new ServicioCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!scs.existeServicio(Nombre))
                {
                    var servicio = scs.createServicio(Nombre);
                    lista.Add(servicio);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListServicios", lista);
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>El Servicio ya existe. Se cancela la Operación</div>");
                }
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los parámetros enviados son Inválidos</div>");
            }
            //List<ServicioDto> lista = new List<ServicioDto>();
            //var scs = new ServicioCommandService();
            //var servicio = scs.createServicio(Nombre);
            //lista.Add(servicio);

            //return PartialView("ListServicios", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeServicio(string Nombre)
        {
            var scs = new ServicioCommandService();
            if (scs.existeServicio(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe un Servicio con el mismo Nombre</div>");
            }
            else
            {
                return JavaScript("guardarNuevoServicio()");
            }

        }

        public async Task<ActionResult> ListarServicios(string filtro)
        {
            List<ServicioDto> lista = new List<ServicioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Servicios.OrderByDescending(p => p.Id).ToListAsync();

            }
            return PartialView("ListServicios", lista);
        }

        public async Task<ActionResult> BuscarServicio(string filtro)
        {
            List<ServicioDto> lista = new List<ServicioDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {
                    lista = await context.Servicios.OrderBy(x=>x.Nombre).ToListAsync();
                }
                else {
                    lista = await context.Servicios.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }

                
            }
            return PartialView("ListServicios", lista);
        }

        //[HttpPost]
        public JsonResult EditarServicio(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                ServicioDto estudio = context.getServicios(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(estudio, JsonRequestBehavior.AllowGet);
            }

        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarServicioEditado(int id, string Nombre, string campoActivacion)
        {
            var scs = new ServicioCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (scs.existeOtroServicioIgual(Nombre, id))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otro Servicio con el mismo Nombre.");
                }
                else
                {
                    ServicioDto servicio = new ServicioDto();
                    servicio.Id = id;
                    servicio.Nombre = Nombre.ToUpper();
                    servicio.Deleted = (campoActivacion == "on");
                    scs.updateServicio(servicio);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetServicioActualizado(int id)
        {
            List<ServicioDto> lista = new List<ServicioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var servicio = context.Servicios.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(servicio);
                ViewBag.Success = "Servicio Actualizado correctamente";
            }

            return PartialView("ListServicios", lista);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElServicio(int servicioId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                string mensajeDenuncias = "";
                string mensajeMotivosDeReclamo = "";
                var hayRelaciones = false;
                int cantidadRelacionesDenuncia;
                var existeDenuncia = context.Denuncias.Any(x => x.SERV_DEN_ID == servicioId);
                if (existeDenuncia)
                {
                    hayRelaciones = true;
                    cantidadRelacionesDenuncia = context.Denuncias.Where(x => x.SERV_DEN_ID == servicioId).Count();
                    mensajeDenuncias = "Existen Denuncias Relacionadas al Servicio. Total: " + cantidadRelacionesDenuncia + "</br>";
                    
                }
                int cantidadRelacionesMotivosDeReclamo;
                var existeMotivosDeReclamo = context.MotivosDeReclamo.Any(x => x.servicioId == servicioId);
                if (existeMotivosDeReclamo)
                {
                    hayRelaciones = true;
                    cantidadRelacionesMotivosDeReclamo = context.MotivosDeReclamo.Where(x => x.servicioId == servicioId).Count();
                    mensajeMotivosDeReclamo = "Existen Motivos de Reclamo Relacionados al Servicio. Total: " + cantidadRelacionesMotivosDeReclamo + "</br>";

                }

                if (hayRelaciones)
                {
                    return Json("<div class='alert alert-danger text-center'>" + mensajeDenuncias + mensajeMotivosDeReclamo + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</div>");

                }
                else
                {
                    return JavaScript("eliminarServicio()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }
               

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarServicio(int servicioId)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existenDenunciasRelacionadas = context.Denuncias.Any(x => x.SERV_DEN_ID == servicioId);
                var existenMotivosDeReclamoRelacionados = context.MotivosDeReclamo.Any(x => x.servicioId == servicioId);

                if (!existenDenunciasRelacionadas && !existenMotivosDeReclamoRelacionados) {
                    ServicioDto servicio = context.getServicios(true)
                                              .Where(t => t.Id == servicioId)
                                              .FirstOrDefault();
                    context.Remove(servicio);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "SERVICIO", "Se ha Eliminado el Servicio: " + servicio.Nombre, "EXISTENTE", "ELIMINADO", usuario, servicio.Id);
                    context.Add(accion);
                    context.SaveChanges();


                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("El elemento posee relaciones. No puede ser eliminado");
                }

                
            }

        }

       
        /*******************************************
         *          Motivos de ReclamoDto
         *******************************************/

        public ActionResult GestionarMotivosDeReclamo()
        {
            MotivoDeReclamoModelView model = new MotivoDeReclamoModelView();
            NuevoDbContext context = new NuevoDbContext();

            model.Servicios = context.Servicios.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.TiposDeProceso = context.TiposDeProceso.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });

            return PartialView("GestionMotivosDeReclamo", model);
        }


       

        //[HttpPost]
        public JsonResult EditarMotivoDeReclamo(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                MotivoDeReclamoDto motivo = context.getMotivosDeReclamo(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(motivo, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMotivoDeReclamo(string nombre, int servicioId, int tipoProcesoId)
        {
            List<MotivoDeReclamoSP> lista = new List<MotivoDeReclamoSP>();
            var mrcs = new MotivoDeReclamoCommandService();

            if (!String.IsNullOrEmpty(nombre))
            {
                if (!mrcs.existeMotivoDeReclamo(nombre, servicioId, tipoProcesoId))
                {
                    var motivoDeReclamo = new MotivoDeReclamoDto();
                    motivoDeReclamo.Nombre = nombre.Trim().ToUpper();
                    motivoDeReclamo.servicioId = servicioId;
                    motivoDeReclamo.tipoProcesoId = tipoProcesoId;
                    motivoDeReclamo.Deleted = false;
                    lista = mrcs.createMotivoDeReclamo(motivoDeReclamo);
                    ViewBag.Success = "Registro creado correctamente";
                    return PartialView("ListMotivosDeReclamoSP", lista);
                }
                else {
                    return Json("<div class='alert alert-warning text-center'>El elemento que intenta crear ya existe</div>");
                }
            }
            else {
                return Json("<div class='alert alert-warning text-center'>Los parámetros enviados son Inválidos</div>");
            }
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeMotivoDeReclamo(string Nombre, int servicioId, int tipoProcesoId)
        {
            var mrcs = new MotivoDeReclamoCommandService();
            if (mrcs.existeMotivoDeReclamo(Nombre, servicioId, tipoProcesoId))
            {
                return Json("<div class='alert alert-danger text-center'>El Motivo de Reclamo que intenta crear ya existe para el Servicio </br> y el Tipo de Proceso Seleccionado</div>");
            }
            else
            {
                return JavaScript("guardarNuevoMotivoDeReclamo()");
            }

        }

        public async Task<ActionResult> BuscarMotivoDeReclamo(string nombre, int? servicioId, int? tipoDeProcesoId) {

            List<MotivoDeReclamoSP> lista = new List<MotivoDeReclamoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(nombre) && servicioId == null && tipoDeProcesoId == null)
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamo")
                   .ToListAsync();
                }
                else
               if (!String.IsNullOrEmpty(nombre) && servicioId == null && tipoDeProcesoId == null)
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorNombre @nombre", new SqlParameter("@nombre", nombre))
                   .ToListAsync();
                } else
                  if (servicioId != null && String.IsNullOrEmpty(nombre) && tipoDeProcesoId == null)
                {
                    lista = await context.Database
                    .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorServicio @servicioId", new SqlParameter("@servicioId", servicioId))
                    .ToListAsync();

                }
                else
                if (tipoDeProcesoId != null && servicioId == null && String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorTipoDeProceso @tipoDeProcesoId", new SqlParameter("@tipoDeProcesoId", tipoDeProcesoId))
                   .ToListAsync();

                }
                else
                if (tipoDeProcesoId != null && servicioId != null && !String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoWithAllParameters @nombre,@servicioId,@tipoDeProcesoId", new SqlParameter("@nombre", nombre), new SqlParameter("@servicioId", servicioId), new SqlParameter("@tipoDeProcesoId", tipoDeProcesoId))

                   .ToListAsync();

                }
                else
                if (tipoDeProcesoId != null && servicioId != null && String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorProcesoyServicio @servicioId,@tipoDeProcesoId", new SqlParameter("@servicioId", servicioId), new SqlParameter("@tipoDeProcesoId", tipoDeProcesoId))

                   .ToListAsync();

                }
                else
                if (tipoDeProcesoId == null && servicioId != null && !String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorNombreyServicio @nombre,@servicioId", new SqlParameter("@nombre", nombre), new SqlParameter("@servicioId", servicioId))

                   .ToListAsync();

                }
                else
                if (tipoDeProcesoId != null && servicioId == null && !String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Database
                   .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorNombreyProceso @nombre,@tipoDeProcesoId", new SqlParameter("@nombre", nombre), new SqlParameter("@tipoDeProcesoId", tipoDeProcesoId))

                   .ToListAsync();

                }

            }
            lista = lista.OrderBy(x => x.Nombre).ToList();
            return PartialView("ListMotivosDeReclamoSP", lista);
        }

        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElMotivoDeReclamo(int reclamoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
              
                var cantidad = 0;
                cantidad= context.Database
                   .SqlQuery<int>("ExisteRelacionReclamoDenunciaMotivoDeReclamo @id", new SqlParameter("@id",reclamoId))
                   .FirstOrDefault();

                if (cantidad > 0)
                {
                    return Json("<div class='alert alert-danger text-center'>Existen Denuncias Relacionadas al Motivo de Reclamo. Total : " + cantidad + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación ha sido cancelada.</div>");
                }
                else
                {
                    return JavaScript("EliminarMotivoDeReclamo()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMotivoDeReclamo(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var cantidad = 0;
                cantidad = context.Database
                   .SqlQuery<int>("ExisteRelacionReclamoDenunciaMotivoDeReclamo @id", new SqlParameter("@id",id))
                   .FirstOrDefault();

                if (cantidad < 1)
                {
                    MotivoDeReclamoDto motivoDeReclamo = context.getMotivosDeReclamo(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                    context.Remove(motivoDeReclamo);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "MOTIVO DE RECLAMO", "Se ha Eliminado el Motivo de Reclamo: " + motivoDeReclamo.Nombre, "EXISTENTE", "ELIMINADA", usuario, motivoDeReclamo.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("El elemento posee relaciones. No se puede eliminar");
                }
                
            }

        }


        public async Task<ActionResult> ListarMotivosDeReclamo()
        {
            List<MotivoDeReclamoDto> lista = new List<MotivoDeReclamoDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.MotivosDeReclamo.OrderBy(p => p.Nombre).ToListAsync();
            }
            return PartialView("ListMotivosDeReclamo", lista);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarMotivoDeReclamoEditado(int id, string Nombre, int servicioId, int tipoProcesoId, string campoActivacion)
        {
            var mrcs = new MotivoDeReclamoCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (mrcs.existeOtroMotivoDeReclamoIgual(Nombre,id,servicioId,tipoProcesoId))
                {
                    return Json("No se pueden actualizar los datos.</br>Ya existe el Motivo de Reclamo para este Servicio</br> y Tipo de Proceso.");
                }
                else
                {
                    MotivoDeReclamoDto motivoReclamo = new MotivoDeReclamoDto();
                    motivoReclamo.Id = id;
                    motivoReclamo.Nombre = Nombre.Trim().ToUpper();
                    motivoReclamo.servicioId = servicioId;
                    motivoReclamo.tipoProcesoId = tipoProcesoId;
                    motivoReclamo.Deleted = (campoActivacion == "on");
                    mrcs.updateMotivoDeReclamo(motivoReclamo);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetMotivodeReclamoActualizado(int id)
        {
            List<MotivoDeReclamoSP> lista = new List<MotivoDeReclamoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                 var motivoDeReclamo = context.Database
                    .SqlQuery<MotivoDeReclamoSP>("GetMotivosDeReclamoPorId @Id", new SqlParameter("@Id",id))
                    .FirstOrDefault();
                lista.Add(motivoDeReclamo);
                ViewBag.Success = "Motivo de Reclamo Actualizado";
            }

            return PartialView("ListMotivosDeReclamoSP", lista);

        }




        /********************************/
        /***         Estados          ***/
        /********************************/

        public ActionResult GestionarEstados()
        {
            var model = new EstadoModelView();
            NuevoDbContext context = new NuevoDbContext();
            model.estados = context.Estados.Select(p => new SelectListItem { Text = p.TipoEstado, Value = p.Id.ToString() });
            return PartialView("GestionEstados", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearSubEstado(string subEstado, int etapaProcesal, bool cierraDenuncia)
        {
            List<SubEstadoSP> lista = new List<SubEstadoSP>();
            var secs = new SubEstadoCommandService();

            if (!String.IsNullOrEmpty(subEstado))
            {
                if (!secs.existeSubEstado(subEstado, etapaProcesal))
                {
                    var nuevoSubEstado = new SubEstadoDto();
                    nuevoSubEstado.Nombre = subEstado.Trim().ToUpper();
                    nuevoSubEstado.EstadoId = etapaProcesal;
                    nuevoSubEstado.CierraDenuncia = cierraDenuncia;
                    nuevoSubEstado.Deleted = false;
                    lista = secs.createSubEstado(nuevoSubEstado);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListSubEstadosSP", lista);
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>El elemento que intenta crear ya existe</div>");
                }
            }
               
            else {

                return Json("<div class='alert alert-danger text-center'>Los parámetros enviados son inválidos</div>");
            }
            


            //var nuevoSubEstado = new SubEstadoDto();
            //    nuevoSubEstado.Nombre = subEstado.ToUpper();
            //    nuevoSubEstado.EstadoId = etapaProcesal;
            //    nuevoSubEstado.CierraDenuncia = cierraDenuncia;
            //    nuevoSubEstado.Deleted = false;
            //var secs = new SubEstadoCommandService();
            //var lista = secs.createSubEstado(nuevoSubEstado);

            //context.Add(nuevoSubEstado);
            //context.SaveChanges();
            //var id = nuevoSubEstado.Id;
            //lista = await context.Database
            //        .SqlQuery<SubEstadoSP>("GetSubEstadosPorId @id", new SqlParameter("@id", id))
            //        .ToListAsync();

            //return null;
            //return PartialView("ListSubEstadosSP", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesSubEstado(int subEstadoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.Denuncias.Any(x => x.CONCILIACION_ID == subEstadoId);
                if (existe)
                {
                    cantidadRelaciones = context.Denuncias.Where(x => x.CONCILIACION_ID == subEstadoId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Denuncias Relacionadas al Estado de Expediente. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("EliminarSubEstado()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarSubEstado(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {

                var existe = context.Denuncias.Any(x => x.CONCILIACION_ID == id);
                if (!existe)
                {
                    SubEstadoDto subEstado = context.GetSubEstados(true)
                                          .Where(t => t.Id == id)
                                          .FirstOrDefault();
                    context.Remove(subEstado);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "SUB ESTADO", "Se ha Eliminado el SubEstado : " + subEstado.Nombre,"EXISTENTE","ELIMINADO", usuario, subEstado.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("El elemento seleccionado posee relaciones. No se puede eliminar");
                }

                
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeSubEstado(string Nombre,int etapaProcesal)
        {
            var secs = new SubEstadoCommandService();
            if (secs.existeSubEstado(Nombre,etapaProcesal))
            {
                return Json("<div class='alert alert-danger text-center'>No se puede concretar la Acción </br>el Estado que intenta crear ya existe para esta Etapa Procesal</div>");
            }
            else
            {
                return JavaScript("guardarNuevoSubEstado()");
            }

        }


        public async Task<ActionResult> BuscarSubEstado(string nombre, int? estadoId)
        {
            List<SubEstadoSP> lista = new List<SubEstadoSP>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(nombre) && estadoId == null)
                {
                    lista = await context.Database
                   .SqlQuery<SubEstadoSP>("GetSubEstados")
                   .ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && estadoId == null)
                {
                    lista = await context.Database
                   .SqlQuery<SubEstadoSP>("GetSubEstadosPorNombre @nombre", new SqlParameter("@nombre", nombre))
                   .ToListAsync();
                }
                else
                  if (String.IsNullOrEmpty(nombre) && estadoId != null)
                {
                    lista = await context.Database
                    .SqlQuery<SubEstadoSP>("GetSubEstadosPorEtapaProcesal @estadoId", new SqlParameter("@estadoId", estadoId))
                    .ToListAsync();

                }
                else
                if (!String.IsNullOrEmpty(nombre) && estadoId != null)
                {
                    lista = await context.Database
                   .SqlQuery<SubEstadoSP>("GetSubEstadosPorNombreyEtapaProcesal @nombre,@estadoId", new SqlParameter("@nombre", nombre), new SqlParameter("@estadoId", estadoId))
                   .ToListAsync();

                }
            }
            lista = lista.OrderBy(x=>x.Nombre).ToList();
            return PartialView("ListSubEstadosSP", lista);
        }


        //[HttpPost]
        public JsonResult EditarSubEstado(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                SubEstadoDto estudio = context.GetSubEstados(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(estudio, JsonRequestBehavior.AllowGet);
            }

        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult guardarSubEstadoEditado(int id, string Nombre, int estadoId, string campoActivacion, bool cierraDenuncia)
        //{
            
        //    SubEstadoDto subEstado = new SubEstadoDto();
        //    subEstado.Id = id;
        //        subEstado.Nombre = Nombre.ToUpper();
        //        subEstado.EstadoId = estadoId;
        //        subEstado.Deleted = (campoActivacion == "on") ;
        //        subEstado.CierraDenuncia = cierraDenuncia;
        //    var ecs = new SubEstadoCommandService();
        //    var lista = ecs.updateSubEstado(subEstado);

                
        //    return PartialView("ListSubEstadosSP", lista);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarSubEstadoEditado(int id, string Nombre, int estadoId, string campoActivacion, bool cierraDenuncia)
        {
            var secs = new SubEstadoCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (secs.existeOtroSubEstadoIgual(Nombre,id,estadoId))
                {
                    return Json("No se pueden actualizar los datos.</br>Ya Existe otro Estado con el mismo Nombre</br>para esta Etapa Procesal");
                }
                else
                {
                    SubEstadoDto subEstado = new SubEstadoDto();
                    subEstado.Id = id;
                    subEstado.Nombre = Nombre.Trim().ToUpper();
                    subEstado.EstadoId = estadoId;
                    subEstado.Deleted = (campoActivacion == "on");
                    subEstado.CierraDenuncia = cierraDenuncia;
                    secs.updateSubEstado(subEstado);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetSubEstadoActualizado(int id)
        {
            List<SubEstadoSP> lista = new List<SubEstadoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
               
                var subEstado = context.Database
                    .SqlQuery<SubEstadoSP>("GetSubEstadosPorId @Id", new SqlParameter("@Id",id))
                    .FirstOrDefault();
                lista.Add(subEstado);
                ViewBag.Success = "Estado Actualizado correctamente";
            }

            return PartialView("ListSubEstadosSP", lista);

        }



        //[HttpPost]
        //public JsonResult EliminarSubEstado(int id)
        //{
        //    try
        //    {
        //        using (NuevoDbContext context = new NuevoDbContext())
        //        {
        //            SubEstadoDto subestado = context.GetSubEstados(true)
        //                                          .Where(t => t.Id == id)
        //                                          .FirstOrDefault();
        //            context.Remove(subestado);
        //            context.SaveChanges();
        //            return Json("Registro eliminado con éxito");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(e.Message);
        //    }
        //}


        /*************************/
        /* Gestión de Organismos */
        /*************************/

        public ActionResult GestionarOrganismos()
        {
            OrganismoModelView model = new OrganismoModelView();
            NuevoDbContext context = new NuevoDbContext();

            model.Provincias = context.Provincias.OrderBy(x=>x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Localidades = context.Localidades.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Regiones = context.Regiones.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            return PartialView("GestionOrganismos", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOrganismo(string Nombre, int provinciaId, int localidadId, int regionId)
        {

            List<OrganismoSP> lista = new List<OrganismoSP>();

            if (!String.IsNullOrEmpty(Nombre))
            {
                var ocs = new OrganismoCommandService();
                if (!ocs.existeOrganismo(Nombre))
                {
                    lista = ocs.createOrganismo(Nombre, provinciaId, localidadId, regionId);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListOrganismosSP", lista);
                }
                else
                {
                    return Json("<div class='alert alert-danger text-center'>El elemento ya existe. La operación fue cancelada</div>");
                }

            }
            else {
                return Json("<div class='alert alert-danger text-center'>No se enviaron los parámetros al Servidor. La operación fue cancelada</div>");
            }
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeOrganismo(string Nombre)
        {
            var ocs = new OrganismoCommandService();
            if (ocs.existeOrganismo(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>No se puede concretar la Acción </br> El Organismo que intenta crear ya existe</div>");
            }
            else
            {
                return JavaScript("guardarNuevoOrganismo()");
            }

        }


        //[HttpPost]
        public JsonResult EditarOrganismo(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                OrganismoDto organismo = context.getOrganismos(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(organismo,JsonRequestBehavior.AllowGet);
            }

        }

       

        // para la edicion

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public ActionResult guardarOrganismoEditado(int id, string nombre, int? provinciaId, int? localidadId, int? regionId, string campoActivacion)
        {
            var ocs = new OrganismoCommandService();

            if (ocs.existeOtroOrganismoIgual(nombre, id))
            {
                return Json("No se pueden actualizar los Datos.</br>Ya existe un Organismo con el mismo Nombre.");
            }
            else
            {
                OrganismoDto organismo = new OrganismoDto();
                organismo.Id = id;
                organismo.Nombre = nombre.Trim().ToUpper();
                organismo.Provincia_Id = provinciaId;
                organismo.Localidad_Id = localidadId;
                organismo.Region_Id = regionId;
                organismo.Activo = (campoActivacion == "on");
               
                ocs.updateOrganismo(organismo);
                return Json("ACTUALIZADO");
            }
        }

        public ActionResult GetOrganismoActualizado(int id)
        {
            List<OrganismoSP> lista = new List<OrganismoSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var organismo =  context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorId @Id", new SqlParameter("@Id",id))
                   .FirstOrDefault();
               
                lista.Add(organismo);
                ViewBag.Success = "Organismo Actualizado correctamente";
            }

            return PartialView("ListOrganismosSP", lista);

        }


        //

        public async Task<ActionResult> BuscarOrganismo(string nombre, int? provinciaId, int? localidadId, int? regionId)
        {
            List<OrganismoSP> lista = new List<OrganismoSP>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId == null && regionId == null)
                {
                    lista = await context.Database
                    .SqlQuery<OrganismoSP>("GetOrganismos")
                    .ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId == null && regionId == null)
                {
                    lista = await context.Database
                    .SqlQuery<OrganismoSP>("GetOrganismosPorNombre @nombre", new SqlParameter("@nombre", nombre))
                    .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId == null && regionId != null)
                {
                    lista = await context.Database
                    .SqlQuery<OrganismoSP>("GetOrganismosPorRegiones @regionId", new SqlParameter("@regionId", regionId))
                    .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId != null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorLocalidad @localidadId", new SqlParameter("@localidadId", localidadId))
                   .ToListAsync();


                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId == null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorProvincia @provinciaId", new SqlParameter("@provinciaId", provinciaId))
                   .ToListAsync();


                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId != null && regionId != null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosWithAllParameters @nombre,@localidadId,@provinciaId,@regionId", new SqlParameter("@nombre", nombre), new SqlParameter("@localidadId", localidadId), new SqlParameter("@provinciaId", provinciaId), new SqlParameter("@regionId", regionId))
                   .ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId != null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorNombreyLocalidad @nombre,@localidadId", new SqlParameter("@nombre", nombre), new SqlParameter("@localidadId", localidadId))
                   .ToListAsync();

                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId == null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorNombreyProvincia @nombre,@provinciaId", new SqlParameter("@nombre", nombre), new SqlParameter("@provinciaId", provinciaId))
                   .ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId == null && regionId != null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorNombreyRegion @nombre,@regionId", new SqlParameter("@nombre", nombre), new SqlParameter("@regionId", regionId))
                   .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId != null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorLocalidadyProvincia @localidadId,@provinciaId", new SqlParameter("@localidadId", localidadId), new SqlParameter("@provinciaId", provinciaId))
                   .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId == null && localidadId != null && regionId != null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorLocalidadyRegion @localidadId,@regionId", new SqlParameter("@localidadId", localidadId), new SqlParameter("@regionId", regionId))
                   .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId == null && regionId != null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorProvinciayRegion @provinciaId,@regionId", new SqlParameter("@provinciaId", provinciaId), new SqlParameter("@regionId", regionId))
                   .ToListAsync();
                }
                else
                if (!String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId != null && regionId == null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorNombreProvinciaylocalidad @nombre,@localidadId,@provinciaId", new SqlParameter("@nombre", nombre), new SqlParameter("@localidadId", localidadId), new SqlParameter("@provinciaId", provinciaId))
                   .ToListAsync();
                }
                else
                if (String.IsNullOrEmpty(nombre) && provinciaId != null && localidadId != null && regionId != null)
                {
                    lista = await context.Database
                   .SqlQuery<OrganismoSP>("GetOrganismosPorProvinciaLocalidadyRegion @provinciaId,@localidadId,@regionId", new SqlParameter("@provinciaId", provinciaId), new SqlParameter("@localidadId", localidadId), new SqlParameter("@regionId", regionId))
                   .ToListAsync();
                }
            }
            lista = lista.OrderBy(x => x.Nombre).ToList();
            return PartialView("ListOrganismosSP", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElOrganismo(int organismoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                string mensajeDenuncias = "";
                string mensajeRelacionEstudio = "";
                var hayRelaciones = false;
                int cantidadRelacionesDenuncia;
                var existeDenuncia = context.Denuncias.Any(x => x.ORGANISMO_ID == organismoId);
                if (existeDenuncia)
                {
                    hayRelaciones = true;
                    cantidadRelacionesDenuncia = context.Denuncias.Where(x => x.ORGANISMO_ID == organismoId).Count();
                    mensajeDenuncias = "Existen Denuncias Relacionadas al Organismo. Total: " + cantidadRelacionesDenuncia + "</br>";
                   
                }
                int cantidadRelacionesEstudios;
                var organismoActual = context.Organismos.Include(o => o.Estudios).ToList().Where(o => o.Id == organismoId).FirstOrDefault();
                cantidadRelacionesEstudios = organismoActual.Estudios.Count();
 
                if (cantidadRelacionesEstudios > 0)
                {
                    hayRelaciones = true;
                    mensajeRelacionEstudio = "Existen estudios Relacionados al Organismo. Total: " + cantidadRelacionesEstudios + "</br>";
                }

                if (hayRelaciones)
                {
                    return Json("<div class='alert alert-danger text-center'>" + mensajeDenuncias + mensajeRelacionEstudio+ "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");

                }
                else
                {
                    return JavaScript("EliminarOrganismo()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarOrganismo(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existeDenuncia = context.Denuncias.Any(x => x.ORGANISMO_ID == id);
                var organismoActual = context.Organismos.Include(o => o.Estudios).ToList().Where(o => o.Id == id).FirstOrDefault();
                int? cantidadRelacionesEstudios = organismoActual.Estudios.Count();

                if (existeDenuncia || (cantidadRelacionesEstudios > 0))
                {
                    return Json("El item seleccionado está relacionado a otros elementos del Sistema. Para poder eliminar debe desvincularlo");
                }
                else {
                    OrganismoDto organismo = context.getOrganismos(true)
                                                                  .Where(t => t.Id == id)
                                                                  .FirstOrDefault();
                    context.Remove(organismo);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO", "Se ha Eliminado el Organismo " + organismo.Nombre, "EXISTENTE", "ELIMINADO", usuario, organismo.Id);
                    context.Add(accion);
                    context.SaveChanges();
                    return Json("Registro eliminado con éxito");

                }
                
            }

        }


        

        public ActionResult GestionarDenunciantes()
        {
            return PartialView("GestionDenunciantes");
        }

        /********************************/
        /***    Responsables    ***/
        /********************************/
        
        [HttpPost]
        public JsonResult GetEstudios()
        {

            var estudios = new List<EstudioDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                estudios = context.Estudios.OrderBy(x => x.Nombre).ToList();
            }
            return Json(estudios);


        }



        public JsonResult EditarResponsable(int id)
        {
            
                using (NuevoDbContext context = new NuevoDbContext())
                {

                    ResponsableDto responsable = context.getResponsables(true)
                                                  .Where(t => t.Id == id)
                                                  .FirstOrDefault();
                    responsable.Rol.Trim();
                    return Json(responsable, JsonRequestBehavior.AllowGet);
                }
   
        }



        public ActionResult GestionarResponsables()
        {
            var model = new ResponsablesModelView();
            
            return PartialView("GestionResponsables", model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GuardarResponsableEditado(ResponsableDto responsableEditado)
        //{

        //    var rcs = new ResponsableCommandService();
        //    var listResponsables = rcs.updateResponsable(responsableEditado);
            

        //    return PartialView("ListResponsables", listResponsables);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarResponsableEditado(ResponsableDto responsableEditado)
        {
            if (ModelState.IsValid)
            {
                var rcs = new ResponsableCommandService();
                if (rcs.existeOtroResponsableIgual(responsableEditado))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otro Responsable con el mismo Usuario.");
                }
                else
                {
                    rcs.updateResponsable(responsableEditado);
                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetResponsableActualizado(int id)
        {
            List<ResponsableDto> lista = new List<ResponsableDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var responsable = context.Responsables.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(responsable);
                ViewBag.Success = "Responsable Actualizado correctamente";
            }

            return PartialView("ListResponsables", lista);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearResponsable(string Nombre, string Apellido,string Usuario,string TipoResponsable,string Rol,int? Estudio_Id) {
            List<ResponsableDto> lista = new List<ResponsableDto>();
            var rcs = new ResponsableCommandService();

            if (!String.IsNullOrEmpty(Nombre) && !String.IsNullOrEmpty(Apellido) && !String.IsNullOrEmpty(Usuario) && !String.IsNullOrEmpty(Rol))
            {
                if (!rcs.existeResponsable(Usuario))
                {
                    ResponsableDto responsable = new ResponsableDto();
                    responsable.Nombre = Nombre.Trim();
                    responsable.Apellido = Apellido.Trim(); ;
                    responsable.UmeId = Usuario.ToLower();
                    responsable.TipoResponsable = TipoResponsable;
                    responsable.Rol = Rol;
                    responsable.Estudio_Id = Estudio_Id;
                    lista = rcs.createResponsable(responsable);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListResponsables", lista);
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>El usuario ingresado ya pertenece a otro responsable</div>");
                }
               
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados son inválidos</div>");
            }
            

            //ResponsableDto responsable = new ResponsableDto();
            //responsable.Nombre = Nombre.ToUpper();
            //responsable.Apellido = Apellido.ToUpper();
            //responsable.UmeId = Usuario.ToLower();
            //responsable.TipoResponsable = TipoResponsable;
            //responsable.Rol = Rol;
            //responsable.Estudio_Id = Estudio_Id;
            //var rcs = new ResponsableCommandService();
            //var listResponsables = rcs.createResponsable(responsable);


            //return PartialView("ListResponsables", listResponsables);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult existeResponsable(string usuario)
        {
            var rcs = new ResponsableCommandService();
            if (rcs.existeResponsable(usuario))
            {
                return Json("<div class='alert alert-danger text-center'>El usuario ingresado ya pertenece a otro Responsable</div>");
            }
            else
            {
                return JavaScript("guardarNuevoResponsable()");
            }

        }

        [HttpPost]
        public async Task<ActionResult> BuscarResponsables(string Nombre, string Apellido, string Usuario, string TipoResponsable) {
            var responsables = new List<ResponsableDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.ToListAsync();

                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre)
                                                                    && x.Apellido.Contains(Apellido)
                                                                    && x.UmeId.Contains(Usuario)
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Apellido.Contains(Apellido)
                                                                    && x.UmeId.Contains(Usuario)
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario)                                                                  
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario)
                                                                    && x.Nombre.Contains(Nombre)
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.TipoResponsable == TipoResponsable)                                                                    
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre))
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre)
                                                                    && x.UmeId.Contains(Usuario))
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre)
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Apellido.Contains(Apellido))
                                                                   .ToListAsync();
                }
                else
                if(!string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre)
                                                                    && x.Apellido.Contains(Apellido))                                                                   
                                                                   .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario)
                                                                    &&  x.Apellido.Contains(Apellido))
                                                                    .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.TipoResponsable == TipoResponsable
                                                                      && x.Apellido.Contains(Apellido))
                                                                      .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario)
                                                                      && x.TipoResponsable == TipoResponsable)
                                                                      .ToListAsync();
                }
                else
                if (string.IsNullOrEmpty(Nombre)
                    && string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario))
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && !string.IsNullOrEmpty(Usuario)
                    && string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.UmeId.Contains(Usuario)
                                                                    && x.Apellido.Contains(Apellido)
                                                                    && x.Nombre.Contains(Nombre))                                                                   
                                                                   .ToListAsync();
                }
                else
                if (!string.IsNullOrEmpty(Nombre)
                    && !string.IsNullOrEmpty(Apellido)
                    && string.IsNullOrEmpty(Usuario)
                    && !string.IsNullOrEmpty(TipoResponsable))
                {
                    responsables = await context.Responsables.Where(x => x.Nombre.Contains(Nombre)
                                                                    && x.Apellido.Contains(Apellido)
                                                                    && x.TipoResponsable == TipoResponsable)
                                                                   .ToListAsync();
                }
                responsables = responsables.OrderBy(x => x.Apellido).ToList();
                return PartialView("ListResponsables",responsables);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesResponsable(int responsableId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                string mensajeDenuncia = "";
                string mensajeEventos = "";
                var hayRelaciones = false;
                int cantidadRelacionesDenuncia;
                var existeDenuncia = context.Denuncias.Any(x => x.RESP_INT_ID == responsableId);
                if (existeDenuncia)
                {
                    hayRelaciones = true;
                    cantidadRelacionesDenuncia = context.Denuncias.Where(x => x.RESP_INT_ID == responsableId).Count();
                    mensajeDenuncia = "Existen Denuncias Relacionadas al Responsable. Total: " + cantidadRelacionesDenuncia + "</br>";
                    
                }
                int cantidadRelacionesEvento;
                var existeEvento = context.EventoDtoes.Any(x => x.ResIntId == responsableId);
                if (existeEvento)
                {
                    hayRelaciones = true;
                    cantidadRelacionesEvento = context.EventoDtoes.Where(x => x.ResIntId == responsableId).Count();
                    mensajeEventos = "Existen Eventos Relacionados al Responsable. Total: " + cantidadRelacionesEvento + "</br>";

                }

                if (hayRelaciones)
                {
                    return Json("<div class='alert alert-danger text-center'>" + mensajeDenuncia + mensajeEventos + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");

                }
                else
                {
                    return JavaScript("EliminarResponsable()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarResponsable(int responsableId)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existeEnDenuncia = context.Denuncias.Any(x => x.RESP_INT_ID == responsableId);
                var existeEnEvento = context.EventoDtoes.Any(x => x.ResIntId == responsableId);
                if (!existeEnDenuncia && !existeEnEvento)
                {

                    ResponsableDto responsable = context.getResponsables(true)
                                              .Where(t => t.Id == responsableId)
                                              .FirstOrDefault();
                    context.Remove(responsable);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "RESPONSABLE", "Se ha Eliminado al Responsable: "+ responsable.Nombre + ','+responsable.Apellido,"EXISTENTE", "ELIMINADO", usuario, responsable.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>El Responsable está relacionado a otros elementos del Sistema. No se puede eliminar</div>");
                }
                
            }

        }

        /********************************/
        /***    Domicilio Mediador    ***/
        /********************************/

        public ActionResult GestionarDomiciliosMediadores()
        {
            var model = new DomicilioMediadorModelView();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var mediadores = context.Mediadores.ToList();
                model.Mediadores = mediadores.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
                
            }
                return PartialView("GestionDomiciliosMediadores",model);
        }



        public ActionResult BuscarDomicilioMediador(string nombre, int? mediadorId)
        {
            List<DomicilioMediadorSP> lista = new List<DomicilioMediadorSP>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (mediadorId == null && string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomiciliosMediadores")
                        .OrderBy(x=>x.Domicilio)
                        .ToList();
                }
                else
                if (mediadorId != null && string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomiciliosPorMediador @mediadorId", new SqlParameter("@mediadorId", mediadorId))
                        .ToList();
                }
                else
                if (mediadorId == null && !string.IsNullOrEmpty(nombre))
                {

                    lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomiciliosPorNombre @nombre", new SqlParameter("@nombre", nombre))
                        .ToList();
                }
                else
                if (mediadorId != null && !string.IsNullOrEmpty(nombre))
                {
                    lista = context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomiciliosPorNombreyMediador @nombre,@mediadorId", new SqlParameter("@nombre", nombre), new SqlParameter("@mediadorId", mediadorId))
                        .ToList();
                }
            }
            return PartialView("ListDomiciliosMediadoresSP", lista);
        }


       

        public async Task<ActionResult> ListarDomiciliosMediadores(string filtro)
        {
            List<DomicilioMediadorDto> lista = new List<DomicilioMediadorDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.DomiciliosMediadores.OrderByDescending(p => p.Id).ToListAsync();
            }
            return PartialView("ListDomiciliosMediadores", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearDomicilioMediador(string domicilio, int mediadorId)
        {
            List<DomicilioMediadorSP> lista = new List<DomicilioMediadorSP>();
            var dcs = new DomicilioCommandService();

            if (!String.IsNullOrEmpty(domicilio))
            {
                if (!dcs.existeDomicilioMediador(domicilio, mediadorId))
                {
                    var domicilioMediador = new DomicilioMediadorDto();
                    domicilioMediador.Domicilio = domicilio.ToUpper();
                    domicilioMediador.MediadorId = mediadorId;
                    domicilioMediador.Activo = true;
                    lista = dcs.createDomicilioMediador(domicilioMediador);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListDomiciliosMediadoresSP", lista);
                }
                else
                {
                    return Json("<div class='alert alert-danger text-center'>No se pudo concretar la acción. El Domicilio Ya existe</div>");
                }
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados son inválidos</div>");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteDomicilioMediador(string Nombre, int mediadorId)
        {
            var dcs = new DomicilioCommandService();
            if (dcs.existeDomicilioMediador(Nombre, mediadorId))
            {
                return Json("<div class='alert alert-danger text-center'>El Mediador ya tiene asignado el Domicilio.</br>La operación fue cancelada</div>");
            }
            else
            {
                return JavaScript("guardarNuevoDomicilioMediador()");
            }

        }


        //[HttpPost]
        public JsonResult EditarDomicilioMediador(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                DomicilioMediadorDto estudio = context.getDomiciliosMediadores(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(estudio, JsonRequestBehavior.AllowGet);
            }

        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarDomicilioMediadorEditado(int id, string nombre, int mediadorSeleccionado, string campoActivacion)
        {
            var dmcs = new DomicilioCommandService();

            if (!String.IsNullOrEmpty(nombre))
            {
                if (dmcs.existeOtroDomicilioIgual(nombre, id, mediadorSeleccionado))
                {
                    return Json("El Mediador ya tiene asignado el Domicilio</br>Se cancela la Edición.");
                }
                else
                {
                    DomicilioMediadorDto domicilio = new DomicilioMediadorDto();
                    domicilio.Id = id;
                    domicilio.Domicilio = nombre.Trim().ToUpper();
                    domicilio.MediadorId = mediadorSeleccionado;
                    domicilio.Activo = (campoActivacion == "on");
                    dmcs.updateDomicilioMediador(domicilio);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetDomicilioMediadorActualizado(int id)
        {
            List<DomicilioMediadorSP> lista = new List<DomicilioMediadorSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var domicilio =context.Database
                        .SqlQuery<DomicilioMediadorSP>("GetDomicilioMediadorPorId @Id", new SqlParameter("@Id",id))
                        .FirstOrDefault();
                lista.Add(domicilio);
                ViewBag.Success = "Domicilio Actualizado correctamente";
            }

            return PartialView("ListDomiciliosMediadoresSP", lista);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElDomicilio(int domicilioId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.Denuncias.Any(x => x.domicilioMediadorId == domicilioId);
                if (existe)
                {
                    cantidadRelaciones = context.Denuncias.Where(x => x.domicilioMediadorId == domicilioId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Denuncias Relacionadas al Domicilio. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("EliminarDomicilioMediador()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarDomicilioMediador(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existe = context.Denuncias.Any(x => x.domicilioMediadorId == id);
                if (!existe)
                {
                    DomicilioMediadorDto domicilio = context.getDomiciliosMediadores(true)
                                                                  .Where(t => t.Id == id)
                                                                  .FirstOrDefault();
                    context.Remove(domicilio);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "DOMICILIO MEDIADOR", "Se ha Eliminado el Domicilio: " + domicilio.Domicilio + " del Mediador con Id: " + domicilio.MediadorId,"EXISTENTE","ELIMINADA", usuario, domicilio.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");

                }
                else {
                    return Json("El elemento posee las Relaciones.No se puede eliminar");
                }

                
            }

        }
        /********************************/
        /***        Mediador          ***/
        /********************************/


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElMediador(int mediadorId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelacionesDomicilio;
                string mensajeDomicilios = "";
                string mensajeDenuncias = "";
                var existeRelaciones = false;
                var existeDomicilios = context.DomiciliosMediadores.Any(x => x.MediadorId == mediadorId);
                if (existeDomicilios)
                {
                    cantidadRelacionesDomicilio = context.DomiciliosMediadores.Where(x => x.MediadorId == mediadorId).Count();
                    existeRelaciones = true;
                    mensajeDomicilios = "Existen Domicilios Relacionados al Mediador. Total: " + cantidadRelacionesDomicilio + "</br>";
                    //return Json("<div class='alert alert-danger text-center'>Existen Domicilios Relacionadas al Mediador. Total : " + cantidadRelacionesDomicilio + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                int cantidadRelacionesDenuncia;
                var existeDenuncias = context.Denuncias.Any(x => x.mediadorId == mediadorId);
                if (existeDenuncias)
                {
                    cantidadRelacionesDenuncia = context.Denuncias.Where(x => x.mediadorId == mediadorId).Count();
                    existeRelaciones = true;
                    mensajeDenuncias = "Existen Denuncias Relacionadas al Mediador. Total: " + cantidadRelacionesDenuncia + "</br>";
                    //return Json("<div class='alert alert-danger text-center'>Existen Domicilios Relacionadas al Mediador. Total : " + cantidadRelacionesDomicilio + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                if (existeRelaciones) {
                    return Json("<div class='alert alert-danger text-center'>" + mensajeDomicilios + mensajeDenuncias + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("EliminarMediador()");
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMediador(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            


                using (NuevoDbContext context = new NuevoDbContext())
            {

                var existeDomiciliosRelacionados = context.DomiciliosMediadores.Any(x => x.MediadorId == id);
                var existeDenunciasRelacionadas = context.Denuncias.Any(x => x.mediadorId == id);

                if (!existeDomiciliosRelacionados && !existeDenunciasRelacionadas)
                {

                    MediadorDto mediador = context.getMediadores(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                    context.Remove(mediador);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "MEDIADOR", "Se ha Eliminado el Mediador: " + mediador.Nombre,"EXISTENTE","ELIMINADO", usuario, mediador.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");
                }
                else
                {
                    return Json("<div class='alert alert-danger text-center'>No es posible eliminar</br>Hay elementos vinculados a este Objeto.</div>");

                }
            }

        }

       
        /********************************/
        /***        Mediadores        ***/
        /********************************/

        public ActionResult GestionarMediadores()
        {
            return PartialView("GestionMediadores");
        }

        public async Task<ActionResult> BuscarMediador(string filtro)
        {
            List<MediadorDto> lista = new List<MediadorDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro)) {
                    lista = await context.Mediadores.OrderBy(x=>x.Nombre).ToListAsync();
                }
                else {
                    lista = await context.Mediadores.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
                
            }
            return PartialView("ListMediadores", lista);
        }

        public async Task<ActionResult> ListarMediadores(string filtro)
        {
            List<MediadorDto> lista = new List<MediadorDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Mediadores.OrderBy(p => p.Nombre).ToListAsync();

            }
            return PartialView("ListMediadores", lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMediador(string Nombre)
        {
            List<MediadorDto> lista = new List<MediadorDto>();
            var mcs = new MediadorCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!mcs.existeMediador(Nombre))
                {
                    var mediador = mcs.createMediador(Nombre);
                    lista.Add(mediador);
                    ViewBag.Success = "Registro guardado correctamente";

                    return PartialView("ListMediadores", lista);
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>No se pudo concretar la acción. El Mediador ya existe</div>");
                }
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los datos enviados son inválidos</div>");
            }
            
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteMediador(string Nombre)
        {
            var mcs = new MediadorCommandService();
            if (mcs.existeMediador(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe un Mediador con el mismo Nombre</div>");
            }
            else
            {
                return JavaScript("guardarNuevoMediador()");
            }

        }


        //[HttpPost]
        public JsonResult EditarMediador(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                MediadorDto estudio = context.getMediadores(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(estudio, JsonRequestBehavior.AllowGet);
            }

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult guardarMediadorEditado(int id, string Nombre, string campoActivacion)
        //{
        //    MediadorDto mediador = new MediadorDto();
        //    mediador.Id = id;
        //    mediador.Nombre = Nombre.ToUpper();
        //    mediador.Activo = (campoActivacion == "on");
        //    List<MediadorDto> lista = new List<MediadorDto>();
        //    var mcs = new MediadorCommandService();
        //    var mediadorActualizado = mcs.updateMediador(mediador);
        //    lista.Add(mediadorActualizado);

        //    return PartialView("ListMediadores", lista);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarMediadorEditado(int id, string Nombre, string campoActivacion)
        {
            var mcs = new MediadorCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (mcs.existeOtroMediadorIgual(Nombre, id))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otro Mediador con el mismo Nombre.");
                }
                else
                {
                    MediadorDto mediador = new MediadorDto();
                    mediador.Id = id;
                    mediador.Nombre = Nombre.Trim().ToUpper();
                    mediador.Activo = (campoActivacion == "on");
                    mcs.updateMediador(mediador);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetMediadorActualizado(int id)
        {
            List<MediadorDto> lista = new List<MediadorDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var mediador = context.Mediadores.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(mediador);
                ViewBag.Success = "Mediador Actualizado correctamente";
            }

            return PartialView("ListMediadores", lista);

        }


        /********************************
         * Tipo de Evento
         * 
        ********************************/

        public ActionResult GestionarTiposDeEventos()
        {
            return PartialView("GestionTipoEventos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTipoDeEvento(string Nombre)
        {
            List<TipoEventoDto> lista = new List<TipoEventoDto>();
            var tecs = new TipoEventoCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (!tecs.existeTipoEvento(Nombre))
                {
                    var tipoEvento = tecs.createTipoEvento(Nombre);
                    lista.Add(tipoEvento);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListTiposDeEvento", lista);
                }
                else {
                    return Json("<div class='alert alert-danger text-center'>Ya existe un elemento con el mismo Nombre</div>");
                }
            }
            else {
                return Json("<div class='alert alert-danger text-center'>Los parámetros enviados son inválidos</div>");
            }
            
            //List<TipoEventoDto> lista = new List<TipoEventoDto>();
            //var tecs = new TipoEventoCommandService();
            //var tipoEvento = tecs.createTipoEvento(Nombre);
            //lista.Add(tipoEvento);

            //return PartialView("ListTiposDeEvento", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteTipoDeEvento(string Nombre)
        {
            var tecs = new TipoEventoCommandService();
            if (tecs.existeTipoEvento(Nombre))
            {
                return Json("<div class='alert alert-danger text-center'>Ya existe un Tipo de Evento con el mismo Nombre</div>");
            }
            else
            {
                return JavaScript("guardarNuevoTipoEvento()");
            }

        }

        public async Task<ActionResult> ListarTiposDeEvento()
        {
            List<TipoEventoDto> lista = new List<TipoEventoDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.TiposDeEventos.OrderBy(x=>x.Nombre).ToListAsync();                              
            }
            return PartialView("ListTiposDeEvento", lista);
        }


        public async Task<ActionResult> BuscarTiposDeEvento(string filtro)
        {
            List<TipoEventoDto> lista = new List<TipoEventoDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (String.IsNullOrEmpty(filtro))
                {
                    lista = await context.TiposDeEventos.OrderBy(x=>x.Nombre).ToListAsync();
                }
                else
                {
                    lista = await context.TiposDeEventos.Where(m => m.Nombre.Contains(filtro)).ToListAsync();
                }
            }
            return PartialView("ListTiposDeEvento", lista);
        }


        //[HttpPost]
        public JsonResult EditarTipoDeEvento(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                TipoEventoDto tipoEvento = context.getTiposDeEventos(true)
                                              .Where(t => t.Id == id)
                                              .FirstOrDefault();
                return Json(tipoEvento, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElTipoDeEvento(int tipoEventoId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                int cantidadRelaciones;
                var existe = context.EventoDtoes.Any(x => x.TipoEventoId == tipoEventoId);
                if (existe)
                {
                    cantidadRelaciones = context.EventoDtoes.Where(x => x.TipoEventoId == tipoEventoId).Count();
                    return Json("<div class='alert alert-danger text-center'>Existen Eventos Relacionados al Tipo de Evento. Total : " + cantidadRelaciones + "</br>Los elementos a eliminar deben estar desvinculados.</br>No se puede eliminar el elemento seleccionado</br>La operación fue cancelada.</div>");
                }
                else
                {
                    return JavaScript("eliminarTipoDeEvento()"); 
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarTipoDeEvento(int tipoEventoId)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existe = context.EventoDtoes.Any(x => x.TipoEventoId == tipoEventoId);
                if (!existe)
                {
                    TipoEventoDto tipoEvento = context.getTiposDeEventos(true)
                                                  .Where(t => t.Id == tipoEventoId)
                                                  .FirstOrDefault();
                    context.Remove(tipoEvento);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "TIPO DE EVENTO", "Se ha Eliminado el Tipo de Evento : " + tipoEvento.Nombre,"EXISTENTE","ELIMINADO", usuario, tipoEvento.Id);
                    context.Add(accion);
                    context.SaveChanges();

                    //return JavaScript("eliminarDeLaLista()");
                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("Este elemento se encuentra relacionado a otros objetos del sistema. No es posible eliminar");
                }
            }

        }

       

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult guardarTipoDeEventoEditado(int id, string Nombre, string campoActivacion)
        //{
        //    TipoEventoDto tipoEvento = new TipoEventoDto();
        //    tipoEvento.Id = id;
        //    tipoEvento.Nombre = Nombre.ToUpper();
        //    tipoEvento.Deleted = (campoActivacion == "on");
        //    List<TipoEventoDto> lista = new List<TipoEventoDto>();
        //    var tecs = new TipoEventoCommandService();
        //    var tipoEventoActualizado = tecs.updateTipoEvento(tipoEvento);
        //    lista.Add(tipoEventoActualizado);

        //    return PartialView("ListTiposDeEvento", lista);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarTipoDeEventoEditado(int id, string Nombre, string campoActivacion)
        {
            var tecs = new TipoEventoCommandService();

            if (!String.IsNullOrEmpty(Nombre))
            {
                if (tecs.existeOtroTipoEventoIgual(Nombre, id))
                {
                    return Json("No se pueden actualizar los datos.</br>Existe otro Tipo de Evento con el mismo Nombre.");
                }
                else
                {
                    TipoEventoDto tipoEvento = new TipoEventoDto();
                    tipoEvento.Id = id;
                    tipoEvento.Nombre = Nombre.ToUpper();
                    tipoEvento.Deleted = (campoActivacion == "on");
                    tecs.updateTipoEvento(tipoEvento);

                    return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }

        }

        public ActionResult GetTipoEventoActualizado(int id)
        {
            List<TipoEventoDto> lista = new List<TipoEventoDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var tipoEvento = context.TiposDeEventos.Where(e => e.Id == id).FirstOrDefault();
                lista.Add(tipoEvento);
                ViewBag.Success = "Tipo de Evento Actualizado correctamente";
            }

            return PartialView("ListTiposDeEvento", lista);

        }


        /********************************
        *           Denunciante 
        *******************************/
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<ActionResult> CrearDenunciante(string nombre, string apellido,int? dni)
        //{
        //    List<DenuncianteDto> lista = new List<DenuncianteDto>();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        var denunciante = new DenuncianteDto();
        //        denunciante.nombre = nombre.ToUpper();
        //        denunciante.apellido = apellido.ToUpper();
        //        denunciante.NroDocumento = dni.ToString();
        //        denunciante.Deleted = true;
        //        context.Add(denunciante);
        //        context.SaveChanges();
        //        var id = denunciante.DenuncianteId;
        //        denunciante = await context.Denunciantes.Where(e => e.DenuncianteId == id).FirstOrDefaultAsync();
        //        lista.Add(denunciante);
        //    }
        //    return PartialView("ListDenunciantes", lista);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearDenunciante(string nombre, string apellido, string dni)
        {
            //List<TipoEventoDto> lista = new List<TipoEventoDto>();
            //var tecs = new TipoEventoCommandService();

            //if (!String.IsNullOrEmpty(Nombre))
            //{
            //    if (!tecs.existeTipoEvento(Nombre))
            //    {
            //        var tipoEvento = tecs.createTipoEvento(Nombre);
            //        lista.Add(tipoEvento);
            //        ViewBag.Success = "Registro guardado correctamente";
            //        return PartialView("ListTiposDeEvento", lista);
            //    }
            //    else
            //    {
            //        return Json("<div class='alert alert-danger text-center'>Ya existe un elemento con el mismo Nombre</div>");
            //    }
            //}
            //else
            //{
            //    return Json("<div class='alert alert-danger text-center'>Los parámetros enviados son inválidos</div>");
            //}

            var dcs = new DenuncianteCommandService();
            List<DenuncianteDto> lista = new List<DenuncianteDto>();
            if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido))
            {
                if (!String.IsNullOrEmpty(dni))
                {
                    int result;
                    int.TryParse(dni, out result);
                    if (result < 1)
                    {
                        return Json("El Nro de Documento es inválido.</br> Solo se permiten números");
                    }
                    if (dcs.existeDni(dni))
                    {
                        return Json("No se puede crear al Denunciante.</br>Ya existe otro Denunciante con el mismo Nro de Documento.");
                    }
                    else
                    {
                        DenuncianteDto denunciante = new DenuncianteDto();
                        denunciante.nombre = nombre.Trim().ToUpper();
                        denunciante.apellido = apellido.Trim().ToUpper();
                        denunciante.NroDocumento = dni.Trim();
                        denunciante.Deleted = false;
                        var den = dcs.crearDenunciante(denunciante);
                        lista.Add(den);
                        ViewBag.Success = "Registro guardado correctamente";
                        return PartialView("ListDenunciantes", lista);
                        //return Json("ACTUALIZADO");
                    }
                }
                else
                {
                    DenuncianteDto denunciante = new DenuncianteDto();
                    denunciante.nombre = nombre.Trim().ToUpper();
                    denunciante.apellido = apellido.Trim().ToUpper();
                    //denunciante.NroDocumento = dni.Trim();
                    denunciante.Deleted = false;
                    var den = dcs.crearDenunciante(denunciante);
                    lista.Add(den);
                    ViewBag.Success = "Registro guardado correctamente";
                    return PartialView("ListDenunciantes", lista);
                    //return Json("ACTUALIZADO");
                }
            }
            else
            {
                return Json("Los parámetros enviados son inválidos");
            }
        }

            public async Task<ActionResult> ListarDenunciantes()
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = await context.Denunciantes.OrderBy(x=>x.apellido).ToListAsync();
            }
            return PartialView("ListDenunciantes", lista);
        }

        [HttpPost]
        public async Task<ActionResult> BuscarDenunciante(string nombre, string apellido,string dni,int CurrentPageIndex)
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();
            
            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(dni))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.NroDocumento)
                                                               && !String.IsNullOrEmpty(m.apellido)
                                                               && !String.IsNullOrEmpty(m.nombre))
                                                        .Where(m => m.NroDocumento.Contains(dni)
                                                        && m.apellido.Contains(apellido)
                                                        && m.nombre.Contains(nombre)).ToListAsync();

                    
                }
                else
                   if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.apellido)
                                                            && !String.IsNullOrEmpty(m.nombre))
                                                            .Where(m => m.apellido.Contains(apellido)
                                                            && m.nombre.Contains(nombre)).ToListAsync();

                   
                }
                else
                   if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(dni))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.nombre)
                                                               && !String.IsNullOrEmpty(m.NroDocumento))
                                                            .Where(m => m.nombre.Contains(nombre)
                                                               && m.NroDocumento.Contains(dni)).ToListAsync();

                   
                }
                else
                   if (!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(dni))
                {

                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.apellido)
                                                               && !String.IsNullOrEmpty(m.NroDocumento))
                                                               .Where(m => m.apellido.Contains(apellido)
                                                               && m.NroDocumento.Contains(dni)).ToListAsync();
                   
                }
                else
                   if (!String.IsNullOrEmpty(nombre))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.nombre))
                                                      .Where(m => m.nombre.Contains(nombre)).ToListAsync();

                    
                }
                else
                   if (!String.IsNullOrEmpty(apellido))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.apellido))
                                                      .Where(m => m.apellido.Contains(apellido)).ToListAsync();

                    
                }
                else
                   if (!String.IsNullOrEmpty(dni))
                {
                    lista = await context.Denunciantes.Where(m => !String.IsNullOrEmpty(m.NroDocumento))
                                                      .Where(m => m.NroDocumento.Contains(dni)).ToListAsync();


                }
                else {
                    lista = await context.Denunciantes.ToListAsync();
                }

            }
            lista = lista.OrderBy(x=>x.apellido).ToList();

            var totalDeReg = lista.Count();
            var cantidadRegPorPagina = 5;
            var cantidadPag = (int)Math.Ceiling((double)totalDeReg / cantidadRegPorPagina);
            //lista = lista.OrderBy(x => x.apellido)
            //         .Skip((CurrentPageIndex - 1) * cantidadRegPorPagina)
            //        .Take(cantidadRegPorPagina).ToList();
            
            var model = new PaginadorDenunciantes();
            model.denunciantes = lista;
            model.PaginaActual = CurrentPageIndex;
            model.TotalDeRegistros = totalDeReg;
            model.RegistrosPorPagina = cantidadRegPorPagina;
            model.TotalDePaginas = cantidadPag;

            return PartialView("ListDenunciantes",model);
        }

        

        //[HttpPost]
        public JsonResult EditarDenunciante(int id)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {

                DenuncianteDto tipoEvento = context.getDenunciantes(true)
                                              .Where(t => t.DenuncianteId == id)
                                              .FirstOrDefault();
                return Json(tipoEvento, JsonRequestBehavior.AllowGet);
            }

        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardarDenuncianteEditado(int id, string nombre, string apellido,string dni, string campoActivacion)
        {
            var dcs = new DenuncianteCommandService();

            if (!String.IsNullOrEmpty(nombre)&& !String.IsNullOrEmpty(apellido))
            {
                if (!String.IsNullOrEmpty(dni)) {
                    int result;
                    int.TryParse(dni,out result);
                    if (result < 1) {
                        return Json("El Nro de Documento es inválido");
                    }
                    if (dcs.existeOtroDenuncianteIgual(id, dni))
                    {
                        //return Json("No se pueden actualizar los datos.</br>Ya existe otro Denunciante con el mismo Nro de Documento.");
                        return JavaScript("<script>toastr.warning('No se pueden actualizar los datos.</br>Ya existe otro Denunciante con el mismo Nro de Documento.')</script>");
                    }
                    else {
                        DenuncianteDto denunciante = new DenuncianteDto();
                        denunciante.DenuncianteId = id;
                        denunciante.nombre = nombre.Trim().ToUpper();
                        denunciante.apellido = apellido.Trim().ToUpper();
                        denunciante.NroDocumento = dni.Trim();
                        denunciante.Deleted = (campoActivacion == "on");
                        dcs.updateDenunciante(denunciante);
                        
                         return JavaScript("GetDenuncianteActualizado()");
                        //return Json("ACTUALIZADO");
                    }
                }             
                else
                {
                    DenuncianteDto denunciante = new DenuncianteDto();
                    denunciante.DenuncianteId = id;
                    denunciante.nombre = nombre.Trim().ToUpper();
                    denunciante.apellido = apellido.Trim().ToUpper();
                    //denunciante.NroDocumento = dni.Trim();
                    denunciante.Deleted = (campoActivacion == "on");
                    dcs.updateDenunciante(denunciante);

                    return JavaScript("GetDenuncianteActualizado()");
                    //return Json("ACTUALIZADO");
                }
            }
            else
            {
                //return Json("Los parámetros enviados son inválidos");
                return JavaScript("<script>toastr.warning('Los parámetros enviados son inválidos')</script>");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExisteDni(string dni) {
            if (!String.IsNullOrEmpty(dni))
            {
                DenuncianteCommandService dcs = new DenuncianteCommandService();
                if (dcs.existeDni(dni))
                {
                    //return Json("Ya existe un Denunciante con el mismo Nro de Documento.");
                    return Json("<div class='alert alert-danger text-center'>Ya existe un Denunciante con el mismo Nro de Documento.</div>");
                }
                else {
                    return JavaScript("GuardarNuevoDenunciante()");
                }
            }
            else {
                return JavaScript("GuardarNuevoDenunciante()");
            }
        }

        public ActionResult GetDenuncianteActualizado(int id)
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var denunciante = context.Denunciantes.Where(e => e.DenuncianteId == id).FirstOrDefault();
                lista.Add(denunciante);
                ViewBag.Success = "Denunciante Actualizado correctamente";
            }

            return PartialView("ListDenunciantes", lista);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarDenunciante(int id)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var existeEnDenuncias = context.Denuncias.Any(x => x.DENUNCIANTE_ID == id);
                int cantidadEnGrupo = 0;
                cantidadEnGrupo = context.Database.SqlQuery<int>("ExisteRelacionDenuncianteGrupoDenunciantes @denuncianteId", new SqlParameter("@denuncianteId",id)).FirstOrDefault();

                if (!existeEnDenuncias && cantidadEnGrupo < 1)
                {
                    DenuncianteDto denunciante = context.getDenunciantes(true)
                                             .Where(t => t.DenuncianteId == id)
                                             .FirstOrDefault();
                    context.Remove(denunciante);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIANTE", "Se ha Eliminado al Denunciante : " + denunciante.nombre + ',' + denunciante.apellido, "EXISTENTE", "ELIMINADO", usuario, denunciante.DenuncianteId);
                    context.Add(accion);
                    context.SaveChanges();

                    return Json("Registro eliminado con éxito");
                }
                else {
                    return Json("El elemento posee relaciones. No puede ser eliminado");
                }

               
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TieneRelacionesElDenunciante(int denuncianteId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                bool hayRelaciones = false;
                string mensajeDenuncias = "";
                string mensajeGrupo = "";
                int cantidadRelaciones;
                var existe = context.Denuncias.Any(x => x.DENUNCIANTE_ID == denuncianteId);
                if (existe)
                {
                    hayRelaciones = true;
                    cantidadRelaciones = context.Denuncias.Where(x => x.DENUNCIANTE_ID == denuncianteId).Count();
                    mensajeDenuncias = "Existen Denuncias Relacionadas al Denunciante, en Total: " + cantidadRelaciones + "</br>";
                   
                }

                int cantidadEnGrupo = 0;
                cantidadEnGrupo = context.Database.SqlQuery<int>("ExisteRelacionDenuncianteGrupoDenunciantes @denuncianteId", new SqlParameter("@denuncianteId", denuncianteId)).FirstOrDefault();
                if (cantidadEnGrupo > 0) {
                    hayRelaciones = true;
                    mensajeGrupo = "Existen Registros del Denunciante en la Tabla de Grupos de Denunciante, en Total: " + cantidadEnGrupo + "</br>";
                }
                
                if (hayRelaciones)
                {
                    return Json("<div class='alert alert-danger text-center'>"+ mensajeDenuncias + mensajeGrupo + "</br>Los elementos a eliminar deben estar desvinculados </br> No se puede eliminar el elemento seleccionado</div>");                    
                }

                else
                {
                    return JavaScript("EliminarDenunciante()"); 
                    //return JavaScript("<script>toastr.success('Eliminado OK')</script>");
                }

            }


        }


        /********************************
        *       Organismos y Estudios
        *******************************/


        public ActionResult GestionarOrganismosEstudios() {
            OrganismoEstudioRelModelView model = new OrganismoEstudioRelModelView();
            NuevoDbContext context = new NuevoDbContext();
            model.Organismos = context.Organismos.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Estudios = context.Estudios.OrderBy(x => x.Nombre).Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            return PartialView("GestionOrganismosEstudios",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>CrearRelacionOrganismoEstudio(int organismoId, int estudioId)/*[Bind(Include ="Nombre")]MotivoDeBajaDto motivoBaja*/
        {
           
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;

            using (NuevoDbContext context = new NuevoDbContext())
            {

                if (existeRelacionOrganismoEstudio(organismoId, estudioId) > 0)
                {
                    return Json("<div class='alert alert-danger text-center'>La operación fue cancelada. </br> La Relación que intenta crear ya existe.</div>");
                    //return JavaScript("<script>");
                }
                else {
                    OrganismoDto organismo = context.Organismos.Include(o => o.Estudios).ToList().Where(o => o.Id == organismoId).FirstOrDefault();
                    EstudioDto estudio = context.Estudios.Where(e => e.Id == estudioId).FirstOrDefault();
                    organismo.Estudios.Add(estudio);
                    context.SaveChanges();
                    var accion = new CommonChangeLoggerDto(DateTime.Now,"ORGANISMO ESTUDIO", "Relación creada entre el Organismo: " + organismo.Nombre + " y el Estudio: " + estudio.Nombre,"","", usuario, organismo.Id);
                    context.Add(accion);
                    context.SaveChanges();


                    return JavaScript("GetNuevaRelacionOrganismoEstudio()");
                }
                   
            }
            
        }
        
        public ActionResult GetNuevaRelacionOrganismoEstudio(int organismoId, int estudioId)
        {
            List<OrganismoEstudioRelSP> lista = new List<OrganismoEstudioRelSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista =  context.Database
                           .SqlQuery<OrganismoEstudioRelSP>("GetRelPorOrganismoyEstudioId @organismoId,@estudioId", new SqlParameter("@organismoId", organismoId), new SqlParameter("@estudioId", estudioId))
                           .ToList();
                ViewBag.Success = "Registro guardado correctamente";
                
            }
            return PartialView("ListOrganismosEstudiosRel", lista);


        }



        //OrganismoEstudioRelDto relacionOE = new OrganismoEstudioRelDto();
        //estudio.Organismos.Add(organismo);
        //relacionOE.OrganismoId = organismoId;
        //relacionOE.EstudioId = estudioId;
        //context.Add(relacionOE);

        private int existeRelacionOrganismoEstudio(int organismoId, int estudioId)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                
                int cantidad = 0;
                cantidad = context.Database.SqlQuery<int>("existeRelacionOrganismoEstudio @organismoId,@estudioId", new SqlParameter("@organismoId", organismoId), new SqlParameter("@estudioId", estudioId)).FirstOrDefault();
                return cantidad;
                //if (cantidad > 0)
                //{

                //    return Json("<div class='alert alert-danger text-center'>La operación fue cancelada. </br> La Relación que intenta crear ya existe.</div>");
                //}
                //else
                //{

                //    return JavaScript("<script>toastr.success('OK')</script>");

                //}

                ////existe = context.Database.ExecuteSqlCommand("existeRelacionOrganismoEstudio @organismoId,@estudioId,@existe output", new SqlParameter("@organismoId", organismoId), new SqlParameter("@estudioId", estudioId), new SqlParameter("@existe", existe));
                ////return context..Any(x => String.Equals(x.Nombre.Trim(), unNombre.Trim()));
                //return Json(cantidad);
            }
            
           

        }

        public async Task<ActionResult> BuscarRelacionOrganismoEstudio(int? organismoId,int? estudioId)
        {
            List<OrganismoEstudioRelSP> lista = new List<OrganismoEstudioRelSP>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                if (organismoId == null && estudioId == null)
                {
                    lista = await context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetOrganismoEstudioRel")
                        .ToListAsync();
                }
                else
               if (organismoId != null && estudioId == null)
                {
                    lista = await context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetOrganismoEstudioRelPorOrganismoId @organismoId", new SqlParameter("@organismoId", organismoId))
                        .ToListAsync();
                }
                else
                if (organismoId == null && estudioId != null)
                {

                    lista = await context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetOrganismoEstudioRelPorEstudioId @estudioId", new SqlParameter("@estudioId", estudioId))
                        .ToListAsync();
                }
                else
                if (organismoId != null && estudioId != null)
                {
                    lista = await context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetRelPorOrganismoyEstudioId @organismoId,@estudioId", new SqlParameter("@organismoId", organismoId), new SqlParameter("@estudioId", estudioId))
                        .ToListAsync();
                }
                lista = lista.OrderBy(x=>x.Organismo).ToList();
            }
            return PartialView("ListOrganismosEstudiosRel", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EliminarRelacionOrganismoEstudio(int organismoId,int estudioId)
        {
            var usuario = System.Web.HttpContext.Current.User.Identity.Name;
            using (NuevoDbContext context = new NuevoDbContext())
                {
                    OrganismoDto organismo = context.getOrganismos(true).Include(t=>t.Estudios)
                                                  .Where(t => t.Id == organismoId)
                                                  .FirstOrDefault();
                    EstudioDto estudio = context.Estudios.Where(e => e.Id == estudioId).FirstOrDefault();
                    organismo.Estudios.Remove(estudio);
                   
                    context.SaveChanges();
                    //auditoria
                    var accion = new CommonChangeLoggerDto(DateTime.Now, "ORGANISMO ESTUDIO", "Relación eliminada entre el Organismo: " + organismo.Nombre + " y el Estudio: " + estudio.Nombre, "EXISTENTE", "ELIMINADA", usuario, organismo.Id);
                    context.Add(accion);
                    context.SaveChanges();

                return Json("Registro eliminado con éxito");
                }
           
        }
    }
}

   
