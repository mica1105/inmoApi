
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


    public InmueblesController(DataContext dataContext, IConfiguration conf){
        _context= dataContext;
        config= conf;
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