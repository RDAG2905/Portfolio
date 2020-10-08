using Greco2.Model;
using Greco2.Models.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Greco2.Controllers
{
    public class CheckerController : Controller
    {
        // GET: Checker
        public ActionResult Index()
        {
            return View();
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult GetRenewModal()
        {
            return PartialView("RenewModal");
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        public ActionResult SetParameters() {

            using (NuevoDbContext context = new NuevoDbContext()) {
                var parámetros = context.Parametros.ToList();
                var destinatarios = context.Emails.ToList();
                var urlECM = parámetros.Where(x => String.Equals(x.KEY.Trim(), "ECMUrl")).FirstOrDefault().VALUE;
                var pathRootArchivos = parámetros.Where(x => String.Equals(x.KEY.Trim(), "PathRootArchivos")).FirstOrDefault().VALUE;
                var emailTEAM = parámetros.Where(x => String.Equals(x.KEY.Trim(), "EmailTEAM")).FirstOrDefault().VALUE;
                Session["urlECM"] = urlECM;
                Session["pathRootArchivos"] = pathRootArchivos;
                Session["emailTEAM"] = emailTEAM;
                Session["destinatarios"] = destinatarios;
            }
            return Json("<div class='alert alert-info' style='inline-block'>Carga de Parámetros OK</div>");
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        public ActionResult SetParametersReporteDenuncias()
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {
                var parámetros = context.Parametros.ToList();
                var destinatarios = context.Emails.ToList(); 
                var emailTEAM = parámetros.Where(x => String.Equals(x.KEY.Trim(), "EmailTEAM")).FirstOrDefault().VALUE;
                Session["emailTEAM"] = emailTEAM;
                Session["destinatarios"] = destinatarios;
            }
            return Json("<div class='alert alert-info' style='inline-block'>Carga de Parámetros OK</div>");
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        public ActionResult SetParametersFileList()
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {
                var parámetros = context.Parametros.ToList();
                var pathRootArchivos = parámetros.Where(x => String.Equals(x.KEY.Trim(), "PathRootArchivos")).FirstOrDefault().VALUE;
                var pathStart = parámetros.Where(x => String.Equals(x.KEY.Trim(), "PathStart")).FirstOrDefault().VALUE;
                var urlECM = parámetros.Where(x => String.Equals(x.KEY.Trim(), "ECMUrl")).FirstOrDefault().VALUE;
            
                Session["pathRootArchivos"] = pathRootArchivos;
                Session["pathStart"] = pathStart;
                Session["urlECM"] = urlECM;
            }
            return Json("<div class='alert alert-info' style='inline-block'>Carga de Parámetros OK</div>");
        }

        //[AllowAnonymous]
        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        [HttpPost]
        public ActionResult Renovar()
        {

            HttpCookie cookie = Request.Cookies["TicketUsuario"];
            if (cookie != null)
            {
                //Ya hemos pasado por login
                String datos = cookie.Value;
                FormsAuthenticationTicket Ticket = FormsAuthentication.Decrypt(datos);

                if (Ticket != null && !Ticket.Expired)
                {
                    FormsAuthenticationTicket Ticket2 = Ticket;

                    //if (FormsAuthentication.SlidingExpiration) { 
                    //Ticket2 = FormsAuthentication.RenewTicketIfOld(Ticket);
                    Ticket2 = new FormsAuthenticationTicket(1,Ticket.Name, DateTime.Now, DateTime.Now.AddMinutes(60),false,Ticket.UserData);
                    String role = Ticket2.UserData;
                    String username = Ticket2.Name;
                    GenericIdentity identidad = new GenericIdentity(username);

                    //Creamos usuario genérico 
                    GenericPrincipal usuario = new GenericPrincipal(identidad, new String[] { role });

                    System.Web.HttpContext.Current.User = usuario;

                    //if (Ticket2 != Ticket)
                    //{
                        // establece la cookie de autenticación con los nuevos valores y el tiempo de vencimiento

                        string hash = FormsAuthentication.Encrypt(Ticket2);
                        HttpCookie newCookie = new HttpCookie("TicketUsuario",hash);
                   
                    //if (Ticket2.IsPersistent)
                        //  cookie.Expires = Ticket2.Expiration;
                        //cookie.Value = hash;
                        //cookie.HttpOnly = true;
                        //cookie.Secure = FormsAuthentication.RequireSSL;
                        //Response.Cookies.Add(cookie);

                    Response.Cookies.Add(newCookie);
                    
                    return Json("ticket actualizado");


                }
                else
                {
                    return Json("El ticket ha expirado");
                }

            }
            else
            {
                return Json("El ticket es nulo");
            }
            //return Json("No se pudo acceder a los datos de la Cookie");
        }

        
    }  

}