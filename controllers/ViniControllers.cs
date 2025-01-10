using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Winery_backend.controllers;

[Route("api/[controller]")]
[ApiController]
public class ViniController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ViniController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/vini
    [HttpGet]
    public async Task<IActionResult> GetVini()
    {
        var vini = await _context.Vini.ToListAsync();
        return Ok(vini);
    }
}