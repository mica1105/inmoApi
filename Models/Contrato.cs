using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmoApi;

public class Contrato{
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required] 
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
        [Required]
        public Decimal Precio { get; set; }
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }
        [ForeignKey("InmuebleId")]
        public Inmueble? Inmueble { get; set; }
        [Display(Name ="Inquilino")]
        public int InquilinoId { get; set; }
        [ForeignKey("InquilinoId")]
        public Inquilino? Inquilino { get; set; }
}