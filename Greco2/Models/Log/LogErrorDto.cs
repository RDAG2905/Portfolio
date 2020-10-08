using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class LogErrorDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Error { get; set; }
        public string UrlRequest { get; set; }
        public string UserId { get; set; }
        public string ErrorDetallado { get; set; }
    }
}