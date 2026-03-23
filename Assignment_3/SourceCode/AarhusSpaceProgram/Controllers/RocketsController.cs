using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class RocketsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<RocketsController> _logger;

    public RocketsController(MainDBContext context, ILogger<RocketsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Rockets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RocketDto>>> GetRockets()
    {
        return await _context.Rockets.Select(r => new RocketDto {
            ID = r.ID,
            ModelName = r.ModelName,
            FuelCapacity = r.FuelCapacity,
            CrewCapacity = r.CrewCapacity,
            NumberOfStages = r.NumberOfStages,
            TotalWeight = r.TotalWeight,
            MissionName = r.Mission != null ? r.Mission.Name : null
        }).ToListAsync();
    }

    // GET: api/Rockets/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RocketDto>> GetRocket(int id)
    {
        var rocket = await _context.Rockets
            .Where(r => r.ID == id)
            .Select(r => new RocketDto {
                ID = r.ID,
                ModelName = r.ModelName,
                FuelCapacity = r.FuelCapacity,
                CrewCapacity = r.CrewCapacity,
                NumberOfStages = r.NumberOfStages,
                TotalWeight = r.TotalWeight,
                MissionName = r.Mission != null ? r.Mission.Name : null
            }).FirstOrDefaultAsync();

        if (rocket == null)
            return NotFound();

        return Ok(rocket);
    }

    // POST: api/Rockets
    [HttpPost]
    public async Task<ActionResult<RocketDto>> CreateRocket(RocketCreateDto dto)
    {
        if (dto.Weight < 0)
            return BadRequest("Invalid value: Rocket.Weight: Variable value cannot be negative!");

        var rocket = new Rocket {
            ModelName = dto.ModelName,
            TotalWeight = (int?)dto.Weight
        };

        _context.Rockets.Add(rocket);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetRocket), new { id = rocket.ID }, new RocketDto {
            ID = rocket.ID,
            ModelName = rocket.ModelName
        });
    }

    // PUT: api/Rockets/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRocket(int id, RocketUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest("Invalid value: \"id\" and \"Rocket.ID\" are not the same!");

        if (dto.Weight < 0)
            return BadRequest("Invalid value: Rocket.Weight: Variable value cannot be negative!");

        var rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
            return NotFound();

        rocket.ModelName = dto.ModelName;
        rocket.TotalWeight = (int?)dto.Weight;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/Rockets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRocket(int id)
    {
        var rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
            return NotFound();

        _context.Rockets.Remove(rocket);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}