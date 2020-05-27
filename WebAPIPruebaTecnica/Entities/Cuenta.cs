using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIPruebaTecnica.Entities
{
    public class Cuenta
    {
        [Timestamp]
        public byte[] TimeStamp { get; set; }
        public int Id { get; set; }
        [Required]
        public int IdCarteraEjecutivo { get; set; }
        [Required]
        [StringLength(20)]
        public string CodCuenta { get; set; }
        [Required]
        [StringLength(100)]
        public string DescripcionProducto { get; set; }
        [Required]
        [StringLength(5)]
        public  string CodTipoCuenta { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        [Required]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
