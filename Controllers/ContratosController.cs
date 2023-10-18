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

public class ContratosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;


    public ContratosController(DataContext dataContext, IConfiguration conf){
        _context= dataContext;
        config= conf;
    }

    [HttpPost]
    public async Task<ActionResult<Contrato>> PostContrato([FromBody] Inmueble inmueble)
    {
        try
        {
            var usuario= User.Identity.Name;
            var contrato = await _context.Contrato.Include(c=> c.Inquilino).Include(i=>i.Inmueble)
            .Where(x=> x.Inmueble.Duenio.Email== usuario && x.Inmueble.Id == inmueble.Id)
            .FirstAsync();
            //await _context.Inquilino.FindAsync(id);

            if (contrato == null)
            {
                return NotFound();
            }

            return Ok(contrato);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}