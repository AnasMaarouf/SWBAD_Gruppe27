using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ScientistsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<ScientistsController> _logger;

    public ScientistsController(MainDBContext context, ILogger<ScientistsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Scientists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScientistDto>>> GetScientists()
    {
        var result = await _context.Scientists
            .Select(s => new ScientistDto {
                ID = s.ID,
                FullName = s.Employee.FullName,
                Missions = s.joint_scientist_missions
                    .Select(j => j.mission.Name)
                    .ToList()
            }).ToListAsync();

        return Ok(result);
    }

    // GET: api/Scientists/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ScientistDto>> GetScientist(int id)
    {
        var scientist = await _context.Scientists
            .Where(s => s.ID == id)
            .Select(s => new ScientistDto {
                ID = s.ID,
                FullName = s.Employee.FullName,
                Missions = s.joint_scientist_missions
                    .Select(j => j.mission.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (scientist == null)
            return NotFound();

        return Ok(scientist);
    }

    // POST: api/Scientists
    [HttpPost]
    public async Task<ActionResult<ScientistDto>> CreateScientist(ScientistCreateDto dto)
    {
        if (dto.ID != dto.FK_EmployeeID)
            return BadRequest("ERROR!: Scientist.ID: Not consistent with Employee.ID");

        var scientist = new Scientist {
            ID = dto.FK_EmployeeID
        };

        _context.Scientists.Add(scientist);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetScientist), new { id = scientist.ID }, new ScientistDto {
            ID = scientist.ID
        });
    }

    // PUT: api/Scientists/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScientist(int id, ScientistUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest("ERROR!: id and Scientist.ID, not consistent!");

        if (dto.ID != dto.FK_EmployeeID)
            return BadRequest("ERROR!: Scientist.ID: Not consistent with Employee.ID!");

        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
            return NotFound();

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/Scientists/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScientist(int id)
    {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
            return NotFound();

        _context.Scientists.Remove(scientist);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}