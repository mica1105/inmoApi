using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inmoApi;

public class Propietario
{
		
	[Key]
	[Display(Name = "Código")]
	public int Id { get; set; }
		
	[Required]
	public string Nombre { get; set; }  = "";
		
	[Required]
	public string Apellido { get; set; } = "";
		
	[Required]
	public string Dni { get; set; } = "";

	public string? Telefono { get; set; }

	public int Estado { get; set; }

	[Required, EmailAddress]
	public string Email { get; set; } = "";
	
	[Required, DataType(DataType.Password)]
	public string Clave { get; set; } = "";
}
        
