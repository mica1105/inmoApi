using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using inmoApi;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
//var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
//var connectionString = "server=localhost;user=root;password=;database=bdinmobiliaria";
var connectionString= builder.Configuration.GetConnectionString("MySqlConnection");
var issuer= builder.Configuration.GetSection("TokenAuthentication")["Issuer"];
var audience=builder.Configuration.GetSection("TokenAuthentication")["Audience"];
var secretKey= builder.Configuration.GetSection("TokenAuthentication")["SecretKey"];
builder.WebHost.UseUrls("http://localhost:5000","https://localhost:5001", "http://*:5000", "https://*:5001");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddJwtBearer(options=>
{
    options.TokenValidationParameters= new Microsoft.IdentityModel.Tokens.TokenValidationParameters{
        ValidateIssuer= true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer= issuer,
        ValidAudience= audience,
        IssuerSigningKey= new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secretKey)),
    };
    options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							// Leer el token desde el query string
							var accessToken = context.Request.Query["access_token"];
							// Si el request es para el Hub u otra ruta seleccionada...
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) &&
								(path.StartsWithSegments("/chatsegurohub") ||
								path.StartsWithSegments("/propietarios/reset") ||
								path.StartsWithSegments("/propietarios/token")))
							{//reemplazar las urls por las necesarias ruta ⬆
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
});

builder.Services.AddDbContext<DataContext>(
	options=> options.UseMySql(
	connectionString,
	ServerVersion.AutoDetect(connectionString)
	)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
