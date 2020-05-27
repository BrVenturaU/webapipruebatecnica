using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIPruebaTecnica.Models
{
    public class CuentasClienteDTO
    {
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
        public string CodTipoCuenta { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}