using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmoApi;

public class Inmueble{
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        public string Direccion { get; set; } = "";
        [Required]
        public string Tipo { get; set; } = "";
        public int Ambientes { get; set; }
        [Required]
        public string Uso { get; set; } = "";
        [Required]
        public decimal Precio { get; set; }
        public string Imagen { get; set; } = "";
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey("PropietarioId")]
        public Propietario? Duenio { get; set; }
        public bool Estado { get; set; }
        
}