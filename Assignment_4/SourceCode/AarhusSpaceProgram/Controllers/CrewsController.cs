using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<CrewResponseDTO>>> GetCrews()
    {
        var crews = _context.Crews
            .Select(c => new CrewResponseDTO
            {
                Id = c.ID,
                Missions = c.Missions.Select(m => m.Name).ToList(),
                Astronauts = c.joint_Astronaut_Crew.Select(a => a.astronaut.employee.FullName).ToList()
            })
            .ToListAsync();

        if (crews == null)
            return NotFound("No crews can be listed");

        return await crews;
    }

    // GET: api/Crews/{id}
    // Gets crew from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<CrewResponseDTO>> GetCrew(int id)
    {
        var crew = await _context.Crews
            .Include(c => c.Missions)
            .Include(c => c.joint_Astronaut_Crew)
                .ThenInclude(j => j.astronaut)
                    .ThenInclude(a => a.employee)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (crew == null)
            return NotFound("Crew " + id + " doesn't exist");

        var result = new CrewResponseDTO
        {
            Id = crew.ID,

            Missions = crew.Missions?
                .Select(m => m.Name)
                .ToList() ?? new List<string>(),

            Astronauts = crew.joint_Astronaut_Crew?
                .Select(j => j.astronaut.employee.FullName)
                .ToList() ?? new List<string>()
        };

        return Ok(result);
    }

    // POST: api/Crews
    // Creates an crew
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Crew>> CreateCrew(CreateCrewDTO dto) {
        var crew = new Crew{};
        _context.Crews.Add(crew);
        await _context.SaveChangesAsync();

        
        {   //logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetCrew), new { id = crew.ID }, crew);
    }

    // DELETE: api/Crews/{id}
    // Deletes crew by id
    [HttpDelete("id")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> DeleteCrew(int id)
    {
        var crew = await _context.Crews.FindAsync(id);
        if (crew == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound();
        }

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        _context.Crews.Remove(crew);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}