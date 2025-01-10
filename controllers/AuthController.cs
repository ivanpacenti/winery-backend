using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Winery_backend.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _context; 

    public AuthController(IOptions<JwtSettings> jwtSettings, ApplicationDbContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = _context.Utenti.FirstOrDefault(u => u.Email == request.Username);

            if (user == null || user.Password != HashPassword(request.Password, user.Salt))
            {
                return Unauthorized("Credenziali non valide");
            }

            var token = GenerateJwtToken(user.Ruolo == 3 ? "Administrator" : "User");

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore interno del server: {ex.Message}");
        }
    }

    
    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }


    [Authorize(Roles = "Administrator")]
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        return Ok("Questa è una risposta protetta!");
    }

    private string GenerateJwtToken(string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "user"),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
    
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Controlla se l'email è già utilizzata
            if (_context.Utenti.Any(u => u.Email == request.Email))
            {
                return BadRequest("L'email è già in uso.");
            }

            // Genera il salt
            var salt = GenerateSalt();

            // Hasha la password
            var hashedPassword = HashPassword(request.Password, salt);

            // Crea un nuovo utente
            var newUser = new Utente
            {
                Nome = request.Nome,
                Cognome = request.Cognome,
                Email = request.Email,
                Password = hashedPassword,
                Salt = salt,
                DataRegistrazione = DateTime.Now,
                Ruolo = request.Ruolo // Può essere 2 (Utente normale) o altro se necessario
            };

            // Aggiunge l'utente al database
            _context.Utenti.Add(newUser);
            _context.SaveChanges();

            return Ok("Utente registrato con successo.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore interno del server: {ex.Message}");
        }
    }


}



public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RegisterRequest
{
    public string Nome { get; set; }
    public string Cognome { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int Ruolo { get; set; } = 2; // Default: Utente normale
}
