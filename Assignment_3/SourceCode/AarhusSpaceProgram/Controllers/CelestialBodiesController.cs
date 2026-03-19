using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CelestialBodiesController : ControllerBase
{
    private readonly MainDBContext _context;

    public CelestialBodiesController(MainDBContext context)
    {
        _context = context;
    }

    // GET: api/CelestialBodies
    // Gets all CelestialBodies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CelestialBody>>> GetCelestialBodies()
    {
        return await _context.CelestialBodies
            .Include(c => c.ParentPlanet)
            .ToListAsync();
    }

    // GET: api/celestialBodies/{id}
    // Gets celestialBody from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<CelestialBody>> GetCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies
            .Include(c => c.ParentPlanet)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (celestialBody == null)
            return NotFound();

        return celestialBody;
    }

    // POST: api/CelestialBodies
    // Creates an celestialBody
    [HttpPost]
    public async Task<ActionResult<CelestialBody>> CreateCelestialBody(CelestialBody celestialBody)
    {
        if (celestialBody.ID < 0)
            return BadRequest("Invalid value: Celestialbody.ID: Must not be negative!");
        
        _context.CelestialBodies.Add(celestialBody);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCelestialBody), new { id = celestialBody.ID }, celestialBody);
    }

    // PUT: api/CelestialBodies/{id}
    // Updates celestialBody on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCelestialBody(int id, CelestialBody celestialBody) {
        if (id != celestialBody.ID)
            return BadRequest();

        if (celestialBody.ID < 0)
            return BadRequest("Invalid value: Celestialbody.ID: Must not be negative!");

        _context.Entry(celestialBody).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.CelestialBodies.Any(c => c.ID == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/CelestialBodies/{id}
    // Deletes celestialBody by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies.FindAsync(id);
        if (celestialBody == null)
            return NotFound();

        _context.CelestialBodies.Remove(celestialBody);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}