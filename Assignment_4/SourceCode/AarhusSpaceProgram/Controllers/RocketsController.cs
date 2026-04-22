using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    // Gets all Rockets
    [HttpGet]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<RocketResponseDTO>>> GetRockets() {
        var rockets = await _context.Rockets
            .Include(r => r.Mission)
            .Select(dto => new RocketResponseDTO {
                Id = dto.ID,
                ModelName = dto.ModelName,
                FuelCapacity = dto.FuelCapacity,
                CrewCapacity = dto.CrewCapacity,
                NumberOfStages = dto.NumberOfStages,
                TotalWeight = dto.TotalWeight,
                assignedMission = dto.Mission.Name
            }).ToListAsync();
        
        if(rockets == null)
            return NotFound("No rockets exists");

        return Ok(rockets);
    }

    // GET: api/celestialBodies/{id}
    // Gets rocket from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<object>> GetRocket(int id)
    {
        var rocket = await _context.Rockets
            .Include(r => r.Mission)
            .Select(dto => new RocketResponseDTO {
                Id = dto.ID,
                ModelName = dto.ModelName,
                FuelCapacity = dto.FuelCapacity,
                CrewCapacity = dto.CrewCapacity,
                NumberOfStages = dto.NumberOfStages,
                TotalWeight = dto.TotalWeight,
                assignedMission = dto.Mission.Name
            }).FirstOrDefaultAsync(r => r.Id == id);

        if (rocket == null)
            return NotFound("Rocket " + id + " does not exist");

        return Ok(rocket);
    }

    // POST: api/Rockets
    // Creates an rocket
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Rocket>> CreateRocket(CreateRocketDTO dto) {
       
        // Variable value Validation
        if(dto.FuelCapacity < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.FuelCapacity: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.CrewCapacity < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.CrewCapacity: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.NumberOfStages < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.NumberOfStages: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.TotalWeight < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.TotalWeight: Variable value cannot be negative!");
        }

        var rocket = new Rocket {
            CrewCapacity = dto.CrewCapacity,
            FuelCapacity = dto.FuelCapacity,
            ModelName = dto.ModelName,
            NumberOfStages = dto.NumberOfStages,
            TotalWeight = dto.TotalWeight
        };

        _context.Rockets.Add(rocket);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetRocket), new { id = rocket.ID }, rocket);
    }

    // PUT: api/Rockets/{id}
    // Updates rocket on id
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UpdateRocket(int id, UpdateRocketDTO dto) {

        // Variable value Validation
        if(dto.FuelCapacity < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.FuelCapacity: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.CrewCapacity < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.CrewCapacity: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.NumberOfStages < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.NumberOfStages: Variable value cannot be negative!");
        }
        // Variable value Validation
        if(dto.TotalWeight < 0) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Invalid value: Rocket.TotalWeight: Variable value cannot be negative!");
        }
        
        
        var rocket = await _context.Rockets
            .FirstOrDefaultAsync(r => r.ID == id);

        if(rocket == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Rocket does not exist");
        }
        
        rocket.CrewCapacity = dto.CrewCapacity;
        rocket.FuelCapacity = dto.FuelCapacity;
        rocket.ModelName = dto.ModelName;
        rocket.NumberOfStages = dto.NumberOfStages;
        rocket.TotalWeight = dto.TotalWeight;

        await _context.SaveChangesAsync();

        {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }

    // DELETE: api/Rockets/{id}
    // Deletes rocket by id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> DeleteRocket(int id)
    {
        var rocket = await _context.Rockets.FindAsync(id);
        
        if (rocket == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        
            return NotFound("Rocket does not exist");
        }
        _context.Rockets.Remove(rocket);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        
        return NoContent();
    }
}