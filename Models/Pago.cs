using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmoApi;

public class Pago{
        [Key]
		[Display(Name = "Código")]
		public int Id { get; set; }
		[Required] [Display(Name ="Nro de Pago")]
		public int NroPago { get; set; }
		[Required][DataType(DataType.Date)]
		public DateTime Fecha { get; set; }
		[Required]
		public decimal Importe { get; set; }
		[Required][Display(Name = "Nro Contrato")]
		public int ContratoId { get; set; }
		[ForeignKey("ContratoId")]
		public Contrato? Contrato { get; set; }
}