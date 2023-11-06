
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace inmoApi;
[Route("[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]    

public class PropietariosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;
	private readonly IWebHostEnvironment environment;

    public PropietariosController(DataContext dataContext, IConfiguration conf,  IWebHostEnvironment env){
        _context= dataContext;
        config= conf;
		environment= env;
    }

	// POST api/<controller>/login
	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login([FromForm] LoginView loginView)
	{
		try
		{
			if (ModelState.IsValid){
				
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: loginView.Clave,
						salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8
					));
				var p = await _context.Propietario.FirstOrDefaultAsync(x => x.Email == loginView.Usuario);
				if (p == null || p.Clave != hashed)
				{
					return BadRequest("Nombre de usuario o clave incorrecta");
				}	
																					
					var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, p.Email),
						new Claim("FullName", p.Nombre + " " + p.Apellido),
					};

					var token = new JwtSecurityToken(
						issuer: config["TokenAuthentication:Issuer"],
						audience: config["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60),
						signingCredentials: credenciales
					);
				return Ok(new JwtSecurityTokenHandler().WriteToken(token));
			}
			return BadRequest();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

    [HttpGet]
    public async Task<ActionResult> Get(){
        try
        {
            var propietario= User.Identity.Name;
            var res= await _context.Propietario.SingleOrDefaultAsync(x=> x.Email == propietario);
            return Ok(res);

        }
        catch (Exception ex)
        {
            
            return BadRequest(ex.Message);
        }
    }


	[HttpPut]
	public async Task<IActionResult> Put(Propietario entidad)
	{
		try
		{
			if(ModelState.IsValid && _context.Propietario.AsNoTracking().FirstOrDefault(X => X.Email == User.Identity.Name) != null)
			{
				Propietario p = _context.Propietario.AsNoTracking().Where(x => x.Email == User.Identity.Name).First();
				entidad.Id= p.Id;
				entidad.Estado= p.Estado;
				_context.Propietario.Update(entidad);
				await _context.SaveChangesAsync();
				return Ok(entidad);
			}
			return BadRequest();
		}
		catch (Exception ex)
		{
			
			return BadRequest(ex.Message);
		}
	}

	[HttpPut("EditarClave")]
	public async Task<IActionResult> CambiarClave([FromForm] string actual, [FromForm] string nueva)
	{
		try
		{
			string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    	password: actual,
                		salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                		prf: KeyDerivationPrf.HMACSHA1,
                		iterationCount: 1000,
                		numBytesRequested: 256 / 8
			));
			Propietario p = _context.Propietario.AsNoTracking().Where(x => x.Email == User.Identity.Name).First();
			if(p.Clave!= hash){
				return BadRequest("Error: clave actual ingresada incorrecta");
			}
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    	password: nueva,
                		salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                		prf: KeyDerivationPrf.HMACSHA1,
                		iterationCount: 1000,
                		numBytesRequested: 256 / 8
			));
			if(p.Clave == hashed){
				return BadRequest("Error: la clave nueva no puede ser igual a la actual");
			}
            p.Clave = hashed;
			_context.Propietario.Update(p);
			await _context.SaveChangesAsync();
			var key = new SymmetricSecurityKey(
							System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
			var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, p.Email),
				new Claim("FullName", p.Nombre + " " + p.Apellido),
			};

			var token = new JwtSecurityToken(
				issuer: config["TokenAuthentication:Issuer"],
				audience: config["TokenAuthentication:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(60),
				signingCredentials: credenciales
			);
			return Ok(new JwtSecurityTokenHandler().WriteToken(token));
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	/*
    Buscar Propietario por id
	[HttpGet("{id}")]
	[AllowAnonymous]
	public async Task<IActionResult> Get(int id)
	{
			try
			{
				var entidad = await _context.Propietario.SingleOrDefaultAsync(x => x.Id == id);
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
	}
		[HttpPost("email")]
		[AllowAnonymous]
		public async Task<IActionResult> GetByEmail([FromForm] string email)
		{
			try
			{ //método sin autenticar, busca el propietario x email
				var entidad = await _context.Propietario.FirstOrDefaultAsync(x => x.Email == email);
				//para hacer: si el propietario existe, mandarle un email con un enlace con el token
				//ese enlace servirá para resetear la contraseña
				//Dominio sirve para armar el enlace, en local será la ip y en producción será el dominio www...
				//var dominio = environment.IsDevelopment() ? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() : "www.misitio.com";
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		
		[HttpPost("crear")]
		[AllowAnonymous]
		public async Task<IActionResult> Crear([FromForm] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    	password: entidad.Clave,
                		salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                		prf: KeyDerivationPrf.HMACSHA1,
                		iterationCount: 1000,
                		numBytesRequested: 256 / 8
					));
                    entidad.Clave = hashed;
					await _context.Propietario.AddAsync(entidad);
					_context.SaveChanges();
					return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}*/
}