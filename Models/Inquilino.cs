using System.ComponentModel.DataAnnotations;

namespace inmoApi;

public class Inquilino{
    [Key]
    [Display(Name = "Código")]
    public int id {get; set;}
    [Required]
	public string Nombre { get; set; } ="";
	[Required]
	public string Apellido { get; set; } ="";
	[Required]
	public string Dni { get; set; } ="";
	[Required]
    [Display(Name = "Lugar de Trabajo")]
	public string LugarTrabajo { get; set; } ="";
	public string? Telefono { get; set; }
	[Required, EmailAddress]
	public string Email { get; set; } ="";
	[Required]
	[Display(Name = "Nombre")]
	public string NombreGarante { get; set; } ="";
	[Required]
	[Display(Name = "Dni")]
	public string DniGarante { get; set; } ="";
	[Display(Name = "Teléfono")]
	public string? TelefonoGarante { get; set; }

}