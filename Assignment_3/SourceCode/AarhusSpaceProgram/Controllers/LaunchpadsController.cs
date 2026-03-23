using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<object>>> GetLaunchpads() {
        return await _context.Launchpads.Select(l_dto => new {
            l_dto.ID,
            l_dto.Location,
            l_dto.CurrentStatus,
            l_dto.MaxSupportedWeight
        }).ToListAsync();
    }

    // GET: api/Launchpads/{id}
    // Gets launchpad from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetLaunchpad(int id) {
        
        var launchpad = await _context.Launchpads.Select(l_dto => new {
            l_dto.ID,
            l_dto.Location,
            l_dto.CurrentStatus,
            l_dto.MaxSupportedWeight
        }).FirstOrDefaultAsync(l => l.ID == id);

        if (launchpad == null)
            return NotFound();

        return launchpad;
    }

    // POST: api/Launchpads
    // Creates an launchpad
    [HttpPost]
    public async Task<ActionResult<Launchpad>> CreateLaunchpad(Launchpad launchpad) {

        if (launchpad.ID < 0)
            return BadRequest("Invalid value: Launchpad.ID: Must not be negative!");

        if(launchpad.MaxSupportedWeight < 0)
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");
        
        _context.Launchpads.Add(launchpad);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetLaunchpad), new { id = launchpad.ID }, launchpad);
    }

    // PUT: api/Launchpads/{id}
    // Updates launchpad on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLaunchpad(int id, Launchpad launchpad) {
        if (id != launchpad.ID)
            return BadRequest();
        
        if (launchpad.ID < 0)
            return BadRequest("Invalid value: Launchpad.ID: Must not be negative!");

        if(launchpad.MaxSupportedWeight < 0)
            return BadRequest("ERROR!: Launchpad.MaxSupportedWeight: Value cannot be negative!");


        _context.Entry(launchpad).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Launchpads.Any(l => l.ID == id))
                return NotFound();
            throw;
        }

        //logging
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

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}