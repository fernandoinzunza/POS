using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Articulo
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Clave {
            get; set;
        }
        [Column(TypeName = "varchar(200)")]
        public string Descripcion { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Departamento { get; set; }
        public double PrecioPublico { get; set; }
        public double PrecioSecundario { get; set; }
        public double PrecioAlCosto { get; set; }
        public float Stock { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string TipoUnidad { get; set; }

        public double PrecioMayoreo { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Unidad { get; set; }
        public int? IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public double StockBajo { get; set; }
        public int IdDepartamento { get; set; }
        public string Caja { get; set; }
        public double UnidadesXCaja {get;set;}
        public int CajasStock { get; set; }
        public double PrecioXCaja { get; set; }
    }
}
