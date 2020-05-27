using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIPruebaTecnica.Models
{
    public class TransaccionCreacionDTO
    {
        [Required]
        [StringLength(10)]
        public string CodMovimiento { get; set; }
        [Required]
        [StringLength(20)]
        public string CodCuenta { get; set; }
        [Required]
        [StringLength(20)]
        public string CodCliente { get; set; }
        [Required]
        [StringLength(100)]
        public string NombreCliente { get; set; }
        [Required]
        [StringLength(20)]
        public string CodRuta { get; set; }
        [Required]
        [StringLength(30)]
        public string CodEjecutivo { get; set; }
        [Required]
        public DateTime FechaCobro { get; set; }
        [Required]
        [StringLength(40)]
        public string NumeroComprobante { get; set; }
        [Required]
        public double Importe { get; set; }
        [Required]
        [StringLength(5)]
        public string CodTransaccion { get; set; }
        public DateTime FechaInsert { get; set; }
        [Required]
        [StringLength(20)]
        public string CodUsuario { get; set; }
        [Required]
        [StringLength(5)]
        public string CodTipoCuenta { get; set; }
        [Required]
        [StringLength(36)]
        public string Referencia { get; set; }
        [Required]
        public int IdMovimientoApp { get; set; }
        public DateTime FechaMovimientoApp { get; set; }
        [Required]
        public int IdClientePhoenix { get; set; }
        [Required]
        public int IdCuentaPhoenix { get; set; }
        [Required]
        public int CodTipoTransaccion { get; set; }
        [Required]
        public int IdTransacAnula { get; set; }
        [Required]
        [StringLength(300)]
        public string UserToken { get; set; }
    }
}
