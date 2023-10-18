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

public class PagosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration config;


    public PagosController(DataContext dataContext, IConfiguration conf){
        _context= dataContext;
        config= conf;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<Pago>>> GetPagos(int id)
    {
        try
        {
            var usuario= User.Identity.Name;
            var pagos = await _context.Pago.Include(p=> p.Contrato).Where(x=>x.Contrato.Id == id).ToListAsync();
            //await _context.Inquilino.FindAsync(id);

            if (pagos == null)
            {
                return NotFound();
            }

            return Ok(pagos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }


}