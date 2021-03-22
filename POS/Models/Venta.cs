using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
   public class Venta
    {
        public int Folio { get; set; }
        public string Facturado { get; set; }
        public string NoFactura { get; set; }
        public string MetodoDePago { get; set; }
    }
}
