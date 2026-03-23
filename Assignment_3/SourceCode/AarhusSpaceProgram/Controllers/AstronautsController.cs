using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AstronautsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<AstronautsController> _logger; 

    public AstronautsController(MainDBContext context, ILogger<AstronautsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/astronauts
    // Gets all astronauts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Astronaut>>> GetAstronauts()
    {
        return await _context.Astronauts
            .ToListAsync();
    }

    // GET: api/astronauts/{id}
    // Gets astronaut from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<Astronaut>> GetAstronaut(int id)
    {
        var astronaut = await _context.Astronauts
            .FirstOrDefaultAsync(m => m.ID == id);

        if (astronaut == null)
            return NotFound();

        return Ok(astronaut);
    }

    [HttpGet("OrderByExperience")]
    public async Task<ActionResult<Astronaut>> GetAstronaut_OrderByExperience()
    {
        var astronauts = await _context.Astronauts
            .Include(a => a.Employee)
            .OrderByDescending(a => a.FlightHours)
            .Select(a => new {
                Name = a.Employee!.FullName,
                a.Rank,
                a.FlightHours
            }).ToListAsync();

        if (astronauts == null)
            return NotFound();

        return Ok(astronauts);
    }

    // POST: api/astronauts
    // Creates an astronaut
    [HttpPost]
    public async Task<ActionResult<Astronaut>> CreateAstronaut(Astronaut astronaut)
    {
        if (astronaut.ID < 0)
            return BadRequest("Invalid value: Astronaut.ID: Must not be negative!");
        
        _context.Astronauts.Add(astronaut);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetAstronaut), new { id = astronaut.ID }, astronaut);
    }

    // PUT: api/astronauts/{id}
    // Updates astronaut on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAstronaut(int id, Astronaut astronaut) {
        if (id != astronaut.ID)
            return BadRequest();

        if (astronaut.ID < 0)
            return BadRequest("Invalid value: Astronaut.ID: Must not be negative!");

        _context.Entry(astronaut).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (_context.Astronauts.Any(a => a.ID == id))
                return NotFound();
            throw;
        }

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfor}", logInfo);

        return NoContent();
    }

    // DELETE: api/astronauts/{id}
    // Deletes astronaut by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAstronaut(int id)
    {
        var astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
            return NotFound();

        _context.Astronauts.Remove(astronaut);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}