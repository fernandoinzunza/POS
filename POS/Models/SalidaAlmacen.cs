using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class SalidaAlmacen
    {
        public int Id { get; set; }

        public string FechaSalida { get; set; }
        public string Razon { get; set; }
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
        public string ClaveArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public int IdCaja { get; set; }
        public int CajasSacadas
        {
            get; set;
        }
        public double UnidadesSacadas { get; set; }
        public double TotalPerdida
        {
            get; set;
        }
        public double TotalPerdidaSinIva
        {
            get { return TotalPerdida / 1.16; }
            set { }
        }
        public string NombreEmpleado { get; set; }
        public string FolioVenta { get; set; }
        public string IdDevolucion { get; set; }
    }
}
