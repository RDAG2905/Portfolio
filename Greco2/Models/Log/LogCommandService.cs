using Greco2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class LogCommandService
    {
        public LogCommandService() {
        }

        public void setError(Exception e)
        {
            using (NuevoDbContext context = new NuevoDbContext())
            {
                var error = new LogErrorDto();
                error.Fecha = DateTime.Now;
                error.Error = e.Message;
                error.UrlRequest = HttpContext.Current.Request.Url.ToString();
                error.ErrorDetallado = e.ToString();
                error.UserId = HttpContext.Current.User.Identity.Name;


            }
        }
    }
}