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
            // Recupera l'utente dal database
            var user = _context.Utenti.FirstOrDefault(u => u.Email == request.Username);

            // Controlla se l'utente esiste e verifica la password
            if (user == null || user.Password != HashPassword(request.Password))
            {
                return Unauthorized("Credenziali non valide");
            }

            // Genera il token JWT basato sul ruolo
            var token = GenerateJwtToken(user.Ruolo == 3 ? "Administrator" : "User");

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore interno del server: {ex.Message}");
        }
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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}