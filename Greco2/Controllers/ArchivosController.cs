using Greco2.Model;
using Greco2.Models.Atributos;
using Greco2.Models.Denuncia;
using Greco2.Models.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    [ActionAuthorize(Roles = "ADMINISTRADOR,ANALISTA,COORDINADOR,GERENTE,ESTUDIO")]
    public class ArchivosController : Controller
    {
        NuevoDbContext _dbContext;

        // GET: Archivos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index()
        {
            //var secureAppSettings = ConfigurationManager.GetSection("secureAppSettings") as NameValueCollection;         
            if (Session["DenunciaActual"] != null)
            {
                var pathRootArchivos = (string)Session["pathRootArchivos"];
                //var pathStart = (string)Session["pathStart"];

                //anti FullPath Manipulation
                var denunciaFolder = (int)Session["DenunciaActual"];
                Regex regex = new Regex(@"(^\d+$)");
                Match match = regex.Match(denunciaFolder.ToString());
                //string folderLocation = secureAppSettings["PathArchivos"] + denunciaFolder;
                string folderLocation = pathRootArchivos + denunciaFolder;

                var path = Path.GetFullPath(folderLocation);
                //if (path.StartsWith(@"D:\MARDOCS", StringComparison.OrdinalIgnoreCase) &&
                if (path.StartsWith(pathRootArchivos, StringComparison.OrdinalIgnoreCase) &&
                    path.EndsWith(denunciaFolder.ToString()) && match.Success)
                {
                    Session["CarpetaActual"] = folderLocation;
                    bool exists = System.IO.Directory.Exists(folderLocation);

                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(folderLocation);

                    }
                }
                else {

                    return Json("</br>Path Inválido");
                }
                //
                //string folderLocation = ConfigurationManager.AppSettings["PathArchivos"] + Session["DenunciaActual"];
                //string folderLocation = secureAppSettings["PathArchivos"] + Session["DenunciaActual"];
                //Session["CarpetaActual"] = folderLocation;
                //bool exists = System.IO.Directory.Exists(folderLocation);

                //if (!exists)
                //{
                //    System.IO.Directory.CreateDirectory(folderLocation);

                //}
                List<ArchivoDto> _Archivos = new List<ArchivoDto>();
                var id = (int)Session["DenunciaActual"];
                using (_dbContext = new NuevoDbContext())
                {
                    
                    _Archivos = _dbContext.Archivos.Where(x => x.DenunciaId == id).OrderByDescending(x => x.fechaCreacion).ToList();
                }
                
                return PartialView("FileList", _Archivos);
            }
            else
            {
                return Json("</br>Carpeta Inexistente");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void ValidarIdentidad() {
            SaveDropzoneJsUploadedFiles();
            var ok = "";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAFTFiles() {
            Guid guid = Guid.NewGuid();
            Session["aftFiles"] = guid + Session.SessionID;
            var envio = (string)Session["aftFiles"];
            return Json(envio/*,JsonRequestBehavior.AllowGet*/);
        }

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Extraemos el contenido en Bytes del ArchivoDto subido.
                var _contenido = new byte[file.ContentLength];
                file.InputStream.Read(_contenido, 0, file.ContentLength);

                // Separamos el Nombre del ArchivoDto de la Extensión.
                int indiceDelUltimoPunto = file.FileName.LastIndexOf('.');
                string _nombre = file.FileName.Substring(0, indiceDelUltimoPunto);
                string _extension = file.FileName.Substring(indiceDelUltimoPunto + 1,
                                    file.FileName.Length - indiceDelUltimoPunto - 1);

                // Instanciamos la clase ArchivoDto y asignammos los valores.
                ArchivoDto _ArchivoDto = new ArchivoDto()
                {
                    Nombre = _nombre,
                    Extension = _extension,
                    Descargas = 0,
                    DenunciaId = (int)Session["DenunciaActual"]
                };

                //try
                //{
                    // Subimos el ArchivoDto al Servidor.
                    var carpetaActual = (string)Session["CarpetaActual"];
                    var pathRoot = (string)Session["pathRootArchivos"];

                    _ArchivoDto.PathRelativo(carpetaActual);
                    _ArchivoDto.SubirArchivo(_contenido);


                    // Guardamos en la base de datos la instancia del ArchivoDto
                    using (_dbContext = new NuevoDbContext())
                    {
                        _dbContext.Archivos.Add(_ArchivoDto);
                        _dbContext.SaveChanges();
                    }
                //}
                //catch (Exception ex)
                //{
                //    // Aquí el código para manejar la Excepción.
                //}
            }

            
            return RedirectToAction("Index");
        }

       
        [ChildActionOnly]
        public ActionResult uploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Extraemos el contenido en Bytes del ArchivoDto subido.
                var _contenido = new byte[file.ContentLength];
                file.InputStream.Read(_contenido, 0, file.ContentLength);

                // Separamos el Nombre del ArchivoDto de la Extensión.
                int indiceDelUltimoPunto = file.FileName.LastIndexOf('.');
                string _nombre = file.FileName.Substring(0, indiceDelUltimoPunto);
                string _extension = file.FileName.Substring(indiceDelUltimoPunto + 1,
                                    file.FileName.Length - indiceDelUltimoPunto - 1);

                // Instanciamos la clase ArchivoDto y asignammos los valores.
                ArchivoDto _ArchivoDto = new ArchivoDto()
                {
                    Nombre = _nombre,
                    Extension = _extension,
                    Descargas = 0,
                    DenunciaId = (int)Session["DenunciaActual"]
                };

               
                    // Subimos el ArchivoDto al Servidor.
                    var carpetaActual = (string)Session["CarpetaActual"];
                    //var pathRoot = (string)Session["pathRootArchivos"];
                    _ArchivoDto.PathRelativo(carpetaActual);
                    _ArchivoDto.SubirArchivo(_contenido);

                    // Guardamos en la base de datos la instancia del ArchivoDto
                    using (_dbContext = new NuevoDbContext())
                    {
                    var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                    _ArchivoDto.usuarioCreador = usuario;
                    var archivoNuevo = _ArchivoDto;
                    _dbContext.Archivos.Add(_ArchivoDto);                        
                    _dbContext.SaveChanges();
                        
                        var logger = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIA", "Archivo Creado", archivoNuevo.Nombre + '.' + archivoNuevo.Extension, archivoNuevo.path, usuario, (int)Session["DenunciaActual"]);
                        _dbContext.Add(logger);
                        _dbContext.SaveChanges();

                    }
               
            }

            var id = (int)Session["DenunciaActual"];
            using (_dbContext = new NuevoDbContext())
            {

                var Archivos = _dbContext.Archivos.Where(x => x.DenunciaId == id).OrderByDescending(x => x.fechaCreacion).ToList();
                return PartialView("FileList", Archivos);
            }
            

            // Redirigimos a la Acción 'Index' para mostrar
            // Los Archivos subidos al Servidor.

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveDropzoneJsUploadedFiles()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            var _Archivos = new List<ArchivoDto>();

            var x = Request.Params.GetValues("aftFiles").First();
            List<string> tipos = new List<string> { ".DOC", ".DOCX", ".TXT", ".ODT", ".PDF", ".RTF", ".CSV", ".XLS", ".XLSX", ".ODS", ".PPS", ".PPT", ".PPSX", ".PPTX", ".POTX", ".ODP", ".JPG", ".JPEG", ".PNG", ".BMP", ".SVG", ".ZIP", ".RAR", ".RAR5", ".7Z", ".MSG", ".ACCDB", ".MDB", ".GIF", ".OGG", ".MP3", ".AVI", ".MP4", ".MKV", ".TIF" };

            if (x == (string)Session["aftFiles"])
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    var extension = Path.GetExtension(file.FileName);

                    if (tipos.Contains(extension.ToUpper()))
                    {
                        uploadFile(file);
                    }
                    else {
                        return Json("Tipo de Archivo No Permitido");
                    }
                    
                }
            }
            else
            {
                isSavedSuccessfully = false;
                throw new UnauthorizedAccessException("El origen la petición es desconocido. No posee autorización");
            }
       
  
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                
                return Json(new { Message = "Error in saving file" });
            }


        }



        //[HttpGet]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DescargarArchivo(Guid id)
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
                       
                        var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                        var logger = new CommonChangeLoggerDto(DateTime.Now,"DENUNCIA","Descarga de Archivo", _ArchivoDto.Nombre + '.' + _ArchivoDto.Extension, _ArchivoDto.path, usuario,(int)Session["DenunciaActual"]);
                        _dbContext.Add(logger);
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

        //[HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarArchivo(string id)
        {
            ArchivoDto _ArchivoDto;

            var idDelete = new Guid(id); 
            using (_dbContext = new NuevoDbContext())
            {
                _ArchivoDto = _dbContext.Archivos.FirstOrDefault(x => x.Id == idDelete);
            }

            if (_ArchivoDto != null)
            {
                using (_dbContext = new NuevoDbContext())
                {
                    _ArchivoDto = _dbContext.Archivos.FirstOrDefault(x => x.Id == idDelete);
                    var archivoEliminado = _ArchivoDto;
                    _dbContext.Archivos.Remove(_ArchivoDto);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        // Eliminamos el ArchivoDto del Servidor.
                        _ArchivoDto.EliminarArchivo();
                        var usuario = System.Web.HttpContext.Current.User.Identity.Name;
                        var logger = new CommonChangeLoggerDto(DateTime.Now, "DENUNCIA", "Archivo Eliminado", archivoEliminado.Nombre + '.' + archivoEliminado.Extension, archivoEliminado.path, usuario, (int)Session["DenunciaActual"]);
                        _dbContext.Add(logger);
                        _dbContext.SaveChanges();
                    }
                }
                // Redirigimos a la Acción 'Index' para mostrar
                // Los Archivos subidos al Servidor.
                return Json("Registro eliminado con éxito",JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }
        public FileResult Download()
        {
            // Obtener contenido del ArchivoDto
            string text = "El texto para mi ArchivoDto.";

            return File(Encoding.ASCII.GetBytes(text), "text/plain", "Prueba.txt.txt");
        }

        //public FileResult Download()
        //{
        //    // Obtener contenido del ArchivoDto
        //    string text = "El texto para mi ArchivoDto.";
        //    var stream = new MemoryStream(Encoding.ASCII.GetBytes(text));

        //    return File(stream, "text/plain", "Prueba.txt");
        //}



    }
}