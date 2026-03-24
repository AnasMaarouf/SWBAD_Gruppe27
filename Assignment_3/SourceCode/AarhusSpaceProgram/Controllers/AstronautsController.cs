using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AstronautsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<AstronautsController> _logger;

    public AstronautsController(MainDBContext context, ILogger<AstronautsController> logger) {
        _context = context;
        _logger = logger;
    }


    // GET: api/astronauts/{id}
    // Gets astronaut from id (primary key), and orders them based on flighthours
    [HttpGet("OrderByExperience")]
    public async Task<ActionResult<AstronautResponseDTO>> GetAstronaut_OrderByExperience()
    {
        var astronauts = await _context.Astronauts
            .Include(a => a.employee)
            .OrderByDescending(a => a.FlightHours)
            .Select(a => new {
                Name = a.employee!.FullName,
                a.Rank,
                a.FlightHours
            }).ToListAsync();

        if (astronauts == null) {
            return NotFound("No astronauts found");
        }
        
        return Ok(astronauts);
    }


    // GET: api/astronauts
    // Gets all astronauts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AstronautResponseDTO>>> GetAstronauts() {
        var astronauts = await _context.Astronauts.Select(a => new AstronautResponseDTO {
                Id = a.ID,
                FullName = a.employee.FullName,
                Rank = a.Rank,
                FlightHours = a.FlightHours,
                Paygrade = a.Paygrade,
                Crews = a.joint_Astronaut_Crew
                    .Select(j => j.crew.ID)
                    .ToList()
            })
            .ToListAsync();

        if(astronauts == null) {
            return NotFound("No astronauts found");
        }

        return Ok(astronauts);
    }

    // GET: api/astronauts/{id}
    // Gets astronaut from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<AstronautResponseDTO>> GetAstronaut(int id)
    {
        var astronaut = await _context.Astronauts
            .Select(a => new AstronautResponseDTO {
                Id = a.ID,
                FullName = a.employee.FullName,
                Rank = a.Rank,
                FlightHours = a.FlightHours,
                Paygrade = a.Paygrade,
                Crews = a.joint_Astronaut_Crew
                    .Select(j => j.crew.ID)
                    .ToList()
            })
            .FirstOrDefaultAsync(m => m.Id == id);

        if (astronaut == null) {
            return NotFound("Astronaut " + id + " not found");
        }

        return Ok(astronaut);
    }

    // POST: api/astronauts
    // Creates an astronaut
    [HttpPost]
    public async Task<ActionResult<CreateAstronautDTO>> CreateAstronaut(CreateAstronautDTO dto) {
        var ifExists = await _context.Astronauts.FirstOrDefaultAsync(a => a.ID == dto.EmployeeId);

        if(ifExists != null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 409, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return Conflict("Astronaut already exists");
        }

        var astronaut = new Astronaut {
            ID = dto.EmployeeId,
            Rank = dto.Rank,
            FlightHours = dto.FlightHours,
            Paygrade = dto.Paygrade
        };

        _context.Astronauts.Add(astronaut);
        await _context.SaveChangesAsync();
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return CreatedAtAction(nameof(GetAstronaut), new { id = astronaut.ID }, astronaut);
    }

    // PUT: api/astronauts/{id}
    // Updates astronaut on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAstronaut(int id, UpdateAstronautDTO astronaut) {
        var oldAstronaut = await _context.Astronauts.FindAsync(id);

        if (oldAstronaut == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound();
        }

        oldAstronaut.FlightHours = astronaut.FlightHours;
        oldAstronaut.Paygrade = astronaut.Paygrade;
        oldAstronaut.Rank = astronaut.Rank;

        _context.Entry(astronaut).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return NoContent();
    }

    // DELETE: api/astronauts/{id}
    // Deletes astronaut by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAstronaut(int id)
    {
        var astronaut = await _context.Astronauts.FindAsync(id);
        if (astronaut == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Astronaut doesn't exist");
        }

        _context.Astronauts.Remove(astronaut);
        await _context.SaveChangesAsync();

        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return NoContent();
    }

    [HttpPut("{id}/assign-to-crew")]
    public async Task<IActionResult> AssignAstronautToCrew(int id, AddAstronautToCrewDTO dto)
    {
        var astronautExists = await _context.Astronauts
            .AnyAsync(a => a.ID == id);

        var crewExists = await _context.Crews
            .AnyAsync(c => c.ID == dto.CrewId);

        if (!astronautExists) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Astronaut not found");
        }

        if (!crewExists) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Crew not found");
        }

        var crewAstronaut = new Joint_Astronaut_Crew
        {
            AstronautID = id,
            CrewID = dto.CrewId
        };

        _context.JointAstronautCrews.Add(crewAstronaut);
        await _context.SaveChangesAsync();
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Astronaut assigned to crew");
    }

    [HttpPut("{id}/unassign-to-crew")]
    public async Task<IActionResult> UnassignAstronautToCrew(int id, RemoveAstronautToCrewDTO dto)
    {
        var relation = await _context.JointAstronautCrews
        .FirstOrDefaultAsync(ca =>
            ca.AstronautID == id &&
            ca.CrewID == dto.CrewId);

        if(relation == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Astronaut not assigned this crew");
        }
        _context.JointAstronautCrews.Remove(relation);
        await _context.SaveChangesAsync();
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return NoContent();
    }

}

