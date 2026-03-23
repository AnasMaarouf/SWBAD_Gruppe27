using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CrewsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<CrewsController> _logger; 

    public CrewsController(MainDBContext context, ILogger<CrewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Crews
    // Gets all Crews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CrewDto>>> GetCrews()
    {
        return await _context.Crews
            .Select(c => new CrewDto {
                ID = c.ID
            })
            .ToListAsync();
    }

    // GET: api/celestialBodies/{id}
    // Gets crew from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<CrewDto>> GetCrew(int id)
    {
        var crew = await _context.Crews
            .Where(c => c.ID == id)
            .Select(c => new CrewDto {
                ID = c.ID
            })
            .FirstOrDefaultAsync();

        if (crew == null)
            return NotFound();

        return Ok(crew);
    }

    // POST: api/Crews
    // Creates an crew
    [HttpPost]
    public async Task<ActionResult<CrewDto>> CreateCrew(CrewCreateDto dto)
    {
        var crew = new Crew();

        _context.Crews.Add(crew);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetCrew), new { id = crew.ID }, new CrewDto { ID = crew.ID });
    }

    // PUT: api/Crews/{id}
    // Updates crew on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCrew(int id, CrewUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        var crew = await _context.Crews.FindAsync(id);
        if (crew == null)
            return NotFound();

        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/Crews/{id}
    // Deletes crew by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCrew(int id)
    {
        var crew = await _context.Crews.FindAsync(id);
        if (crew == null)
            return NotFound();

        _context.Crews.Remove(crew);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}