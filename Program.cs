using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RapasIMCEEApi;
using RapasIMCEEApi.Data;
using RapasIMCEEApi.Dtos;
using RapasIMCEEApi.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var conn = EnvrionnementVariableHelper.GetConnectionStringFromEnv();

builder.Services.AddDbContext<RapasDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT_KEY missing");
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

// ... swagger, etc
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/login", async (LoginDto dto, RapasDbContext db) =>
{
    // On cherche l’utilisateur par son identifiant
    var user = await db.Utilisateurs.SingleOrDefaultAsync(u => u.Libelle == dto.Libelle);
    if (user == null) return Results.Unauthorized();

    // Récupération de la clé de chiffrement depuis l’environnement
    var passwordKeyBase64 = Environment.GetEnvironmentVariable("PASSWORD_KEY")
         ?? throw new Exception("PASSWORD_KEY is missing");

    // Déchiffrage du mot de passe stocké en DB
    var storedPlain = CryptoHelper.DecryptString(user.Password, passwordKeyBase64);

    // Comparaison avec celui reçu
    if (storedPlain != dto.Password) return Results.Unauthorized();

    // Génération du JWT
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT_KEY missing");
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
    var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        claims: new[] {
            new System.Security.Claims.Claim("sub", user.Id.ToString()),
            new System.Security.Claims.Claim("libelle", user.Libelle)
        },
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: creds
    );

    var jwt = tokenHandler.WriteToken(token);
    return Results.Ok(new { token = jwt });
});
