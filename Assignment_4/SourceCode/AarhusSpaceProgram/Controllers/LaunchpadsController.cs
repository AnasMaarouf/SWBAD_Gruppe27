using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<LaunchpadResponseDTO>>> GetLaunchpads() {
        var launchpads = await _context.Launchpads.Select(dto => new LaunchpadResponseDTO {
            Id = dto.ID,
            Location = dto.Location,
            CurrentStatus = dto.CurrentStatus
        }).ToListAsync();

        if(launchpads == null)
            return NotFound("No launchpads exists");
        
        return Ok(launchpads);
    }

    // GET: api/Launchpads/{id}
    // Gets launchpad from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<DetailedLaunchpadResponseDTO>> GetLaunchpad(int id) {
        
        var launchpad = await _context.Launchpads
        .Include(l => l.Missions)
        .Select( dto => new DetailedLaunchpadResponseDTO {
            Id = dto.ID,
            Location = dto.Location,
            CurrentStatus = dto.CurrentStatus,
            MaxSupportedWeight = dto.MaxSupportedWeight,
            Missions = dto.Missions.Select(m => m.Name).ToList()
        }).FirstOrDefaultAsync(dto => dto.Id == id);

        if (launchpad == null)
            return NotFound();

        return Ok(launchpad);
    }

    // POST: api/Launchpads
    // Creates an launchpad
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Launchpad>> CreateLaunchpad(CreateLaunchpadDTO dto) {
        if(dto.MaxSupportedWeight < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");
        }

        var launchpad = new Launchpad
        {
            CurrentStatus = dto.CurrentStatus,
            Location = dto.Location,
            MaxSupportedWeight = dto.MaxSupportedWeight
        };

        _context.Launchpads.Add(launchpad);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return CreatedAtAction(nameof(GetLaunchpad), new { id = launchpad.ID }, launchpad);
    }

    // PUT: api/Launchpads/{id}
    // Updates launchpad on id
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UpdateLaunchpad(int id, CreateLaunchpadDTO dto) {
        if(dto.MaxSupportedWeight < 0)
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");

        var launchpad = await _context.Launchpads
        .FirstOrDefaultAsync(dto => dto.ID == id);

        if(launchpad == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Launchpad does not exist");
        }

        launchpad.CurrentStatus = dto.CurrentStatus;
        launchpad.Location = dto.Location;
        launchpad.MaxSupportedWeight = dto.MaxSupportedWeight;

        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Launchpad updated");
    }

    // DELETE: api/Launchpads/{id}
    // Deletes launchpad by id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> DeleteLaunchpad(int id)
    {
        var launchpad = await _context.Launchpads.FindAsync(id);
        if (launchpad == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound();
        }

        _context.Launchpads.Remove(launchpad);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }
}