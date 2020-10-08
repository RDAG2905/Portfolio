using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    [RoutePrefix("error")]
    public class ErrorController : Controller
    {
        // GET: Error
        [Route("")]
        public ActionResult GenericError()
        {
            return View("GenericError");
        }

        [Route("internalError")]
        public ActionResult InternalError()
        {
            return View("InternalError");
        }

        [Route("notFound")]
        public ActionResult NotFound()
        {
            return View("NotFound");
        }

        [Route("accessDenied")]
        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public ActionResult getTime()
        {
            if (!String.IsNullOrEmpty((string)Session["HorarioUltimaPeticion"]))
            {

            var url =  Request.Url;
            var HorarioUltimaPeticion = Convert.ToDateTime((string)Session["HorarioUltimaPeticion"]);
            var HoraActual = DateTime.Now.ToLocalTime();
            //var requestTime = int.Parse(HorarioUltimaPeticion);
            var diferencia = HoraActual.Subtract(HorarioUltimaPeticion);
            var minutos = diferencia.Minutes;
               
            var segundos = diferencia.Seconds;
                if (segundos > 20)
                {
                    return Json("Ha transcurrido mucho tiempo de inactividad " + url );
                }

                return Json("Han transcurrido : " + minutos + " - Minutos" + segundos + " - Segundos " + " url : " + url);
            }
            return Json("La hora Es nula o está vacia");
        }
    }
}