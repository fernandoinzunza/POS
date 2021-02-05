using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POS.Models
{
    public class EntradaAlmacen
    {
       
        public int Id { get; set; }
        public string Fecha { get; set; }
        public int IdDepartamento { get; set; }
        public string Departamento { get; set; }
        public string ClaveArticulo{get;set;}
        public string Articulo { get; set; }
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; }
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double CostoTotal { get; set; }
        public int NoEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string NoFactura { get; set; }

      
        public string Caja { get; set; }
        public double UnidadesXCaja { get; set; }
        public int CajasStock { get; set; } 

    }
}
