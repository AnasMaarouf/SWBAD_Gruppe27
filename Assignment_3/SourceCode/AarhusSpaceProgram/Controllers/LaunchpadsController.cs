using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class LaunchpadsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<LaunchpadsController> _logger;

    public LaunchpadsController(MainDBContext context, ILogger<LaunchpadsController> logger) {
        _context = context;
        _logger = logger;
    }

    // GET: api/Launchpads
    // Gets all Launchpads
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LaunchpadDto>>> GetLaunchpads()
    {
        return await _context.Launchpads
            .Select(l => new LaunchpadDto {
                ID = l.ID,
                Location = l.Location,
                CurrentStatus = l.CurrentStatus,
                MaxSupportedWeight = l.MaxSupportedWeight
            })
            .ToListAsync();
    }

    // GET: api/Launchpads/{id}
    // Gets launchpad from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<LaunchpadDto>> GetLaunchpad(int id)
    {
        var launchpad = await _context.Launchpads
            .Where(l => l.ID == id)
            .Select(l => new LaunchpadDto {
                ID = l.ID,
                Location = l.Location,
                CurrentStatus = l.CurrentStatus,
                MaxSupportedWeight = l.MaxSupportedWeight
            })
            .FirstOrDefaultAsync();

        if (launchpad == null)
            return NotFound();

        return Ok(launchpad);
    }

    // POST: api/Launchpads
    // Creates an launchpad
    [HttpPost]
    public async Task<ActionResult<LaunchpadDto>> CreateLaunchpad(LaunchpadCreateDto dto)
    {
        if (dto.MaxSupportedWeight < 0)
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");

        var launchpad = new Launchpad {
            Location = dto.Location,
            CurrentStatus = dto.CurrentStatus.ToString(),
            MaxSupportedWeight = dto.MaxSupportedWeight
        };

        _context.Launchpads.Add(launchpad);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetLaunchpad), new { id = launchpad.ID }, new LaunchpadDto {
            ID = launchpad.ID,
            Location = launchpad.Location,
            MaxSupportedWeight = launchpad.MaxSupportedWeight
        });
    }

    // PUT: api/Launchpads/{id}
    // Updates launchpad on id
    [HttpPut("{id}")]
     public async Task<IActionResult> UpdateLaunchpad(int id, LaunchpadUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        if (dto.MaxSupportedWeight < 0)
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");

        var launchpad = await _context.Launchpads.FindAsync(id);
        if (launchpad == null)
            return NotFound();

        launchpad.Location = dto.Location;
        launchpad.CurrentStatus = dto.CurrentStatus.ToString();
        launchpad.MaxSupportedWeight = dto.MaxSupportedWeight;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/Launchpads/{id}
    // Deletes launchpad by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLaunchpad(int id)
    {
        var launchpad = await _context.Launchpads.FindAsync(id);
        if (launchpad == null)
            return NotFound();

        _context.Launchpads.Remove(launchpad);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}