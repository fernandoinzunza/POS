using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class EstadisticaArticulo
    {
        public string ClaveArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public double Cantidad { get; set; }
        public double TotalSinIva { get; set; }
        public double TotalConIva { get; set; }
    }
}
