
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

public class InmueblesController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment environment;


    public InmueblesController(DataContext dataContext, IConfiguration conf, IWebHostEnvironment env){
        _context= dataContext;
        config= conf;
        environment= env;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inmueble>>> GetInmuebles(){
        try
        {
            var usuario= User.Identity.Name;
            var res= await _context.Inmueble.Where(e=> e.Duenio.Email == usuario).ToListAsync();
            return Ok(res);
        }
        catch (Exception ex)
        {
            
            return BadRequest(ex);
        }
    }
    [HttpGet("Alquilados")]
    public async Task<ActionResult<IEnumerable<Inmueble>>> GetPorInquilino(){
        try
        {
            var usuario= User.Identity.Name;
            var inmuebles= await _context.Contrato.Include(e=> e.Inmueble)
            .Where(e=> e.Inmueble.Duenio.Email == usuario)
            .Select(x=> x.Inmueble).ToListAsync();
            return Ok(inmuebles);
        }
        catch (Exception ex)
        {
            
            return BadRequest(ex);
        }
    }
    	[HttpPost("Crear")]
		public async Task<IActionResult> Crear([FromForm] Inmueble inmueble)
		{
			try
			{
				
                    var u= User.Identity.Name;
                    Propietario propietario = await _context.Propietario.FirstAsync(p=> p.Email== u);
                    inmueble.Estado= false;
                    inmueble.PropietarioId= propietario.Id;
                    await _context.Inmueble.AddAsync(inmueble);
					_context.SaveChanges();
                    if (inmueble.ImagenFile != null && inmueble.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = "inmueble_" + inmueble.Id + Path.GetExtension(inmueble.ImagenFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        inmueble.Imagen = Path.Combine("/Uploads", fileName);
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            inmueble.ImagenFile.CopyTo(stream);
                        }
                        _context.Inmueble.Update(inmueble);
                        await _context.SaveChangesAsync();
                    }
                    return CreatedAtAction(nameof(GetInmuebles), new { id = inmueble.Id }, inmueble);

            }
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
            }
        }


    [HttpGet("{id}")]
    public async Task<ActionResult<Inmueble>> GetInmueble(int id){
        try
        {
            var usuario= User.Identity.Name;
            var res= await _context.Inmueble.Include(e=> e.Duenio).Where( e => e.Duenio.Email == usuario).SingleAsync(e=> e.Id == id);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutInmueble(Inmueble inmueble)
    {
        try
        {
            if (ModelState.IsValid && _context.Inmueble.AsNoTracking().Include(e => e.Duenio).FirstOrDefault(e => e.Id == inmueble.Id && e.Duenio.Email == User.Identity.Name) != null)
            {
               var propiedad = await _context.Inmueble.FindAsync(inmueble.Id);
               propiedad.Estado= inmueble.Estado;
                _context.Inmueble.Update(propiedad);
                await _context.SaveChangesAsync();
                return Ok(inmueble);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}