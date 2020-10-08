using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Greco2.Models.User
{
    public class LoginViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        
        [DataType(DataType.Text)]
        public string User { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
    }
}