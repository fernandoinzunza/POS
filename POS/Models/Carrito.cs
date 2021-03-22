using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
   public class Carrito
    {
        public int Id { get; set; }
        public int Folio { get; set; }
        public string ClaveArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public double PrecioAlCosto { get; set; }
        public double PrecioPublico { get; set; }
        public double Cantidad { get; set; }
        public double Utilidad { get; set; }
        public double SubTotal { get; set; }
        public string MetodoDePago { get; set; }
        public double SubTotalSinIva { get; set; }
        public string Cancelado { get; set; }
        public string RazonCancelado { get; set; }
        public string FechaVenta { get; set; }
        public int IdDescuento { get; set; }
        public string NombreDescuento { get; set; }
        public double PorcentajeDescuento { get; set; }

    }
}
