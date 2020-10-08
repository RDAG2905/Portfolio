using CaptchaMvc.HtmlHelpers;
using Greco2.Model;
using Greco2.Models.Enum;
using Greco2.Models.Responsables;
//using Greco2.Models.User;
using Greco2.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Greco2.Models.User;

namespace Greco2.Controllers
{
    public class UserController : Controller
    {


        public ActionResult Index()
        {
            //ViewBag.usuarioLogueado = "";
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            return View("Login", new Models.Login.LoginViewModel());
            //return View("Login", new UserLogin());
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.Login.LoginViewModel login)
        {
            //if (this.IsCaptchaValid("Captcha no válido"))
            //{

            //if (login.Password == "Greco2020*" && login.User == "u200000")
            //{
            //    using (NuevoDbContext db = new NuevoDbContext())
            //    {
            //        var unRol = db.Roles.Where(x => x.Id == 1).FirstOrDefault();
            //        unRol.NameRol = unRol.NameRol.ToUpper().Trim();
            //        unRol.DNRol = "cn=20200415161046536,cn=TEAM,cn=TEAM,cn=Level10,cn=RoleDefs,cn=RoleConfig,cn=AppConfig,cn=UserApplication,cn=DriverSet1,ou=Servicios,o=Telecom";
            //        unRol.Aplication = "TEAM";
            //        unRol.System = "TEAM";
            //        db.SaveChanges();

            //        var unRol2 = db.Roles.Where(x => x.Id == 2).FirstOrDefault();
            //        unRol2.NameRol = "ANALISTA";
            //        unRol2.DNRol = "cn=20200415161045821,cn=TEAM,cn=TEAM,cn=Level10,cn=RoleDefs,cn=RoleConfig,cn=AppConfig,cn=UserApplication,cn=DriverSet1,ou=Servicios,o=Telecom";
            //        unRol2.Aplication = "TEAM";
            //        unRol2.System = "TEAM";
            //        db.SaveChanges();


            //        var unRol3 = db.Roles.Where(x => x.Id == 3).FirstOrDefault();
            //        unRol3.NameRol = "COORDINADOR";
            //        unRol3.DNRol = "cn=20200415161045096,cn=TEAM,cn=TEAM,cn=Level10,cn=RoleDefs,cn=RoleConfig,cn=AppConfig,cn=UserApplication,cn=DriverSet1,ou=Servicios,o=Telecom";
            //        unRol3.Aplication = "TEAM";
            //        unRol3.System = "TEAM";
            //        db.SaveChanges();

            //        var unRol4 = db.Roles.Where(x => x.Id == 4).FirstOrDefault();
            //        unRol4.NameRol = "ESTUDIO";
            //        unRol4.DNRol = "cn=20200415161047364,cn=TEAM,cn=TEAM,cn=Level10,cn=RoleDefs,cn=RoleConfig,cn=AppConfig,cn=UserApplication,cn=DriverSet1,ou=Servicios,o=Telecom";
            //        unRol4.Aplication = "TEAM";
            //        unRol4.System = "TEAM";
            //        db.SaveChanges();

            //        var unRol5 = db.Roles.Where(x => x.Id == 5).FirstOrDefault();
            //        unRol5.NameRol = "GERENTE";
            //        unRol5.DNRol = "cn=20200415161044193,cn=TEAM,cn=TEAM,cn=Level10,cn=RoleDefs,cn=RoleConfig,cn=AppConfig,cn=UserApplication,cn=DriverSet1,ou=Servicios,o=Telecom";
            //        unRol5.Aplication = "TEAM";
            //        unRol5.System = "TEAM";
            //        db.SaveChanges();
            //        db.Database.ExecuteSqlCommand("Update tResponsables set Rol = 'ADMINISTRADOR' where Rol= 'Administrador'");//89
            //        db.Database.ExecuteSqlCommand("Update tResponsables set Rol = 'ANALISTA' where Rol= 'Analista'");//21
            //        db.Database.ExecuteSqlCommand("Update tResponsables set Rol = 'COORDINADOR' where Rol= 'Coordinadores'");//89
            //        db.Database.ExecuteSqlCommand("Update tResponsables set Rol = 'GERENTE' where Rol= 'Gerente DC'");//21
            //        db.Database.ExecuteSqlCommand("Update tResponsables set Rol = 'ESTUDIO' where Rol= 'Estudio Externo'");//2
            //        IEnumerable<Role> roles = db.Roles.ToList();
            //        return Json(roles, JsonRequestBehavior.AllowGet);
            //    }

            //ADMembership adHelper = new ADMemberioship();
            //var itenes1 = "Lista de Roles vacia -- ";
            //var itenes2 = "Lista de Roles vacia -- ";
            //var itenes3 = "Lista de Roles vacia -- ";
            //var itenes4 = "Lista de Roles vacia -- ";

            //List<string> list1 = adHelper.GetUserRoles("u551212","TEAM","TEAM");
            ////List<string> list1 = adHelper.GetUserRoles("GRE551212", "Greco", "Greco");
            //foreach (var item in list1)
            //{
            //    itenes1 = item + " -- ";
            //}
            //List<string> list2 = adHelper.GetUserRoles("u194486", "TEAM", "TEAM");
            ////List<string> list2 = adHelper.GetUserRoles("GRE194486", "Greco", "Greco");
            //foreach (var item in list2)
            //{
            //    itenes2 = item + " -- ";
            //}
            //List<string> list3 = adHelper.GetUserRoles("u182874", "TEAM", "TEAM");
            ////List<string> list3 = adHelper.GetUserRoles("GRE182874", "Greco", "Greco");
            //foreach (var item in list3)
            //{
            //    itenes3 = item + " -- ";
            //}
            //List<string> list4 = adHelper.GetUserRoles("u194596", "TEAM", "TEAM");
            ////List<string> list4 = adHelper.GetUserRoles("GRE194596", "Greco", "Greco");
            //foreach (var item in list4)
            //{
            //    itenes4 = item + " -- ";
            //}

            //ViewBag.item1 = "ADMINISTRADOR u551212 : " + itenes1;
            //ViewBag.item2 = "ADMINISTRADOR u194486 : " + itenes2;
            //ViewBag.item3 = "ANALISTA u182874 : " + itenes3;
            //ViewBag.item4 = "COORDINADOR u194596 : " + itenes4;


            //return View();
            //}
            //else {
            //    using (NuevoDbContext db = new NuevoDbContext()) {
            //        IEnumerable<ResponsableDto> resp = db.Responsables.ToList();
            //        return Json(resp, JsonRequestBehavior.AllowGet);

            //    }

            //ViewBag.failure ="Credencial Inválida";
            //return View();
            //}



            string status = "false";
            string message = "";

            AccountADMembership objAccount = new AccountADMembership();


            if (objAccount.ValidateUser(login.User, login.Password))
            {
                ResponsableDto responsable = new ResponsableDto();

                var rol = "";
                using (NuevoDbContext db = new NuevoDbContext())
                {
                    responsable = db.Responsables.Where(x => x.UmeId == login.User).FirstOrDefault();
                    if (responsable == null)
                    {
                        ViewBag.Failure = "El Usuario no se encuentra en la Lista de Responsables";
                        return View();

                    }
                    var xRol = db.Roles.Where(x => responsable.Rol.Contains(x.NameRol.Trim())).FirstOrDefault();
                    rol = xRol.NameRol.Trim();
                }



                //if (responsable != null)
                //{
                //    rol = responsable.Rol;
                //    if (rol == "")
                //    {
                //        ViewBag.Failure = "El Responsable no tiene un Rol Definido";
                //        return View();
                //    }
                //}
                //else
                //   if (responsable == null)
                //{
                //    ViewBag.Failure = "El Usuario no se encuentra en la Lista de Responsables";
                //    return View();

                //}
                //var rol = "Administrador";

                if (!objAccount.VerifyRole(login, rol))
                {
                    ViewBag.failure = "La lista UserRoles está vacia o no contiene el Rol Asignado";
                    return View();
                    //return this.GetUnauthorizedView();

                    //return RedirectToAction("GetUnauthorizedView","User",null);

                }

                //if(rol.Trim() == "Administrador")
                if (rol.Trim().Contains(Rol.ADMINISTRADOR.ToString()) || rol.Trim().Contains(Rol.COORDINADOR.ToString()) || rol.Trim().Contains(Rol.GERENTE.ToString()))


                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                   (1, responsable.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30), false, rol);
                    String cifrado = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                    Response.Cookies.Add(cookie);
                    SaveUserDataToSession(responsable);
                    ViewBag.usuarioLogueado = responsable.UmeId;
                    ViewBag.nombreUsuario = responsable.Apellido + "," + responsable.Nombre;
                    ViewBag.rolUsuario = rol;
                    return this.GetAdminView();
                    //return RedirectToAction("GetAdminView", "User", new { @status = status });

                }


                else if (rol.Trim().Contains(Rol.ANALISTA.ToString()))
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
               (1, responsable.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30), false, rol);
                    String cifrado = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                    Response.Cookies.Add(cookie);
                    SaveUserDataToSession(responsable);
                    ViewBag.usuarioLogueado = responsable.UmeId;
                    ViewBag.nombreUsuario = responsable.Apellido + "," + responsable.Nombre;
                    ViewBag.rolUsuario = rol;
                    return this.GetCommonView();
                    //return RedirectToAction("GetCommonView", "User", new { @status = status });

                }
                else
          if (rol.Trim().Contains(Rol.ESTUDIO.ToString()))
                {

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                       (1, responsable.UmeId, DateTime.Now, DateTime.Now.AddMinutes(30), false, rol);
                    String cifrado = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie("TicketUsuario", cifrado);
                    Response.Cookies.Add(cookie);
                    SaveUserDataToSession(responsable);
                    if (responsable.Estudio_Id != null)
                    {
                        Session["estudioExternoId"] = responsable.Estudio_Id;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("El responsable Externo debe estar asociado a un Estudio Jurídico");
                    }
                    Session["userRol"] = responsable.Rol;

                    ViewBag.usuarioLogueado = responsable.UmeId;
                    ViewBag.nombreUsuario = responsable.Apellido + "," + responsable.Nombre;
                    ViewBag.rolUsuario = rol;
                    return this.GetCommonViewExternos();
                    //return RedirectToAction("GetCommonViewExternos", "User", new { @status = status });

                }
                else
                {
                    ViewBag.failure = "USUARIO NO AUTORIZADO";
                    return View();
                }


            }
            else
            {
                message = "CREDENCIAL INVÁLIDA";
            }
            ViewBag.Status = status;
            ViewBag.Failure = message;

            //}
            return View();
        }

        public void SaveUserDataToSession(ResponsableDto user)
        {
            Session["usuarioLogueado"] = user.UmeId;
            Session["nombreUsuario"] = user.Nombre + ' ' + user.Apellido;
            Session["rolUsuario"] = user.Rol;
        }

        public ActionResult CerrarSesion()
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

            return RedirectToAction("Login");
        }

        public ActionResult SesionCulminada()
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


        //Logout
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            ViewBag.Status = true;
            ViewBag.Message = "LogOut successfully";
            return View();
        }

        //Verify Account  
        //[HttpGet]
        ////public ActionResult VerifyAccount(UserLogin login, string status)
        //public ActionResult VerifyAccount(LoginViewModel login, string status)
        //{
        //    string message = "";

        //    if (Convert.ToBoolean(status))
        //        message = "Your account has been authorized";
        //    else
        //        message = "Your account has not been authorized";

        //    ViewBag.Status = status;
        //    ViewBag.Message = message;
        //    return View();
        //}

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


        [HttpGet]
        public ActionResult LoginSuccess(string id)
        {
            bool Status = false;
            ViewBag.Message = "OK";
            Status = true;
            ViewBag.Status = Status;
            return View();
        }
    }

    public interface IADMembership
    {
        bool LoginUser(string userName, string password);
    }

    public class AccountADMembership : IADMembership
    {
        private readonly MembershipProvider _provider;

        public AccountADMembership()
            : this(null)
        {
        }

        public AccountADMembership(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public bool VerifyRole(Models.Login.LoginViewModel user, string role)
        {
            bool result = false;
            using (NuevoDbContext db = new NuevoDbContext())
            {


                Role objRole = db.Roles.Where(a => role.Contains(a.NameRol.Trim())).FirstOrDefault();
                //linea recién agregada 06-08-2020-->
                if (objRole == null) { throw new ArgumentException("El Rol del Usuario no se encuentra en la Lista de Roles Admitidos", role); };
                // Aquí se produce la excepción
                if (objRole.DNRol != null)
                {
                    if (ValidateUserRole(user.User, objRole))
                        //result = true;
                        result = ValidateUserRole(user.User, objRole);

                }
            }
            return result;
            //return true;
        }

        public bool ValidateUser(string userName, string password)
        {
            //if (String.IsNullOrEmpty(userName)) throw new ArgumentException("The value cannot be null or empty.", "userName");
            //if (String.IsNullOrEmpty(password)) throw new ArgumentException("The value cannot be null or empty.", "password");

            return LoginUser(userName, password);
        }

        public bool LoginUser(string userName, string password)
        {
            //if (String.IsNullOrEmpty(userName)) throw new ArgumentException("The value cannot be null or empty.", "userName");
            //if (String.IsNullOrEmpty(password)) throw new ArgumentException("The value cannot be null or empty.", "password");

            ADMembership adHelper = new ADMembership();
            return adHelper.LoginUser(userName, password);
        }



        public bool ValidateUserRole(string userName, Role role)
        {
            bool result = false;
            //if (String.IsNullOrEmpty(userName)) throw new ArgumentException("The value cannot be null or empty", "userName");

            ADMembership adHelper = new ADMembership();

            List<string> list = adHelper.GetUserRoles(userName, role.Aplication, role.System);

            if (list.Contains(role.DNRol))
                result = true;

            return result;
        }
    }

}