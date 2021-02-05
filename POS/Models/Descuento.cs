using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS.Models
{
    public class Descuento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoDescuento
        {
            get; set;
        }
        public string Nombre { get; set; }

        public double Porcentaje { get; set; }
    }
}
