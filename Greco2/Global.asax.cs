using Greco2.Model;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Greco2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //public void Application_AcquireRequestState() {
        //    HttpCookie token = Request.Cookies["Abra"];

        //    if (HttpContext.Current.Session != null && token != null)
        //    {
        //        string tokenData = token.Value;
        //        FormsAuthenticationTicket antiforgery = FormsAuthentication.Decrypt(tokenData);
        //        //var t = HttpContext.Current.Session["token"].ToString() + HttpContext.Current.Session.SessionID;
        //        if (antiforgery.Name != HttpContext.Current.Session["token"].ToString() + HttpContext.Current.Session.SessionID /*(string)Session["token"]*/)
        //        {
        //            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        //            FormsAuthentication.SignOut();
        //        }


        //    }
        //}


        public void Application_AcquireRequestState()
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["HorarioUltimaPeticion"] != null)
            {
                var HorarioUltimaPeticion = Convert.ToDateTime((string)Session["MinutosDeInactividad"]);
                var HoraActual = DateTime.Now.ToLocalTime();
                //var requestTime = int.Parse(HorarioUltimaPeticion);
                var diferencia = HoraActual.Subtract(HorarioUltimaPeticion);
                var minutos = diferencia.Minutes;
                var segundos = diferencia.Seconds;
                if (segundos >= 21)
                {
                    new RedirectResult("~/Home/AlertSessionEnd");
                }

            }
        }

        public void Application_PostAuthenticateRequest()
        {
           
                HttpCookie cookie = Request.Cookies["TicketUsuario"];
            if (cookie != null)
            {
                //Ya hemos pasado por login
                String datos = cookie.Value;
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(datos);

                if (ticket != null && !ticket.Expired)
                {
                    String role = ticket.UserData;
                    String username = ticket.Name;
                    GenericIdentity identidad = new GenericIdentity(username);

                    //Creamos usuario genérico 
                    GenericPrincipal usuario = new GenericPrincipal(identidad, new String[] { role });

                    HttpContext.Current.User = usuario;

                    //FormsAuthentication.RenewTicketIfOld(ticket);
                    //var exp = ticket.Expiration;
                    //if (HttpContext.Current.Session != null && HttpContext.Current.Session["HorarioUltimaPeticion"] != null)
                    //{
                    //    var HorarioUltimaPeticion = (DateTime)HttpContext.Current.Session["HorarioUltimaPeticion"];
                    //    var HoraActual = DateTime.Now.ToLocalTime();
                    //    var diferencia = HoraActual.Subtract(HorarioUltimaPeticion);
                    //    var minutos = diferencia.Minutes;
                    //}

                }
                else
                {
                    //new RedirectResult("~/Home/SesionCulminada");
                    if (Request.Url.LocalPath.Contains("Edit") || Request.Url.LocalPath.Contains("uardar") || Request.Url.LocalPath.Contains("rear") || Request.Url.LocalPath.Contains("CulminadaJson") || Request.Url.LocalPath.Contains("GetNuevaDenuncia") || Request.Url.LocalPath.Contains("Buscar") || Request.Url.LocalPath.Contains("Get"))//
                    {
                        
                        new RedirectResult("~/Home/AlertSessionEnd");
                        //Response.Redirect("~/Home/SesionCulminadaJson");

                    }
                    else
                    {
                        new RedirectResult("~/Home/SesionCulminada");
                    }

                }


            }

        }

        protected void Application_Error()
        {
            Exception ex = this.Server.GetLastError();
            var url = Request.Url.ToString();
            var user = HttpContext.Current.User.Identity.Name;
            ErrorLogger log = new ErrorLogger(ex,url,user);
            using (NuevoDbContext context = new NuevoDbContext()) {
                var e = new LogErrorDto();
                e.Fecha = DateTime.Now;
                e.Error = ex.Message;
                //if (ex.) {
                //    var errorMessages = ex.EntityValidationErrors
                //        .SelectMany(x => x.ValidationErrors)
                //        .Select(x => x.ErrorMessage);
                //}
                e.UrlRequest = url;
                e.UserId = user;
                e.ErrorDetallado = ex.ToString();
                context.Add(e);
                context.SaveChanges();
            }

        }
    }
}
