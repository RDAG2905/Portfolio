
using CaptchaMvc.HtmlHelpers;
using Greco2.Excel;
using Greco2.Model;
using Greco2.Models;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Denunciante;
using Greco2.Models.Enum;
using Greco2.Models.Estado;
using Greco2.Models.Estudios;
using Greco2.Models.Evento;
using Greco2.Models.Login;
using Greco2.Models.Mediador;
using Greco2.Models.ModalidadGestión;
using Greco2.Models.MotivoDeReclamo;
using Greco2.Models.Organismo;
using Greco2.Models.OrganismosEstudiosRel;
using Greco2.Models.ReqInforme;
using Greco2.Models.Responsables;
using Greco2.Models.Servicio;
using Greco2.Models.TipoEvento;
using Greco2.Models.TipoProceso;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Greco2.Controllers
{
    public class HomeController : Controller
    {



        public ActionResult Index()
        {
            //ViewBag.usuarioLogueado = "";
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View("Login", new LoginViewModel());
        }

        public ActionResult validateUser(LoginViewModel model) {
            String rol = "";
            var usuario = new ResponsableDto();

            using (NuevoDbContext context = new NuevoDbContext()) {
                //throw new InvalidDataException();
                //usuario = context.Responsables.Where(r => r.UmeId == model.User /*&& r.Password == model.Password*/).FirstOrDefault();
                //usuario = context.Responsables.Where(r => r.UmeId == model.User && model.Password == "Greco2020").FirstOrDefault();
                if (String.Equals(model.Password, "Greco2020*"))
                {
                    usuario = context.Responsables.Where(r => r.UmeId == model.User).FirstOrDefault();
                }
                else {
                    //return JavaScript("<script>toastr.warning('Usuario o Contraseña Inválidos.')</script>");
                    //var newModel = new LoginViewModel();
                    //newModel.User = model.User;
                    //newModel.Password = model.Password;
                    ViewBag.failure = "Usuario o Contraseña Inválidos.";
                    return View("Login",model);
                }

            }
            //if (this.IsCaptchaValid("Captcha no válido")) { 
            if (usuario == null)
            {
                return RedirectToAction("GetUnauthorizedView");
            }
            else if (usuario.Rol.Trim() == Rol.ADMINISTRADOR.ToString() || usuario.Rol.Trim() == Rol.COORDINADOR.ToString() || usuario.Rol.Trim().Contains(Rol.GERENTE.ToString()))
            {
                rol = usuario.Rol.Trim();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                   (1, usuario.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30),false, rol);
                String cifrado = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                Response.Cookies.Add(cookie);
                Session["estudioExternoId"] = null;
                
                SaveUserDataToSession(usuario);
                
                ViewBag.usuarioLogueado = usuario.UmeId;
                ViewBag.nombreUsuario = usuario.Apellido + "," + usuario.Nombre;
                ViewBag.rolUsuario = usuario.Rol;
                //ViewBag.Ingreso = "Usuario Logueado";
                // timer de sesion:
                Session["HoraInicioSesion"] = DateTime.Now.ToLocalTime().ToString();
                //Session["HorarioUltimaPeticion"] = DateTime.Now.ToLocalTime().ToString();
                //Session["MinutosDeInactividad"] = DateTime.Now.ToLocalTime().ToString();
                //
                return this.GetAdminView();

            }
            else if (usuario.Rol.Trim() == Rol.ANALISTA.ToString())
            {
                rol = usuario.Rol.Trim();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                   (1, usuario.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30),false, rol);
                String cifrado = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                Response.Cookies.Add(cookie);
                SaveUserDataToSession(usuario);
                Session["estudioExternoId"] = null;
                ViewBag.usuarioLogueado = usuario.UmeId;
                ViewBag.nombreUsuario = usuario.Apellido + "," + usuario.Nombre;
                ViewBag.rolUsuario = usuario.Rol;
               
                return this.GetCommonView();
            }
            else 
            if(usuario.Rol.Trim().Contains(Rol.ESTUDIO.ToString())) {
                rol = usuario.Rol.Trim();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                   (1, usuario.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30),false, rol);
                String cifrado = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                Response.Cookies.Add(cookie);
                SaveUserDataToSession(usuario);
                if (usuario.Estudio_Id != null)
                {
                    Session["estudioExternoId"] = usuario.Estudio_Id;
                }
                else {
                    throw new UnauthorizedAccessException("El responsable Externo debe estar asociado a un Estudio Jurídico");
                }                               
                Session["userRol"] = usuario.Rol;
                
                ViewBag.usuarioLogueado = usuario.UmeId;
                ViewBag.nombreUsuario = usuario.Apellido + "," + usuario.Nombre;
                ViewBag.rolUsuario = usuario.Rol;
                return this.GetCommonViewExternos();
            }
            else
            {
                //return RedirectToAction("Index"); 
                return RedirectToAction("GetUnauthorizedView");
            }
        //}
           // return RedirectToAction("GetUnauthorizedView");
        }

        public void SaveUserDataToSession(ResponsableDto user) {
            Session["usuarioLogueado"] = user.UmeId;
            Session["nombreUsuario"] = user.Nombre + ' ' + user.Apellido;
            Session["rolUsuario"] = user.Rol;
        }

        public ActionResult Inicio() {
            if (User.Identity.IsAuthenticated) {
                return this.GetCommonView();
            }
            return this.GetUnauthorizedView();
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [ValidateAntiForgeryToken]
        public ActionResult BackToTheWelcome() {
            var usuario = System.Web.HttpContext.Current.User;
            if (usuario.IsInRole("Administrador") || usuario.IsInRole("Coordinadores") || usuario.IsInRole("Gerente DC"))
            {
                return GetAdminView();
            }
            else
            if (usuario.IsInRole("Analista") || usuario.IsInRole("Estudio Externo"))
            {
                return GetCommonView();
            }
            
            return GetUnauthorizedView();
        }

        public ActionResult AlertSessionEnd() {
            return JavaScript("<script>toastr.error('La sesión ha culminado')</script>");
        } 

        public ActionResult CerrarSesion()
        {
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            FormsAuthentication.SignOut();
            //Recuperar la cookie y caducarla
            HttpCookie cookie = Request.Cookies["TicketUsuario"];
            if (cookie != null) {
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
                Session.RemoveAll();
                Session.Abandon();
            }

            return RedirectToAction("Login");
        }

        public ActionResult SesionCulminada() {
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            FormsAuthentication.SignOut();
            //Recuperar la cookie y caducarla
            HttpCookie cookie = Request.Cookies["TicketUsuario"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
                Session.RemoveAll();
                Session.Abandon();
            }

            return PartialView("SessionFinalizadaPartialView");
        }

        public ActionResult SesionCulminadaJson()
        {
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            FormsAuthentication.SignOut();
            //Recuperar la cookie y caducarla
            HttpCookie cookie = Request.Cookies["TicketUsuario"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
                Session.RemoveAll();
                Session.Abandon();
            }
            return Json("Ha finalizado la Sesión");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ingreso(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                return validateUser(loginModel);
            }
            else
            {
                return View("Login", new LoginViewModel());
            }
        }
        [ChildActionOnly]
        public ActionResult GetCommonViewExternos()
        {
            return View("WelcomeExterno");
        }

        [ChildActionOnly]
        public ActionResult GetCommonView()
        {
            return View("Welcome");
        }

        [ChildActionOnly]
        public ActionResult GetAdminView()
        {
            return View("WelcomeAdmin");
        }


        public ActionResult GetUnauthorizedView()
        {
            return View("Unauthorized");
        }

        public ActionResult tablaMuestra()
        {
            return View();
        }
        public ActionResult DenunciaEdicion()
        {
            return View();
        }

        public ActionResult MDenuncia()
        {
            return View();
        }


        public DenunciaModelView getDenunciasModelView() {
            DenunciaModelView model = new DenunciaModelView(null, null, null, null);
            var estados = new List<EstadoDto>();
            var estado = new EstadoDto();
            var subEstados = new List<SubEstadoDto>();
            var tiposProceso = new List<TipoProcesoDto>();
            var servicios = new List<ServicioDto>();
            var organismos = new List<OrganismoDto>();
            var estudios = new List<EstudioDto>();
            var tipoEventos = new List<TipoEventoDto>();
            var subtipoProcesos = new List<SubTipoProcesoDto>();
            var modalidadesDeGestion = new List<ModalidadGestionDto>();
            var motivosReclamo = new List<MotivoDeReclamoDto>();
            var reqsInforme = new List<ReqInformeDto>();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                estados = context.Estados.ToList();
                subEstados = context.SubEstados.Where(s => s.EstadoId == 1).ToList();
                tiposProceso = context.TiposDeProceso.ToList();
                servicios = context.Servicios.ToList();
                organismos = context.Organismos.ToList();
                //estudios = context.Estudios.ToList();
                tipoEventos = context.TiposDeEventos.ToList();
                subtipoProcesos = context.SubTiposDeProceso.ToList();
                modalidadesDeGestion = context.ModalidadesDeGestion.ToList();
                //motivosReclamo = context.MotivosDeReclamo.ToList();
                reqsInforme = context.ReqsInforme.ToList();
            }

            model.Estados = estados.Select(p => new SelectListItem { Text = p.TipoEstado, Value = p.Id.ToString() });
            model.SubEstados = subEstados.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.tiposProceso = tiposProceso.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Servicios = servicios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Organismos = organismos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.Estudios = estudios.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.tipoEventos = tipoEventos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.modalidadesDeGestion = modalidadesDeGestion.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.subTiposProceso = subtipoProcesos.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.motivosDeReclamo = motivosReclamo.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });

            model.reqsInforme = reqsInforme.Select(p => new SelectListItem { Text = p.Nombre, Value = p.Id.ToString() });
            model.eventos = new List<EventoSP>();
            //model.tipoProcesoSeleccionado = "";
            return model;
        }

        //public ActionResult Denuncia(string tipoDenuncia)
        //{
        //    var model = new DenunciaModelView(null,null);
        //    model.tipoDenuncia = tipoDenuncia;
        //    return View("NuevaDenuncia",new DenunciaModelView(null,null));
        //    //return View(new DenunciaModelView());
        //}

        public ActionResult Agenda()
        {
            return View();
        }
        //public ActionResult SaveDropzoneJsUploadedFiles()
        public JsonResult Upload()
        {

            //foreach (string fileName in Request.Files)
            //{
            foreach (string fileName in Request.Files)

            {
                HttpPostedFileBase file = Request.Files[fileName];

                //You can Save the file content here

            }

            //return Json(new { Message = string.Empty });
            //return new JsonResult(){ Data = Request.Files };
            return Json(Request.Files, JsonRequestBehavior.AllowGet);
        }

        //public ViewResult FiltroDenuncias() {
        public ActionResult FiltroDenuncias()
        {
            return RedirectToAction("FiltroDenuncias", "FiltroDenuncias");
            //return View(new FiltroDenunciasModelView());
        }

        public ActionResult NuevoEvento() {
            return PartialView("NuevoEvento", new EventoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEvento(int id)
        {
            //EventoDto evento = new EventoDto();
            //EventoQueryService eqs = new EventoQueryService();
            //evento = eqs.GetEventoById(id);
            //return Json(evento);
            EventoModelView model = new EventoModelView(id);
            return PartialView("NuevoEvento", model);
            //return PartialView("NuevoEvento", eqs.GetEventoById(id));
        }



        //[HttpPost]



        [HttpPost]
        public JsonResult GetMotivosDeReclamoPorServicio(int idServicio, int tipoProcesoId)
        {
            try
            {

                var motivosDeReclamo = new List<MotivoDeReclamoDto>();
                using (NuevoDbContext context = new NuevoDbContext()) {
                    motivosDeReclamo = context.MotivosDeReclamo
                                              .Where(m => m.servicioId == idServicio && m.tipoProcesoId == tipoProcesoId)
                                              .ToList();
                }
                return Json(motivosDeReclamo);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [HttpPost]
        public async Task<JsonResult> MotivosDeReclamoServicioTipoProceso(int idServicio)
        {
            try
            {
                var motivosDeReclamo = new List<MotivoReclamoServicioTipoProceso>();

                using (NuevoDbContext context = new NuevoDbContext())
                {
                    motivosDeReclamo = await context
                                       .Database
                                       .SqlQuery<MotivoReclamoServicioTipoProceso>("JoinTipoProcesoServicioMotivoReclamo")
                                       .ToListAsync();
                    motivosDeReclamo = motivosDeReclamo.Where(x => x.ServicioId == idServicio).ToList();
                }
                return Json(motivosDeReclamo);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [HttpPost]
        public JsonResult GetDomiciliosMediadores(int idMediador)
        {
            try
            {

                var domicilios = new List<DomicilioMediadorDto>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    domicilios = context.DomiciliosMediadores.Where(m => m.MediadorId == idMediador && m.Activo).ToList();
                }
                return Json(domicilios);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }
        [HttpPost]
        public JsonResult GetEstudiosPorOrganismo(int organismoId)
        {
            try
            {
                var estudios = new List<EstudioDto>();
                var organismo = new OrganismoDto();
                var relaciones = new List<OrganismoEstudioRelSP>();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    relaciones = context.Database
                        .SqlQuery<OrganismoEstudioRelSP>("GetOrganismoEstudioRelPorOrganismoId @organismoId", new SqlParameter("@organismoId", organismoId))
                        .ToList();
                }

                return Json(relaciones);
            }
            catch (Exception e) {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult GetDenunciante(string nombre, string apellido, int? dni)//
        {
            //oldVersion
            //var denunciante = new DenuncianteDto();
            //DenuncianteModelView dm = new DenuncianteModelView();
            //denunciante.nombre = nombre.ToUpper();
            //denunciante.apellido = apellido.ToUpper();
            //denunciante.Nro_Documento = dni;

            //DenuncianteQueryService dqs = new DenuncianteQueryService();

            //var denuncianteResult = (!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(nombre))
            //    ? dqs.getDenunciantes().Where(d => d.apellido == denunciante.apellido && d.nombre == denunciante.nombre).FirstOrDefault()
            //    : dqs.getDenunciantes().Where(d => d.Nro_Documento == denunciante.Nro_Documento).FirstOrDefault();
            //if (denuncianteResult != null)
            //{
            //    dm.denunciante = denuncianteResult;
            //    return PartialView("DenuncianteResult", dm);
            //}
            //else
            //{
            //    return PartialView("SinResultado");
            //}
            //try {
            DenuncianteQueryService dqs = new DenuncianteQueryService();
            var denunciante = new DenuncianteDto();

            if ((!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(nombre) && dni == null))
            {
                //try { 
                denunciante = dqs.GetDenunciantePorNombreyApellido(nombre, apellido);
                //}
                //catch (Exception e) {
                //    return PartialView("DenuncianteResult",null);
                //};
            }
            else
                if ((String.IsNullOrEmpty(apellido) && String.IsNullOrEmpty(nombre) && dni != null))
            {
                denunciante = dqs.GetDenunciantePorDni(dni);
            }
            else
                if ((!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(nombre) && (dni != null && dni.ToString().Trim().Length > 5)))
            {
                denunciante = dqs.GetDenunciantePorNombreApellidoyDni(nombre, apellido, dni);
            }
            else {
                return PartialView("SinResultado");
            }
            DenuncianteModelView dm = new DenuncianteModelView();
            //var denuncianteResult = denunciante.nombre + ' ' + denunciante.apellido + ' ' + denunciante.Nro_Documento;
            dm.denunciante = denunciante;
            return PartialView("DenuncianteResult", dm);

            //} catch (Exception e) {
            //    var m = e.Message;
            //    return PartialView("SinResultado");
            //}
        }

        //public DenuncianteModelView retornarDenuncianteModel(string denuncianteResult) {

        //}

        //[HttpPost]
        //public ActionResult Guardar(string nombre, string apellido, int? dni)
        //{
        //    var denunciante = new DenuncianteDto();
        //    denunciante.nombre = nombre.ToUpper();
        //    denunciante.apellido = apellido.ToUpper();
        //    denunciante.NroDocumento = dni.ToString();
        //    DenuncianteCommandService dcs = new DenuncianteCommandService();
        //    DenuncianteQueryService dqs = new DenuncianteQueryService();
        //    //var maximo = dqs.getDenunciantes().Max(d => d.DenuncianteId);
        //    //denunciante.DenuncianteId = maximo + 1;          
        //    int? newId = dcs.createDenunciante(denunciante);
        //    if (newId.HasValue) {
        //        DenuncianteDto newDenunciante = dqs.GetDenuncianteById((int)newId);
        //        DenuncianteModelView dm = new DenuncianteModelView();
        //        dm.denunciante = newDenunciante;
        //        return PartialView("DenuncianteResult", dm);
        //    }
        //    else
        //    {
        //        return PartialView("SinResultado");
        //    }


        //}

        NuevoDbContext _dbContext;

        //[HttpPost]
        public ActionResult Archivos()
        {
            List<ArchivoDto> _ArchivoDtos = this.GetArchivos();
            return PartialView("Archivos", _ArchivoDtos);
        }


        public List<ArchivoDto> GetArchivos() {
            List<ArchivoDto> _ArchivoDtos = new List<ArchivoDto>();
            using (_dbContext = new NuevoDbContext())
            {
                // Recuperamos la Lista de los ArchivoDtos subidos.
                //_ArchivoDtos = _dbContext.Archivos.OrderBy(x => x.fechaCreacion).ToList();
                _ArchivoDtos = _dbContext.Archivos.OrderByDescending(x => x.fechaCreacion).ToList();
            }
            return _ArchivoDtos;
        }


        
        //[HttpPost]
        //public ActionResult SaveDropzoneJsUploadedFiles()
        //{
        //    bool isSavedSuccessfully = true;
        //    string fName = "";
        //    try
        //    {
        //        foreach (string fileName in Request.Files)
        //        {
        //            HttpPostedFileBase file = Request.Files[fileName];
        //            //Save file content goes here
        //            fName = file.FileName;
        //            if (file != null && file.ContentLength > 0)
        //            {

        //                int indiceDelUltimoPunto = file.FileName.LastIndexOf('.');
        //                string _nombre = file.FileName.Substring(0, indiceDelUltimoPunto);
        //                string _extension = file.FileName.Substring(indiceDelUltimoPunto + 1,
        //                                    file.FileName.Length - indiceDelUltimoPunto - 1);
        //                ArchivoDto _archivo = new ArchivoDto()
        //                {
        //                    Nombre = _nombre,
        //                    Extension = _extension,
        //                    Descargas = 0
        //                };

        //                using (_dbContext = new NuevoDbContext())
        //                {
        //                    _dbContext.Archivos.Add(_archivo);
        //                    _dbContext.SaveChanges();
        //                };
        //                var _PathAplicacion = HttpContext.Request.PhysicalApplicationPath;
        //                bool isExists = System.IO.Directory.Exists(_PathAplicacion);

        //                if (!isExists)
        //                {
        //                    System.IO.Directory.CreateDirectory(_PathAplicacion);
        //                }
        //                var path = string.Format("{0}\\{1}", _archivo.PathCompleto, file);
        //                file.SaveAs(path);

        //            }
        //        }
        //    }
        //    catch (Exception ex)

        //    {
        //        isSavedSuccessfully = false;
        //    }


        //    if (isSavedSuccessfully)
        //    {
        //        return Json(new { Message = fName });
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Error in saving file" });
        //    }


        //}


        [HttpGet]
        public ActionResult DescargarArchivoDto(Guid id)
        {
            ArchivoDto _ArchivoDto;
            FileContentResult _fileContent;

            using (_dbContext = new NuevoDbContext())
            {
                _ArchivoDto = _dbContext.Archivos.FirstOrDefault(x => x.Id == id);
            }

            if (_ArchivoDto == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    // Descargamos el ArchivoDto del Servidor.
                    _fileContent = new FileContentResult(_ArchivoDto.DescargarArchivo(),
                                                         "application/octet-stream");
                    _fileContent.FileDownloadName = _ArchivoDto.Nombre + "." + _ArchivoDto.Extension;

                    // Actualizamos el nº de descargas en la base de datos.
                    using (_dbContext = new NuevoDbContext())
                    {
                        _ArchivoDto.Descargas++;
                        _dbContext.Archivos.Attach(_ArchivoDto);
                        _dbContext.Entry(_ArchivoDto).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }

                    return _fileContent;
                }
                catch (Exception ex)
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpGet]
        public ActionResult EliminarArchivoDto(Guid id)
        {
            ArchivoDto _ArchivoDto;

            using (_dbContext = new NuevoDbContext())
            {
                _ArchivoDto = _dbContext.Archivos.FirstOrDefault(x => x.Id == id);
            }

            if (_ArchivoDto != null)
            {
                using (_dbContext = new NuevoDbContext())
                {
                    _ArchivoDto = _dbContext.Archivos.FirstOrDefault(x => x.Id == id);
                    _dbContext.Archivos.Remove(_ArchivoDto);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        // Eliminamos el ArchivoDto del Servidor.
                        _ArchivoDto.EliminarArchivo();
                    }
                }
                // Redirigimos a la Acción 'Index' para mostrar
                // Los ArchivoDtos subidos al Servidor.
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GenerateSpreadsheet_old([Bind(Include="fechaNotifDesde,fechaNotifGciaDesde,fechaNotifHasta,fechaNotifGciaHasta,nroExpediente,etapaProcesalSeleccionada,organismoSeleccionado,regionSeleccionada,provinciaSeleccionada,localidadSeleccionada,servicioSeleccionado,idDenuncia,estudioSeleccionado,apellidoDenunciante,nombreDenunciante,tipoDocumento,dniDenunciante,estadoSeleccionado,nroLinea,tramiteCRM,motivoDeReclamoSeleccionado,responsableSeleccionado,responsableInterno,idDenunciaOriginal,exportarAExcel,esUnCambioMasivo,incluirDenunciasInactivas,verDenunciasEliminadas")] FiltroDenunciasModelView filtroModel) { 
            var lista = aplicarFiltros(filtroModel);
            //esto es provisorio.. Optimizar
            if (!filtroModel.verDenunciasEliminadas)
            {
                lista = lista.Where(x => x.DELETED == false || x.DELETED == null).ToList();
               
            }
            else {
                lista = lista.Where(x => x.DELETED == true).ToList();
            }

            if(!filtroModel.incluirDenunciasInactivas){
                lista = lista.Where(x => x.INACTIVO != true).ToList();
            }
            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                     e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                     e.Nro_Documento,
                                     e.Nro_Linea,
                                     e.Tramite_CRM,
                                     e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                     e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Mediador,
                                     e.Matricula,
                                     e.Domicilio_Mediador,
                                     e.Fecha_Homologacion,
                                     e.Nro_Gestion_Coprec,
                                     e.Honorarios_Coprec,
                                     e.Fecha_Gestion_Honorarios,
                                     e.Monto_Acordado,
                                     e.Arancel,
                                     e.Fecha_Gestion_Arancel,
                                     e.Agenda_Coprec,
                                     e.Denuncia_Preventiva,
                                     e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("Denuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);
                Session["pathReporteDenunciasAEliminar"] = fullPath;

                CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

                return Json(new { fileName = fileName, errorMessage = "" });
            }
            else {
                return Json(new { fileName = "isEmpty", errorMessage = "" });
            }
        }
        
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
           
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
               
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                   
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            
            return dataTable;
        }

        
        private List<ListaSabanaSP> aplicarFiltros(FiltroDenunciasModelView filtroModel)
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();

            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorResponsable @RESP_INT_ID", new SqlParameter("@RESP_INT_ID", filtroModel.responsableInterno))
                            .ToList();
                }
                lista = filtrarSinResponsable(filtroModel, lista);
            }
            else
             if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechasDeNotificacionGcia @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGcia(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechasDeNotificacion @fechaDesde,@fechaHasta", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde)
                                                                                                                  , new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacion(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionGciaDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifGciaDesde))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGciaDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifGciaHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionGciaHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifGciaHasta))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionGciaHasta(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifDesde != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionDesde @fechaDesde", new SqlParameter("@fechaDesde", filtroModel.fechaNotifDesde))

                            .ToList();
                }
                lista = filtrarSinFechaNotificacionDesde(filtroModel, lista);
            }
            else
            if (filtroModel.fechaNotifHasta != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorFechaDeNotificacionHasta @fechaHasta", new SqlParameter("@fechaHasta", filtroModel.fechaNotifHasta))
                            .ToList();
                }
                lista = filtrarSinFechaNotificacionHasta(filtroModel, lista);
            }
            else
            if (filtroModel.nroExpediente != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorNroExpediente @nroExpediente", new SqlParameter("@nroExpediente", filtroModel.nroExpediente))
                            .ToList();
                }
                lista = filtrarSinNroExpediente(filtroModel, lista);
            }
            else
            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEtapaProcesal @etapaProcesalId", new SqlParameter("@etapaProcesalId", filtroModel.etapaProcesalSeleccionada))
                            .ToList();
                }
                lista = filtrarSinEtapaProcesal(filtroModel, lista);
            }
            else
            if (filtroModel.organismoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorOrganismo @organismoId", new SqlParameter("@organismoId", filtroModel.organismoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinOrganismo(filtroModel, lista);
            }
            else
            if (filtroModel.regionSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorRegion @regionId", new SqlParameter("@regionId", filtroModel.regionSeleccionada))
                            .ToList();
                }
                lista = filtrarSinRegion(filtroModel, lista);
            }
            else
            if (filtroModel.provinciaSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorProvincia @provinciaId", new SqlParameter("@provinciaId", filtroModel.provinciaSeleccionada))
                            .ToList();
                }
                lista = filtrarSinProvincia(filtroModel, lista);
            }
            else
            if (filtroModel.localidadSeleccionada > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorLocalidad @localidadId", new SqlParameter("@localidadId", filtroModel.localidadSeleccionada))
                            .ToList();
                }
                lista = filtrarSinLocalidad(filtroModel, lista);
            }
            else
            if (filtroModel.servicioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorServicio @servicioId", new SqlParameter("@servicioId", filtroModel.servicioSeleccionado))
                            .ToList();
                }
                lista = filtrarSinServicio(filtroModel, lista);
            }
            else
            if (filtroModel.idDenuncia != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorIdDenuncia @denunciaId", new SqlParameter("@denunciaId", filtroModel.idDenuncia))
                            .ToList();
                }
                lista = filtrarSinIdDenuncia(filtroModel, lista);
            }
            else
            if (filtroModel.estudioSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEstudio @estudioId", new SqlParameter("@estudioId", filtroModel.estudioSeleccionado))
                            .ToList();
                }
                lista = filtrarSinEstudio(filtroModel, lista);
            }
            //else 
            //if (filtroModel.verDenunciasEliminadas)
            //{

            //}
            else
            if (filtroModel.apellidoDenunciante != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorApellido @apellido", new SqlParameter("@apellido", filtroModel.apellidoDenunciante))
                            .ToList();
                }
                lista = filtrarSinApellido(filtroModel, lista);
            }
            else
            if (filtroModel.nombreDenunciante != null)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorNombreDenunciante @nombre", new SqlParameter("@nombre", filtroModel.nombreDenunciante))
                            .ToList();
                }
                lista = filtrarSinNombre(filtroModel, lista);
            }
           
            else
            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorDni @dni", new SqlParameter("@dni", filtroModel.dniDenunciante))
                            .ToList();
                }
                lista = filtrarSinNro_Documento(filtroModel, lista);
            }
            else
            if (filtroModel.estadoSeleccionado != null) /* if (!String.IsNullOrEmpty(filtroModel.estadoSeleccionado))*/
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorEstadoConciliacion @estadoId", new SqlParameter("@estadoId", filtroModel.estadoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinEstadoConciliacion(filtroModel, lista);
            }
            else
            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorNroLinea @nroLinea", new SqlParameter("@nroLinea", filtroModel.nroLinea))
                            .ToList();
                }
                lista = filtrarSinNroLinea(filtroModel, lista);
            }
            else
            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP ExcelGetDenunciasPorTramiteCRM
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorTramiteCRM @tramiteCRM", new SqlParameter("tramiteCRM", filtroModel.tramiteCRM))
                            .ToList();
                }
                lista = filtrarSinTramiteCRM(filtroModel, lista);

            }

            else
            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    lista = context.Database
                            .SqlQuery<ListaSabanaSP>("ExcelGetDenunciasPorMotivoDeReclamo @motivoReclamo", new SqlParameter("@motivoReclamo", filtroModel.motivoDeReclamoSeleccionado))
                            .ToList();
                }
                lista = filtrarSinMotivoDeReclamo(filtroModel, lista);
            }
            return lista;
        }

        private List<ListaSabanaSP> filtrarSinNroExpediente(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString().ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinEtapaProcesal(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            //if (filtroModel.etapaProcesalSeleccionada > 0)
            //{
            //    lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            //}

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString().ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }
            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinOrganismo(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            //if (filtroModel.organismoSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            //}

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
                //lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinRegion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            //if (filtroModel.regionSeleccionada > 0)
            //{
            //    lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            //}

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinProvincia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }
            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }
            
            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinLocalidad(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            //if (filtroModel. localidadSeleccionada > 0)
            //{
            //    lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            //}

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinServicio(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel.localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            //if (filtroModel.servicioSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            //}

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinIdDenuncia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            //if (filtroModel.idDenuncia != null)
            //{
            //    lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            //}

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinEstudio(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            //if (filtroModel.estudioSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            //}
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinApellido(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            //if (filtroModel.apellidoDenunciante != null)
            //{
            //    lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            //}

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNombre(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            //if (filtroModel.nombreDenunciante != null)
            //{
            //    lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            //}

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNro_Documento(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            //if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            //{
            //    lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            //}
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinEstadoConciliacion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            //if (filtroModel.verDenunciasEliminadas)
            //{
            //    lista = lista.Where(e => e.DELETED == true).ToList();
            //}

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            //if (filtroModel.estadoSeleccionado != null)
            //{
            //    lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            //}

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinNroLinea(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            //if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            //{
            //    lista = lista.Where(e => e.nroLinea == filtroModel.nroLinea.ToString()).ToList();
            //}

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinTramiteCRM(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            //if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            //{
            //    lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            //}

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinMotivoDeReclamo(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            //if (filtroModel.verDenunciasEliminadas)
            //{

            //}

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            //if (filtroModel.motivoDeReclamoSeleccionado > 0)
            //{
            //    lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            //}
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinResponsable(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
                //lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }
        private List<ListaSabanaSP> filtrarSinFechaNotificacionDesde(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
            //                         && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            //}
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionHasta(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            //if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
            //                         && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            //if (filtroModel.fechaNotifHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            //}

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGciaDesde(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            //if (filtroModel.fechaNotifGciaDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            //}

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGciaHasta(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            //if (filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacion(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            
            if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
                                     && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }
            
            if (filtroModel.fechaNotifGciaDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            }

            if (filtroModel.fechaNotifGciaHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            }

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                        
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
                //lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        private List<ListaSabanaSP> filtrarSinFechaNotificacionGcia(FiltroDenunciasModelView filtroModel, List<ListaSabanaSP> lista)
        {
            if (filtroModel.responsableInterno != null && filtroModel.responsableInterno > 0)
            {
                lista = lista.Where(e => e.RESP_INT_ID == filtroModel.responsableInterno).ToList();
            }

            if (filtroModel.fechaNotifDesde != null && filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)
                                     && e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }
            //if (filtroModel.fechaNotifGciaDesde != null && filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)
            //                         && e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.fechaNotifDesde != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion >= DateTime.Parse(filtroModel.fechaNotifDesde)).ToList();
            }
            if (filtroModel.fechaNotifHasta != null)
            {
                lista = lista.Where(e => e.Fecha_Notificacion <= DateTime.Parse(filtroModel.fechaNotifHasta)).ToList();
            }

            //if (filtroModel.fechaNotifGciaDesde != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia >= DateTime.Parse(filtroModel.fechaNotifGciaDesde)).ToList();
            //}

            //if (filtroModel.fechaNotifGciaHasta != null)
            //{
            //    lista = lista.Where(e => e.Fecha_Notificacion_Gcia <= DateTime.Parse(filtroModel.fechaNotifGciaHasta)).ToList();
            //}

            if (filtroModel.nroExpediente != null)
            {
                lista = lista.Where(e =>  e.Expediente == filtroModel.nroExpediente).ToList();
            }

            if (filtroModel.etapaProcesalSeleccionada > 0)
            {
                lista = lista.Where(e => e.ETAPA_ID == filtroModel.etapaProcesalSeleccionada).ToList();
            }

            if (filtroModel.organismoSeleccionado > 0)
            {
                lista = lista.Where(e => e.ORGANISMO_ID == filtroModel.organismoSeleccionado).ToList();
            }

            if (filtroModel.regionSeleccionada > 0)
            {
                lista = lista.Where(e => e.Region_Id == filtroModel.regionSeleccionada).ToList();
            }

            if (filtroModel.provinciaSeleccionada > 0)
            {
                lista = lista.Where(e => e.Provincia_Id == filtroModel.provinciaSeleccionada).ToList();
            }

            if (filtroModel. localidadSeleccionada > 0)
            {
                lista = lista.Where(e => e.Localidad_Id == filtroModel.localidadSeleccionada).ToList();
            }

            if (filtroModel.servicioSeleccionado > 0)
            {
                lista = lista.Where(e => e.SERV_DEN_ID == filtroModel.servicioSeleccionado).ToList();
            }

            if (filtroModel.idDenuncia != null)
            {
                lista = lista.Where(e => e.DenunciaId == filtroModel.idDenuncia).ToList();
            }

            if (filtroModel.estudioSeleccionado > 0)
            {
                lista = lista.Where(e => e.ESTUDIO_ID == filtroModel.estudioSeleccionado).ToList();
            }
            // No está creado el Stored Procedure
            if (filtroModel.verDenunciasEliminadas)
            {

            }

            if (filtroModel.apellidoDenunciante != null)
            {
                lista = lista.Where(e => e.Apellido.Contains(filtroModel.apellidoDenunciante)).ToList();
            }

            if (filtroModel.nombreDenunciante != null)
            {
                lista = lista.Where(e => e.Nombre.Contains(filtroModel.nombreDenunciante)).ToList();
            }

            if (filtroModel.dniDenunciante != null)// --> va solo ? Falta el SPq    
            {
                lista = lista.Where(e => String.Equals(e.Nro_Documento,filtroModel.dniDenunciante.ToString())).ToList();
            }
            if (filtroModel.estadoSeleccionado != null)
            {
                lista = lista.Where(e => e.Estado_Actual == filtroModel.estadoSeleccionado).ToList();
            }

            if (filtroModel.nroLinea != null)// --> va solo ? Falta el SP
            {
                lista = lista.Where(e => e.Nro_Linea == filtroModel.nroLinea.ToString()).ToList();
            }

            if (filtroModel.tramiteCRM != null)// --> va solo? Falta el SP
            {
                lista = lista.Where(e => e.Tramite_CRM == filtroModel.tramiteCRM).ToList();
            }

            if (filtroModel.motivoDeReclamoSeleccionado > 0)
            {
                lista = lista.Where(e => e.RECLAMO_ID == filtroModel.motivoDeReclamoSeleccionado).ToList();
            }
            return lista;

        }

        public ActionResult GenerateSpreadsheet(List<DenunciaExcel> lista)
        {
            var path = Server.MapPath("~/temp");
            var fileName = "ReporteDenuncias"
                            + DateTime.Now.Year.ToString()
                            + DateTime.Now.Month.ToString()
                            + DateTime.Now.Day.ToString()
                            + ".xlsx";

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            
            DataSet dataSet = new System.Data.DataSet("DenunciasSpreadsheet");
            //dataSet.Tables.Add(Table());
            dataSet.Tables.Add(ToDataTable(lista));
            // Create the Excel file in temp path
            string fullPath = Path.Combine(path, fileName);
            //CreateExcelFile cef = new CreateExcelFile();
            CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

            // Return the Excel file name
            //return Json(new { fileName = fileName, errorMessage = "" }, JsonRequestBehavior.AllowGet);
            fullPath = Path.Combine(Server.MapPath("~/temp"), fileName);

            // Return the file for download, this is an Excel 
            // so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", fileName);
        }

        //[HttpGet]
        //[NoCache]
        //public ActionResult DownloadSpreadsheet(string file)
        //{
        //    // Get the temp folder and file path in server
        //    string fullPath = Path.Combine(Server.MapPath("~/temp"), file);            
        //    // Return the file for download, this is an Excel 
        //    // so I set the file content type to "application/vnd.ms-excel"
        //    return File(fullPath, "application/vnd.ms-excel", file);
        //}

        //public ActionResult EliminarReporteDenuncias()
        //{
                      
        //    string fullpath = (string)Session["pathReporteDenunciasAEliminar"];
        //    System.IO.File.Delete(fullpath);
        //    return Json("Se ha eliminado archivo temporal");
        //}

        private DataTable Table()
        {
            // Sample dataset from https://www.dotnetperls.com/dataset and I added more entries

            //DataTable table = new DataTable("Prescription");
            //table.Columns.Add("Dosage", typeof(int));
            //table.Columns.Add("Drug", typeof(string));
            //table.Columns.Add("Patient", typeof(string));
            //table.Columns.Add("Date", typeof(DateTime));

            //table.Rows.Add(25, "Indocin", "David", DateTime.Parse("01/01/2001"));
            //table.Rows.Add(50, "Enebrel", "Sam", DateTime.Parse("01/01/2002"));
            //table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Parse("01/01/2003"));
            //table.Rows.Add(21, "Combivent", "Janet", DateTime.Parse("01/01/2004"));
            //table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Parse("01/01/2005"));

            //table.Rows.Add(50, "Meloxicam", "Nobert", DateTime.Parse("02/01/2001"));
            //table.Rows.Add(25, "Clonazepam", "Noah", DateTime.Parse("03/01/2002"));
            //table.Rows.Add(150, "Metformin", "Liam", DateTime.Parse("04/01/2003"));
            //table.Rows.Add(300, "Cyclobenzaprine", "Mason", DateTime.Parse("05/01/2004"));
            //table.Rows.Add(200, "Doxycycline", "Jacob", DateTime.Parse("06/01/2005"));

            //table.Rows.Add(250, "Gabapentin", "William", DateTime.Parse("01/02/2001"));
            //table.Rows.Add(125, "Hydrochlorothiazide", "Linda", DateTime.Parse("01/03/2002"));
            //table.Rows.Add(10, "Ibuprofen", "Ethan", DateTime.Parse("01/04/2003"));
            //table.Rows.Add(15, "Lexapro", "James", DateTime.Parse("01/05/2004"));
            //table.Rows.Add(50, "Lorazepam", "Alexander", DateTime.Parse("01/06/2005"));

            using (NuevoDbContext context = new NuevoDbContext())
            {

                var lista = context.Database
                        .SqlQuery<ListaSabanaSP>("GetListaSabana")
                        .ToList();
                return this.ToDataTable(lista);
            }
            //return table;
        }



        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuted(ActionExecutedContext context)
            {
                context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

        //[HttpPost]
        public ActionResult ExportListToExcel(FiltroDenunciasModelView filtroModel)
        //public FileResult ExportListToExcel(FiltroDenunciasModelView filtroModel)
        {

            //var grid = new System.Web.UI.WebControls.GridView();

            //List<ListaSabanaSP> ln = new List<ListaSabanaSP>();
            var lista = aplicarFiltros(filtroModel);
            var listaNueva = (from e in lista

                              select new DenunciaExcel()
                              {
                                  //DenunciaId = e.DenunciaId,
                                  //TipoProceso = e.TipoProceso,
                                  //Etapa_Procesal = e.Etapa_Procesal,
                                  //usuarioCreador = e.usuarioCreador,
                                  //EXPEDIENTE_ID =  e.Expediente,
                                  //Denunciante = e.Denunciante,
                                  //DenuncianteIndividual = e.DenuncianteIndividual,
                                  //Nro_Documento = e.Nro_Documento,
                                  //nro_Linea = e.Nro_Linea,
                                  //TRAMITECRM = e.Tramite_CRM,
                                  //FCREACION = e.FCREACION,
                                  //Fecha_Notificacion = e.Fecha_Notificacion,
                                  //Fecha_Notificacion_Gcia = e.Fecha_Notificacion_Gcia,
                                  //Organismo = e.Organismo,
                                  //Localidad = e.Localidad,
                                  //Provincia = e.Provincia,
                                  //Region = e.Region,
                                  //Responsable_Interno = e.Responsable_Interno,
                                  //Estudio = e.Estudio,
                                  //Modalidad_Gestion = e.Modalidad_Gestion,
                                  //Sub_Tipo_Proceso = e.Sub_Tipo_Proceso,
                                  //Servicio = e.Servicio,
                                  //Motivo_Reclamo = e.Motivo_Reclamo,
                                  //Estado_Actual = e.Estado_Actual,
                                  //FECHARESULTADO = e.FECHARESULTADO,
                                  //denunciaPreventiva = e.denunciaPreventiva,
                                  //denunciaFormal = e.denunciaFormal

                              }).ToList();

            return (GenerateSpreadsheet(listaNueva));

            //grid.DataSource = listaNueva;
            //grid.DataBind();

            //Response.ClearContent();
            //Response.AddHeader("content-disposition", "attachment; filename=Exported.xls");
            //Response.ContentType = "application/excel";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //Response.Write(sw.ToString());

            //Response.End();

        }

        //public void ExportListToExcel_old(FiltroDenunciasModelView filtroModel)
        
        //{

            //var grid = new System.Web.UI.WebControls.GridView();
            
            //var lista = aplicarFiltros(filtroModel);
            //var listaNueva = from e in lista

                             //select new
                             //{
                                 //e.DenunciaId,
                                 //e.TipoProceso,
                                 //e.Etapa_Procesal,
                                 //e.usuarioCreador,
                                 // e.Expediente,
                                 //e.Denunciante,
                                 //e.DenuncianteIndividual,
                                 //e.Nro_Documento,
                                 //e.Nro_Linea,
                                 //e.Tramite_CRM,
                                 //e.FCREACION,
                                 //e.Fecha_Notificacion,
                                 //e.Fecha_Notificacion_Gcia,
                                 //e.Organismo,
                                 //e.Localidad,
                                 //e.Provincia,
                                 //e.Region,
                                 //e.Responsable_Interno,
                                 //e.Estudio,
                                 //e.Modalidad_Gestion,
                                 //e.Sub_Tipo_Proceso,
                                 //e.Servicio,
                                 //e.Motivo_Reclamo,
                                 //e.Estado_Actual,
                                 //e.FECHARESULTADO,
                                 //e.denunciaPreventiva,
                                 //e.denunciaFormal

                             //};

            //GenerateSpreadsheet(listaNueva);

            //grid.DataSource = listaNueva;
            //grid.DataBind();

            //Response.ClearContent();
            //Response.AddHeader("content-disposition", "attachment; filename=Exported.xls");
            //Response.ContentType = "application/excel";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //Response.Write(sw.ToString());

            //Response.End();

        //}
        //[HttpPost]
        //public JsonResult GetLocalidadesPorProvincia(int idProvincia)
        //{
        //    try
        //    {
        //        var localidades = new List<LocalidadDto>();
        //        using (NuevoDbContext context = new NuevoDbContext())
        //        {
        //            localidades = context.Localidades.Where(m => m.Provincia_Id == idProvincia).OrderBy(m => m.Nombre).ToList();
        //        }
        //        return Json(localidades);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(e.Message);
        //    }

        //}

        public static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
        {
            //We really want to know if there is any data at all
            var firstRecord = v.FirstOrDefault();
            if (firstRecord == null)
                return null;

            /*Okay, we have some data. Time to work.*/

            //So dear record, what do you have?
            PropertyInfo[] infos = firstRecord.GetType().GetProperties();

            //Our table should have the columns to support the properties
            DataTable table = new DataTable();

            //Add, add, add the columns
            foreach (var info in infos)
            {

                Type propType = info.PropertyType;

                if (propType.IsGenericType
                    && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) //Nullable types should be handled too
                {
                    table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }

            //Hmm... we are done with the columns. Let's begin with rows now.
            DataRow row;

            foreach (var record in v)
            {
                row = table.NewRow();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row[i] = infos[i].GetValue(record) != null ? infos[i].GetValue(record) : DBNull.Value;
                }

                table.Rows.Add(row);
            }

            //Table is ready to serve.
            table.AcceptChanges();

            return table;
        }

        [HttpPost]
        public JsonResult GenerarListaSabanaExcel()
        {
            List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                lista = context.Database
                        .SqlQuery<ListaSabanaSP>("ExcelGetListaSabana")
                        .ToList();
            }

            if (lista.Any())
            {
                var listaNueva = from e in lista

                                 select new
                                 {
                                     e.DenunciaId,
                                     e.Tipo_Proceso,
                                     e.Etapa_Procesal,
                                     e.Usuario_Creador,
                                     e.Expediente,
                                     e.Denunciante,
                                     e.Denunciante_Individual,
                                     e.Nro_Documento,
                                     e.Nro_Linea,
                                     e.Tramite_CRM,
                                     e.Fecha_Creacion,
                                     e.Fecha_Notificacion,
                                     e.Fecha_Notificacion_Gcia,
                                     e.Organismo,
                                     e.Localidad,
                                     e.Provincia,
                                     e.Region,
                                     e.Responsable_Interno,
                                     e.Estudio,
                                     e.Modalidad_Gestion,
                                     e.Sub_Tipo_Proceso,
                                     e.Servicio,
                                     e.Motivo_Reclamo,
                                     e.Estado_Actual,
                                     e.Fecha_Resultado,
                                     e.Mediador,
                                     e.Matricula,
                                     e.Domicilio_Mediador,
                                     e.Fecha_Homologacion,
                                     e.Nro_Gestion_Coprec,
                                     e.Honorarios_Coprec,
                                     e.Fecha_Gestion_Honorarios,
                                     e.Monto_Acordado,
                                     e.Arancel,
                                     e.Fecha_Gestion_Arancel,
                                     e.Agenda_Coprec,
                                     e.Denuncia_Preventiva,
                                     e.Denuncia_Formal

                                 };

                var tabla = LinqQueryToDataTable(listaNueva);

                var path = Server.MapPath("~/temp");

                var fileName = "ReporteDenuncias"
                               + DateTime.Now.Year.ToString()
                               + DateTime.Now.Month.ToString()
                               + DateTime.Now.Day.ToString()
                               + ".xlsx";

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                DataSet dataSet = new System.Data.DataSet("Denuncias");

                dataSet.Tables.Add(tabla);

                string fullPath = Path.Combine(path, fileName);

                CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

                return Json(new { fileName = fileName, errorMessage = "" });
            }
            else
            {
                return Json(new { fileName = "isEmpty", errorMessage = "" });
            }
        }



    }

}