using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class EntradaGrid
    {
        public string Fecha { get; set; }
        public int IdDepartamento { get; set; }
        public string Departamento { get; set; }
        public string ClaveArticulo { get; set; }
        public string Articulo { get; set; }
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; }
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double PrecioXCaja { get; set; }
        public double CostoTotal { get; set; }
        public string NoFactura { get; set; }
    }
}
