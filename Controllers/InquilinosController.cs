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

public class InquilinosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;
    


    public InquilinosController(DataContext dataContext, IConfiguration conf){
        _context= dataContext;
        config= conf;
    }

    [HttpPost]
    public async Task<ActionResult<Inquilino>> PostInquilino([FromBody] Inmueble inmueble)
    {
        try
        {
            var usuario= User.Identity.Name;
            
            var inquilino = await _context.Contrato.Include(i=>i.Inquilino)
            .Where(x=> x.Inmueble.Duenio.Email== usuario && x.Inmueble.Id == inmueble.Id)
            .Select(i=> i.Inquilino).FirstAsync();
            //await _context.Inquilino.FindAsync(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return Ok(inquilino);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}