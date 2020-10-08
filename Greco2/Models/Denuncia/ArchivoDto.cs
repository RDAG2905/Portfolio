using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Greco2.Models.Denuncia
{

    public class ArchivoDto
    {
        // CONSTRUCTOR
        public ArchivoDto()
        {
            this.Id = Guid.NewGuid();
            this.fechaCreacion = DateTime.Now;
        }

        // PROPIEDADES PÚBLICAS

        public Guid Id { get; set; }
        
        public DateTime fechaCreacion { get; set; }
       
        public string Nombre { get; set; }
        
        public string Extension { get; set; }
        
        public int Descargas { get; set; }
        public string path { get; set; }
        public int DenunciaId { get; set; }
        public string usuarioCreador { get; set; }
        

        public void PathRelativo(string carpeta)
        {

            this.path =  
                ( carpeta +"/"+ this.Id.ToString() + "." + this.Extension);
            

        }

        public string PathCompleto // Versión con el Path raiz en la Base 
        {
            get
            {
                var _PathAplicacion = HttpContext.Current.Request.PhysicalApplicationPath;
                return Path.Combine(_PathAplicacion, this.path);

            }
        }

        

        // MÉTODOS PÚBLICOS
        public void SubirArchivo(byte[] archivo)
        {
            File.WriteAllBytes(this.PathCompleto, archivo);
        }

        public byte[] DescargarArchivo()
        {
            return File.ReadAllBytes(this.PathCompleto);
        }

        public void EliminarArchivo()
        {           
            File.Delete(this.path);
        }


        //public void PathRelativo(string carpeta) // Versión con el Path raiz en el Web Config
        //{

        //    this.path =
        //        (carpeta + "/" + this.Id.ToString() + "." + this.Extension);


        //}


        //public string PathCompleto // Versión con el Path raiz en el Web Config 
        //{
        //    get
        //    {
        //        var _PathAplicacion = HttpContext.Current.Request.PhysicalApplicationPath;
        //        if (Path.IsPathRooted(this.path))
        //        {
        //            throw new ArgumentException(this.path + " El origen de este Parámetro es Desconocido");
        //        }
        //        else
        //        {
        //            return Path.Combine(_PathAplicacion, this.path);
        //        }

        //    }
        //}
    }
}