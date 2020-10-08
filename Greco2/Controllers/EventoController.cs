using Greco2.Model;
using Greco2.Models.Denuncia;
using Greco2.Models.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    public class EventoController : Controller
    {
        // GET: Evento
        public ActionResult Index()
        {
            return View();
        }

       


        public ActionResult CrearEvento([Bind(Include = "DenunciaId,Fecha,tipoEventoId,CONTESTADO,SOLUCIONADO,REQUERIMIENTOINFORME,observacion,ResIntId")]EventoDto evento)
        {
            DenunciaDto denuncia = new DenunciaDto();

            using (NuevoDbContext context = new NuevoDbContext())
            {
                denuncia = context.Denuncias.Where(x => x.DenunciaId == evento.DenunciaId).FirstOrDefault();
            }
            var esUnEventoVálido = true;
            var mensajeFechaInvalida1 = "";
            var mensajeFechaInvalida2 = "";
            var mensajeResponsableInvalido = "";
            var mensajeTipoDeEventoInvalido = "";

            if (evento.Fecha == null)
            {
                mensajeFechaInvalida1 = "La Fecha De Vencimiento es Inválida</br>";
                esUnEventoVálido = false;
            }
            if (denuncia.FSELLOCIA > evento.Fecha)
            {
                mensajeFechaInvalida2 = "La Fecha de vencimiento no puede ser anterior a la Fecha de Notificación del Reclamo </br>";
                esUnEventoVálido = false;
            }
            if (evento.ResIntId == null)
            {
                mensajeResponsableInvalido = "Seleccione un Responsable</br>";
                esUnEventoVálido = false;
            }
            if (!(evento.TipoEventoId > 0))
            {
                mensajeTipoDeEventoInvalido = "Seleccione un Tipo de Evento</br>";
                esUnEventoVálido = false;
            }

            if (esUnEventoVálido) {
                var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                evento.FECHACREACION = DateTime.Now;
                evento.Deleted = false;
                evento.CREATIONPERSON = usuario;
                EventoCommandService evc = new EventoCommandService();
                evc.createEvento(evento,usuario);
                return Json("Evento guardado correctamente");
            }
            else {
                 return Json("<div class='alert alert-danger text-center'>" + mensajeFechaInvalida1 + mensajeFechaInvalida2 + mensajeResponsableInvalido + mensajeTipoDeEventoInvalido + "</div>");     
            }

            


            //var courier = evc.elEventoEsValido(evento);
            //if (courier.elObjetoEsVálido)
            //{

            //}
            //else {
            //    if (courier.mensajes != null && courier.mensajes.Count > 0)
            //    {
            //        courier.mensajes;
            //    }
            //    else {

            //    }
            //}
               

            //if (ModelState.IsValid)
            //{
            //    return JavaScript("<script>toastr.success('guardado correctamente')</script>");
            //}
            //else {
            //    return JavaScript("<script>toastr.error('Existe uno o más parámetros inválidos')</script>");
            //}
        }

        public ActionResult GetEventContent()
        {
            return PartialView("EventoNuevoModal");
        }
    }

   

}