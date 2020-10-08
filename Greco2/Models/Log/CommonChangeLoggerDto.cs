using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greco2.Models.Log
{
    public class CommonChangeLoggerDto
    {
        public int Id { get; set; }
        public DateTime? FechaCambio { get; set; }
        public string ObjetoModificado { get; set; }
        public string Descripcion { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
        public string Usuario { get; set; }
        public int ObjetoId { get; set; }

        public CommonChangeLoggerDto(){}
        public CommonChangeLoggerDto(DateTime? fecha, string entidadModificada, string unaDescripcion,string unValorAnterior, string unValorActual, string user, int id) {
            FechaCambio = fecha;
            ObjetoModificado = entidadModificada;
            Descripcion = unaDescripcion;
            ValorAnterior = unValorAnterior;
            ValorActual = unValorActual;
            Usuario = user;
            ObjetoId = id;
        }
    }
}