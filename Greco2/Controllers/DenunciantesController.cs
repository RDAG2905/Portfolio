using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Denunciante;
using Greco2.Models.Grupo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    
    public class DenunciantesController : Controller
    {
        // GET: Denunciantes

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public ActionResult Index()
        {
            return View();
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public PartialViewResult GetGrupoDenunciantesView() {
            
            return PartialView("GrupoDenunciantes");
        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public ActionResult GetNroDocumentos()
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var denunciantes = context.Denunciantes.ToList();
                var listaDni = from e in denunciantes
                               select new
                               {
                                   e.NroDocumento
                               };
                return Json(listaDni);
            }
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public async Task<ActionResult> GetFilaDenunciante(int? id)
        {

            using (NuevoDbContext context = new NuevoDbContext())
            {
                var denunciante = await context.Denunciantes.Where(d => d.DenuncianteId == id).FirstOrDefaultAsync();
                return Json(denunciante);
                //return PartialView("FilaGrupoDenunciantesNuevo", denunciante);
            }

        }

        public void crearGrupo(List<List<string>> lista)
        {





        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        public bool existenDuplicados(List<List<string>> documentosAValidar, List<string> duplicados) {

            //foreach (var elem in documentosAValidar)
            //    {

            //    var lista = documentosAValidar;
            //    lista = documentosAValidar.where(e=>e != elem).ToList();
            //    foreach (var item in lista) {
            //        if ((item[1] == elem[1])
            //           && item[2] == elem[2]
            //           && item[3] == elem[3]) {
            //            duplicados.Add("posición: "+ item[0] + " y "+ elem[0]);
            //        }
            //    }

            //}
            //return (duplicados.Count() > 0);
            return false;
        }

        //Queda en espera hasta la revisión con el usuario
        //[ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AgregarNuevosDenunciantes(List<List<string>> lista, int idGrupo)
        //{
        //    string dni = "";
        //    var dniExistentes = new List<List<string>>();

        //    List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
        //    var grupo = new GrupoDto();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        foreach (var item in lista)
        //        {

        //            DenuncianteDto nuevoDenunciante = new DenuncianteDto();

        //            if (String.IsNullOrEmpty(item[4]))
        //            {
        //                if (!String.IsNullOrEmpty(item[3]))
        //                {

        //                    dni = Convert.ToString(item[3]);
        //                    var nombre = item[1];
        //                    var apellido = item[2];
        //                    if (context.Denunciantes.Where(d => String.Equals(d.NroDocumento.Trim(), dni.Trim())
        //                                                   && (!d.nombre.Contains(nombre)
        //                                                   || !d.apellido.Contains(apellido))).Any())
        //                    {
        //                        dniExistentes.Add(item);
        //                    }

        //                    nuevoDenunciante.NroDocumento = Convert.ToString(item[3]);
        //                }

        //                nuevoDenunciante.nombre = item[1].ToUpper();
        //                nuevoDenunciante.apellido = item[2].ToUpper();


        //                context.Add(nuevoDenunciante);
        //                await context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                var idExistente = Convert.ToInt32(item[4]);
        //                nuevoDenunciante = await context.Denunciantes.Where(d => d.DenuncianteId == idExistente).FirstOrDefaultAsync();
        //            }
        //            denunciantes.Add(nuevoDenunciante);
        //        }

        //        foreach (var item in denunciantes) {
        //            var fechaCreacion = DateTime.Now;
        //            context.Database
        //            .ExecuteSqlCommand("Insert into GrupoDenunciantesRel values(@GrupoDto_Id,@DenuncianteId,@fechaCreacion)"
        //            , new SqlParameter("@GrupoDto_Id", idGrupo)
        //            , new SqlParameter("@DenuncianteId", item.DenuncianteId)
        //            , new SqlParameter("@fechaCreacion", fechaCreacion));
        //        }
        //    }

        //    return RedirectToAction("TraerGrupoDenunciantes", "Denunciantes", new { grupoId = idGrupo });
        //}

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarNuevosDenunciantes(List<List<string>> lista, int idGrupo)
        {
           
            var dniExistentes = new List<List<string>>();

            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            var grupo = new GrupoDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in lista)
                {
                    DenuncianteDto nuevoDenunciante = new DenuncianteDto();

                    if (String.IsNullOrEmpty(item[4]))
                    {

                        nuevoDenunciante.nombre = item[1].ToUpper();
                        nuevoDenunciante.apellido = item[2].ToUpper();
                        nuevoDenunciante.NroDocumento = item[3];

                        var denuncianteExistente = await context.Denunciantes.Where(d => String.Equals(d.NroDocumento.Trim(), nuevoDenunciante.NroDocumento.Trim())
                                                                               && String.Equals(d.nombre.Trim(), nuevoDenunciante.nombre.Trim())
                                                                               && String.Equals(d.apellido.Trim(), nuevoDenunciante.apellido.Trim()))
                                                                                .FirstOrDefaultAsync();
                        if (denuncianteExistente == null)
                        {
                            context.Add(nuevoDenunciante);
                            await context.SaveChangesAsync();
                            denunciantes.Add(nuevoDenunciante);
                        }
                        else
                        {
                            denunciantes.Add(denuncianteExistente);
                        }
                    }
                    else
                    {
                        var idExistente = Convert.ToInt32(item[4]);
                        nuevoDenunciante = await context.Denunciantes.Where(d => d.DenuncianteId == idExistente).FirstOrDefaultAsync();
                        denunciantes.Add(nuevoDenunciante);
                    }                   
                }

                foreach (var item in denunciantes)
                {
                    var fechaCreacion = DateTime.Now;
                    context.Database
                    .ExecuteSqlCommand("Insert into GrupoDenunciantesRel values(@GrupoDto_Id,@DenuncianteId,@fechaCreacion)"
                    , new SqlParameter("@GrupoDto_Id", idGrupo)
                    , new SqlParameter("@DenuncianteId", item.DenuncianteId)
                    , new SqlParameter("@fechaCreacion", fechaCreacion));
                }

                //auditoria
                foreach (var item in denunciantes)
                {
                    var fechaCreacionGrupoAuditoria = DateTime.Now;
                    var objetoModificado = "GRUPO";
                    var descripcionObjeto = "Se agregó al Denunciante con Id : " + item.DenuncianteId + " al grupo Id : " + idGrupo.ToString();
                    var valorAnterior = "";
                    var valorActual = "";
                    var usuario = System.Web.HttpContext.Current.User.Identity.Name;

                    context.Database
                    .ExecuteSqlCommand("Insert into tCommonChLogger values(@fechaCreacionGrupo,@objetoModificado,@descripcionObjeto,@valorAnterior,@valorActual,@usuario,@GrupoDto_Id)"
                    , new SqlParameter("@fechaCreacionGrupo", fechaCreacionGrupoAuditoria)
                    , new SqlParameter("@objetoModificado", objetoModificado)
                    , new SqlParameter("@descripcionObjeto", descripcionObjeto)
                    , new SqlParameter("@valorAnterior", valorAnterior)
                    , new SqlParameter("@valorActual", valorActual)
                    , new SqlParameter("@usuario", usuario)
                    , new SqlParameter("@GrupoDto_Id",idGrupo));
                    

                }
            }
            return RedirectToAction("TraerGrupoDenunciantes", "Denunciantes", new { grupoId = idGrupo });
        }



        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
        public ActionResult TraerGrupoDenunciantes(int? grupoId)
        {
            GrupoSP grupo = new GrupoSP();
            if (grupoId != null || grupoId > 0)
            {

                using (NuevoDbContext context = new NuevoDbContext())
                {
                    List<DenuncianteGrupoSP> grupoDenunciantes = context.Database
                                                            .SqlQuery<DenuncianteGrupoSP>("GetGrupoDenunciantesPorIdGrupo @grupoId", new SqlParameter("@grupoId", grupoId))
                                                            .ToList();

                    //grupo.Id = grupoDenunciantes.First().GrupoDto_Id;
                    grupo.Id = int.Parse(grupoId.Value.ToString());
                    grupo.grupoDenunciantes = grupoDenunciantes;
                    //var idPrincipal = context.Grupos.Find(grupoId).IdDenunciantePrincipal;
                    //grupo.DenunciantePrincipal = context.Denunciantes.Find(idPrincipal);
                }

            }

           
            return PartialView("ListGrupoDenunciantesSP", grupo);

            //return PartialView("GrupoDenunciantes");
        }

        private List<string> verificarDuplicados(List<string> lista) {
            var duplicados = new List<string>();
            var cantidad = lista.Count;
            for (int i = 0; i < cantidad; i++) {
                var actual = lista[0];
                var primero = lista.ElementAt(0);
                lista.RemoveAt(0);
                var lista2 = lista;
                if (lista2.Any(x => String.Equals(primero.Trim(), x.Trim()))){
                    duplicados.Add(primero);
                };
                lista = lista2;
                //duplicados.Count();
               
            }
            return duplicados;
          
        }

        private List<List<string>> verificarDatosDuplicados(List<List<string>> lista)
        {
            var datosDuplicados = new List<List<string>>();
            var cantidad = lista.Count;
            for (int i = 0; i < cantidad; i++)
            {
                var actual = lista[0];
                var primero = lista.ElementAt(0);
                lista.RemoveAt(0);
                var lista2 = lista;
                if (lista2.Any(x => String.Equals(primero[1].Trim().ToUpper(), x[1].Trim().ToUpper()) && String.Equals(primero[2].Trim().ToUpper(), x[2].Trim().ToUpper())))
                {
                    datosDuplicados.Add(primero);
                };
                lista = lista2;
                //duplicados.Count();

            }
            return datosDuplicados;

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> validarDniDenunciantes(List<List<string>> documentosAValidar)/*(string nombre, string apellido, int? dni)*/
        {

            List<string> documentos = new List<string>();
            foreach (var item in documentosAValidar)
            {
                if (!String.IsNullOrEmpty(item[3]))
                {
                    documentos.Add(item[3]);
                }
            }
            var cantidadDuplicados = verificarDuplicados(documentos);
            if (cantidadDuplicados.Any())
            {
                //return JavaScript("<script>toastr.error('Verifique los datos ingresados<br/>Existen Nros. de Identidad duplicados. Total de casos :  " + cantidadDuplicados.Count + "')</script>");
                return Json("<div class='alert alert-danger text-center'>Verifique los datos ingresados. Existen Números de Identidad duplicados. Total de casos : " + cantidadDuplicados.Count + "</div>");
            }
            //
            var datosDenunciantes = new List<List<string>>();
            foreach (var item in documentosAValidar)
            {
                if (String.IsNullOrEmpty(item[4]) && String.IsNullOrEmpty(item[3]))
                {
                    datosDenunciantes.Add(item);
                }
            }

            var cantidadDatosDuplicados = verificarDatosDuplicados(datosDenunciantes);
            if (cantidadDatosDuplicados.Any())
            {
                //return JavaScript("<script>toastr.error('Verifique los datos ingresados<br/>Existen Denuciantes duplicados. Total de casos :  " + cantidadDatosDuplicados.Count + "')</script>");
                return Json("<div class='alert alert-danger text-center'>Verifique los datos ingresados. Existen Denunciantes con el mismo nombre y apellido. Los mismos no poseen dni por lo cual no se pueden diferenciar. El grupo no puede contener duplicados . Total de casos :  " + cantidadDatosDuplicados.Count  + "<div>"); /*Total de casos :  " + cantidadDatosDuplicados.Count*/
            }

            //
            
            var dniExistentes = new List<List<string>>();
            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            var grupo = new GrupoDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in documentosAValidar)
                {
                    DenuncianteDto nuevoDenunciante = new DenuncianteDto();

                    if (String.IsNullOrEmpty(item[4]))
                    {
                        if (!String.IsNullOrEmpty(item[3]))
                        {
                           
                            var nombre = item[1];
                            var apellido = item[2];
                            var dni = item[3];

                            //if (context.Denunciantes.Where(d => String.Equals(d.NroDocumento.Trim(), dni.Trim())
                            //                               && (!d.nombre.Contains(nombre)
                            //                               || !d.apellido.Contains(apellido))).Any())

                            if (context.Denunciantes.Any(d => String.Equals(d.NroDocumento.Trim(), dni.Trim())))
                                                           //&& (!String.Equals(d.nombre.Trim(), nombre.Trim())
                                                           //|| !String.Equals(d.apellido.Trim(), apellido.Trim()))))
                            {
                                dniExistentes.Add(item);
                            }
                        }
                    }
                }
            }
            if (dniExistentes.Count > 0)
            {
                return PartialView("WarningDniExistentes", dniExistentes);
            }
            else {
                return JavaScript("<script>validarListaDni();</script>");
            }
            

        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> validarDniDenunciantesNuevos(List<List<string>> documentosAValidar)/*(string nombre, string apellido, int? dni)*/
        {
          
            List<string> documentos = new List<string>();
            foreach (var item in documentosAValidar)
            {
                if (!String.IsNullOrEmpty(item[3]))
                {
                    documentos.Add(item[3]);
                }
            }
            var cantidadDuplicados = verificarDuplicados(documentos);
            if (cantidadDuplicados.Any())
            {
                return Json("<div class='alert alert-danger text-center'>Verifique los datos ingresados. Existen Números de Identidad duplicados. Total de casos : " + cantidadDuplicados.Count + "</div>");
                //return JavaScript("<script>toastr.error('Verifique los datos ingresados<br/>Existen Nros. de Identidad duplicados. Total de casos :  " + cantidadDuplicados.Count + "')</script>");
            }

            //
            var datosDenunciantes = new List<List<string>>();
            foreach (var item in documentosAValidar)
            {
                if (String.IsNullOrEmpty(item[4]) && String.IsNullOrEmpty(item[3]))
                {
                    datosDenunciantes.Add(item);
                }
            }

            var cantidadDatosDuplicados = verificarDatosDuplicados(datosDenunciantes);
            if (cantidadDatosDuplicados.Any())
            {
                return Json("<div class='alert alert-danger text-center'>Verifique los datos ingresados. Existen Denunciantes con el mismo nombre y apellido. Los mismos no poseen dni por lo cual no se pueden diferenciar. El grupo no puede contener duplicados . Total de casos :  " + cantidadDatosDuplicados.Count + "<div>"); /*Total de casos :  " + cantidadDatosDuplicados.Count*/
                //return JavaScript("<script>toastr.error('Verifique los datos ingresados<br/>Existen Denuciantes duplicados. Total de casos :  " + cantidadDatosDuplicados.Count + "')</script>");
                //return Json("Existen Denunciantes con nombre y apellido duplicados. Los mismos no poseen dni"); /*Total de casos :  " + cantidadDatosDuplicados.Count*/
            }

            //

            var dniExistentes = new List<List<string>>();
            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            var grupo = new GrupoDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in documentosAValidar)
                {
                    DenuncianteDto nuevoDenunciante = new DenuncianteDto();

                    if (String.IsNullOrEmpty(item[4]))
                    {
                        if (!String.IsNullOrEmpty(item[3]))
                        {

                            var nombre = item[1];
                            var apellido = item[2];
                            var dni = item[3];

                            
                            if (context.Denunciantes.Any(d => String.Equals(d.NroDocumento.Trim(), dni.Trim())))
                            
                            {
                                dniExistentes.Add(item);
                            }
                        }
                    }
                }
            }
            if (dniExistentes.Count > 0)
            {
                return PartialView("WarningDniExistentes", dniExistentes);
            }
            else
            {
                return JavaScript("<script>validarNuevosDenunciantesGrupo();</script>");
            }


        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> validarListaDni(List<List<string>> documentosAValidar)
        {
           
            var dniExistentes = new List<List<string>>();

            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            var grupo = new GrupoDto();
            GrupoSP grupoSP = new GrupoSP();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in documentosAValidar)
                {
                    DenuncianteDto nuevoDenunciante = new DenuncianteDto();

                    if (String.IsNullOrEmpty(item[4]))
                    {
                       
                        nuevoDenunciante.nombre = item[1].ToUpper();
                        nuevoDenunciante.apellido = item[2].ToUpper();
                        nuevoDenunciante.NroDocumento = item[3];

                        var denuncianteExistente = await context.Denunciantes.Where(d => String.Equals(d.NroDocumento.Trim(), nuevoDenunciante.NroDocumento.Trim())
                                                                               && String.Equals(d.nombre.Trim(), nuevoDenunciante.nombre.Trim())
                                                                               && String.Equals(d.apellido.Trim(), nuevoDenunciante.apellido.Trim()))
                                                                                .FirstOrDefaultAsync();
                        if (denuncianteExistente == null)
                        {
                            context.Add(nuevoDenunciante);
                            await context.SaveChangesAsync();
                            denunciantes.Add(nuevoDenunciante);
                        }
                        else {
                            denunciantes.Add(denuncianteExistente);
                        }

                    }
                    else
                    {
                        var idExistente = Convert.ToInt32(item[4]);
                        nuevoDenunciante = await context.Denunciantes.Where(d => d.DenuncianteId == idExistente).FirstOrDefaultAsync();
                        denunciantes.Add(nuevoDenunciante);
                    }
                    //denunciantes.Add(nuevoDenunciante);
                }
               

                //grupo.grupoDenunciantes = denunciantes; comentado 12/05/2020
                grupo.IdDenunciantePrincipal = denunciantes[0].DenuncianteId;
                context.Add(grupo);
                await context.SaveChangesAsync();
                //agregado 12/05/2020
                var idGrupo = grupo.Id;
                foreach (var item in denunciantes)
                {
                    var fechaCreacion = DateTime.Now;
                    context.Database
                    .ExecuteSqlCommand("Insert into GrupoDenunciantesRel values(@GrupoDto_Id,@DenuncianteId,@fechaCreacion)"
                    , new SqlParameter("@GrupoDto_Id", idGrupo)
                    , new SqlParameter("@DenuncianteId", item.DenuncianteId)
                    , new SqlParameter("@fechaCreacion", fechaCreacion));
                }
                List<DenuncianteGrupoSP> grupoDenunciantes = context.Database
                                                            .SqlQuery<DenuncianteGrupoSP>("GetGrupoDenunciantesPorIdGrupo @grupoId", new SqlParameter("@grupoId", idGrupo))
                                                            .ToList();
                
                grupoSP.Id = grupoDenunciantes.First().GrupoDto_Id;
                grupoSP.grupoDenunciantes = grupoDenunciantes;

                // auditoria

                var fechaCreacionGrupoAuditoria = DateTime.Now;
                var objetoModificado = "GRUPO";
                var descripcionObjeto = "Se ha creado al grupoId : " + grupoSP.Id.ToString();
                var valorAnterior = "";
                var valorActual = "Cantidad de Integrantes : " + grupoSP.grupoDenunciantes.Count.ToString();
                var usuario = System.Web.HttpContext.Current.User.Identity.Name;

                context.Database
                .ExecuteSqlCommand("Insert into tCommonChLogger values(@fechaCreacionGrupo,@objetoModificado,@descripcionObjeto,@valorAnterior,@valorActual,@usuario,@GrupoDto_Id)"
                , new SqlParameter("@fechaCreacionGrupo", fechaCreacionGrupoAuditoria)
                , new SqlParameter("@objetoModificado", objetoModificado)
                , new SqlParameter("@descripcionObjeto", descripcionObjeto)
                , new SqlParameter("@valorAnterior", valorAnterior)
                , new SqlParameter("@valorActual", valorActual)
                , new SqlParameter("@usuario", usuario)
                , new SqlParameter("@GrupoDto_Id", grupoSP.Id));

            }
            return PartialView("ListGrupoDenunciantesSP", grupoSP);
            //return PartialView("ListGrupoDenunciantes", grupo);

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public async Task<ActionResult> validarListaDniAgregarAlGrupo(List<List<string>> documentosAValidar)
        {


            //int? dni = null;
            string dni = "";
            var dniExistentes = new List<List<string>>();

            List<DenuncianteDto> denunciantes = new List<DenuncianteDto>();
            var grupo = new GrupoDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                foreach (var item in documentosAValidar)
                {
                    DenuncianteDto nuevoDenunciante = new DenuncianteDto();

                    if (String.IsNullOrEmpty(item[4]))
                    {
                        if (!String.IsNullOrEmpty(item[3]))
                        {
                           
                            dni = Convert.ToString(item[3]);
                            var nombre = item[1];
                            var apellido = item[2];
                                                       if (context.Denunciantes.Where(d => String.Equals(d.NroDocumento.Trim(),dni.Trim())
                                                           && (!d.nombre.Contains(nombre)
                                                           || !d.apellido.Contains(apellido))).Any())
                        {
                            dniExistentes.Add(item);
                        }
                       
                    }
                    nuevoDenunciante.nombre = item[1].ToUpper();
                        nuevoDenunciante.apellido = item[2].ToUpper();


                        context.Add(nuevoDenunciante);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var idExistente = Convert.ToInt32(item[4]);
                        nuevoDenunciante = await context.Denunciantes.Where(d => d.DenuncianteId == idExistente).FirstOrDefaultAsync();
                    }
                    denunciantes.Add(nuevoDenunciante);
                }
                if (dniExistentes.Count > 0)
                {
                    return Json("Verifique los Nros de Documentos : " + dniExistentes);
                }

                grupo.grupoDenunciantes = denunciantes;
                grupo.IdDenunciantePrincipal = denunciantes[0].DenuncianteId;
                context.Add(grupo);
                await context.SaveChangesAsync();
            }

            return PartialView("ListGrupoDenunciantes", grupo);

        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        public async Task<ActionResult> BuscarDenunciantesGrupoDenuncia(string nombre, string apellido, string dni)
        {
            List<DenuncianteDto> lista = new List<DenuncianteDto>();

            //using (NuevoDbContext context = new NuevoDbContext())
            //{
            //    if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(dni))
            //    {
            //        lista = await context.Denunciantes.Where(m => String.Equals(m.NroDocumento,dni)
            //                                                   && m.apellido.Contains(apellido)
            //                                                   && m.nombre.Contains(nombre)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(apellido))
            //    {
            //        lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)
            //                                                   && m.nombre.Contains(nombre)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(dni))
            //    {

            //        lista = await context.Denunciantes.Where(m => m.nombre.Contains(nombre)
            //                                                   && String.Equals(m.NroDocumento, dni)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(apellido) && !String.IsNullOrEmpty(dni))
            //    {

            //        lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)
            //                                                   && String.Equals(m.NroDocumento, dni)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(nombre))
            //    {
            //        lista = await context.Denunciantes.Where(m => m.nombre.Contains(nombre)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(apellido))
            //    {
            //        lista = await context.Denunciantes.Where(m => m.apellido.Contains(apellido)).ToListAsync();
            //    }
            //    else
            //    if (!String.IsNullOrEmpty(dni))
            //    {
            //        lista = await context.Denunciantes.Where(m => String.Equals(m.NroDocumento,dni)).ToListAsync();

            //    }
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
                else
                {
                    lista = await context.Denunciantes.ToListAsync();
                }

            }
            lista = lista.OrderBy(x => x.apellido).ToList();
            return PartialView("DenunciantesGrupo", lista);

        }
        
        
            
    

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]

        public ActionResult ExisteElDniDG(string dni, int id)
        {
            if (!String.IsNullOrEmpty(dni))
            {
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    var existe = context.Denunciantes.Any(d => String.Equals(d.NroDocumento, dni) && d.DenuncianteId != id);
                    if (existe)
                    {
                        return Json("<div class='alert alert-danger text-center'>Ya existe un denunciante con el mismo Nro. de Documento</div>");
                    }
                }
            }
            return JavaScript("guardarDenuncianteGrupoEdicion()");
            //return Json("<div class='alert alert-success text-center'>dni OK</div>");

        }


        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarDenuncianteDenuncia(int? id)
        {
            if (id > 0)
            {
                DenuncianteDto denunciante = new DenuncianteDto();
                using (NuevoDbContext context = new NuevoDbContext())
                {
                    denunciante = await context.Denunciantes.Where(d => d.DenuncianteId == id).FirstOrDefaultAsync();
                }
                return PartialView("DenuncianteEdicion", denunciante);
            }
            else {
                return Json("<div class='alert alert-danger text-center'>No existe ningún elemento a Editar</div>");
            }
        }

        //[HttpPost]
        //public async Task<ActionResult> EditarDenuncianteDenunciaJson(int? id)
        //{
        //    DenuncianteDto denunciante = new DenuncianteDto();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        denunciante = await context.Denunciantes.Where(d => d.DenuncianteId == id).FirstOrDefaultAsync();
        //    }
        //    return Json(denunciante);
        //}

        [HttpPost]
        public ActionResult Guardar(string nombre, string apellido, int? dni)
        {
            var denunciante = new DenuncianteDto();
            denunciante.nombre = nombre.ToUpper();
            denunciante.apellido = apellido.ToUpper();
            denunciante.NroDocumento = dni.ToString();
            DenuncianteCommandService dcs = new DenuncianteCommandService();
            DenuncianteQueryService dqs = new DenuncianteQueryService();
            //var maximo = dqs.getDenunciantes().Max(d => d.DenuncianteId);
            //denunciante.DenuncianteId = maximo + 1;          
            int? newId = dcs.createDenunciante(denunciante);
            if (newId.HasValue)
            {
                DenuncianteDto newDenunciante = dqs.GetDenuncianteById((int)newId);
                DenuncianteModelView dm = new DenuncianteModelView();
                dm.denunciante = newDenunciante;
                return PartialView("DenuncianteResult", dm);
            }
            else
            {
                return PartialView("SinResultado");
            }


        }

        [HttpPost]
        public async Task<ActionResult> BuscarDenunciante(string nombre, string apellido, string dni)
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
                else
                {
                    lista = await context.Denunciantes.ToListAsync();
                }

            }
            lista = lista.OrderBy(x => x.apellido).ToList();
            return PartialView("Denunciantes", lista);
        }

        [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> GetDenuncianteJson(int? id)
        {
            var denunciante = new DenuncianteDto();
            using (NuevoDbContext context = new NuevoDbContext())
            {
                denunciante = context.Denunciantes.Where(e => e.DenuncianteId == id).FirstOrDefault();
            }
            return Json(denunciante);
        }

    }
}