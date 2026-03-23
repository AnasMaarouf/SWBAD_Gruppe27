using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

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
    public async Task<ActionResult<IEnumerable<AstronautDto>>> GetAstronauts()
    {
        return await _context.Astronauts
            .Include(a => a.Employee)
            .Select(a => new AstronautDto {
                ID = a.ID,
                Rank = a.Rank,
                FlightHours = a.FlightHours,
                Paygrade = a.Paygrade,
                FullName = a.Employee != null ? a.Employee.FullName : null
            })
            .ToListAsync();
    }

    // GET: api/astronauts/{id}
    // Gets astronaut from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<AstronautDto>> GetAstronaut(int id)
    {
        var astronaut = await _context.Astronauts
            .Include(a => a.Employee)
            .Where(a => a.ID == id)
            .Select(a => new AstronautDto {
                ID = a.ID,
                Rank = a.Rank,
                FlightHours = a.FlightHours,
                Paygrade = a.Paygrade,
                FullName = a.Employee != null ? a.Employee.FullName : null
            })
            .FirstOrDefaultAsync();

        if (astronaut == null)
            return NotFound();

        return Ok(astronaut);
    }

    [HttpGet("OrderByExperience")]
    public async Task<ActionResult<IEnumerable<AstronautDto>>> GetAstronaut_OrderByExperience()
    {
        var astronauts = await _context.Astronauts
            .Include(a => a.Employee)
            .OrderByDescending(a => a.FlightHours)
            .Select(a => new AstronautDto {
                ID = a.ID,
                Rank = a.Rank,
                FlightHours = a.FlightHours,
                Paygrade = a.Paygrade,
                FullName = a.Employee != null ? a.Employee.FullName : null
            })
            .ToListAsync();

        if (astronauts == null)
            return NotFound();

        return Ok(astronauts);
    }

    // POST: api/astronauts
    // Creates an astronaut
    [HttpPost]
    public async Task<ActionResult<AstronautDto>> CreateAstronaut(AstronautCreateDto dto)
    {
        var astronaut = new Astronaut {
            ID = dto.FK_EmployeeID,
            Rank = dto.Rank,
            FlightHours = dto.FlightHours,
            Paygrade = dto.Paygrade
        };

        _context.Astronauts.Add(astronaut);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetAstronaut), new { id = astronaut.ID }, new AstronautDto {
            ID = astronaut.ID,
            Rank = astronaut.Rank,
            FlightHours = astronaut.FlightHours,
            Paygrade = astronaut.Paygrade
        });
    }

    // PUT: api/astronauts/{id}
    // Updates astronaut on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAstronaut(int id, AstronautUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        var astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null)
            return NotFound();

        astronaut.Rank = dto.Rank;
        astronaut.FlightHours = dto.FlightHours;
        astronaut.Paygrade = dto.Paygrade;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

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

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}