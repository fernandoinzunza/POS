using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Devolucion
    {
        public int Id { get; set; }
        public int Folio { get; set; }
        public string ClaveArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public double Cantidad { get; set; }
        public int IdCaja { get; set; }
        public int IdCarrito { get; set; }
        public double Perdida { get; set; }
        public double PerdidaSivIva { get; set; }
        public string FechaDevolucion { get; set; }
        public string Razon { get; set; }
        public string TipoCancelacion { get; set; }





    }
}
