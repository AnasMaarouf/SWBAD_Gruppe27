using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    // Gets all Scientists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScientistResponseDto>>> GetScientists() {
        var scientists = await _context.Scientists
            .Select(s => new ScientistResponseDto
            {
                Id = s.ID,
                FullName = s.Employee.FullName,
                Title = s.Title,
                Specialty = s.Specialty,
                Missions = s.joint_scientist_missions.Select(m => m.mission.Name).ToList()
            })
            .ToListAsync();

        if(scientists == null) {
            return NotFound("No scientists exist");
        }

        return Ok(scientists);
    }

    // GET: api/celestialBodies/{id}
    // Gets scientist from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<ScientistResponseDto>> GetScientist(int id) {
        var scientist = await _context.Scientists
            .Select(s => new ScientistResponseDto
            {
                Id = s.ID,
                FullName = s.Employee.FullName,
                Title = s.Title,
                Specialty = s.Specialty,
                Missions = s.joint_scientist_missions.Select(m => m.mission.Name).ToList()
            })
            .FirstOrDefaultAsync(r => r.Id == id);

        if (scientist == null)
            return NotFound("No scientist with the id " + id);

        return scientist;
    }

    // POST: api/Scientists
    // Creates an scientist
    [HttpPost]
    public async Task<ActionResult<Scientist>> CreateScientist(CreateScientistDto dto) {
        var employee = await _context.Employees
            .Where(e => e.ID == dto.EmployeeId)
            .FirstOrDefaultAsync();

        if(employee == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("employee doesnt exist");
        }

        var scientist = new Scientist {
            ID = dto.EmployeeId,
            Title = dto.Title,
            Specialty = dto.Specialty
        };

        _context.Scientists.Add(scientist);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetScientist), new { id = scientist.ID }, scientist);
    }

    // PUT: api/Scientists/{id}
    // Updates scientist on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScientist(int id, UpdateScientistDto dto) {
        var scientist = await _context.Scientists
            .Where(e => e.ID == id)
            .FirstOrDefaultAsync();

        if(scientist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("scientist doesnt exist");
        }

        scientist.Title = dto.Title;
        scientist.Specialty = dto.Specialty;

        _context.Entry(scientist).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return Ok("scientist updated");
    }

    // DELETE: api/Scientists/{id}
    // Deletes scientist by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScientist(int id)
    {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("scientist didn't exist already");
        }

        _context.Scientists.Remove(scientist);
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }

    [HttpPost("{id}/assign-to-mission")]
    public async Task<IActionResult> AssignScientistToMission(int id, AssignScientistToMissionDto dto) {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Scientist not found");
        }

        var mission = await _context.Missions.FindAsync(dto.MissionID);
        if (mission == null){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Mission not found");
        }
        var exists = await _context.JointScientistMissions
            .AnyAsync(j => j.ScientistID == id && j.MissionID == dto.MissionID);

        if (exists) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Scientist already assigned to mission");
        }

        var joint = new Joint_Scientist_Mission
        {
            ScientistID = id,
            MissionID = dto.MissionID
        };

        _context.JointScientistMissions.Add(joint);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return Ok("Scientist assigned mission");
    }

    [HttpDelete("{id}/unassign-to-mission")]
    public async Task<IActionResult> UnassignScientistToMission(int id, UnassignScientistFromMissionDto dto) {
        var existing = await _context.JointScientistMissions
            .FirstOrDefaultAsync(j => j.ScientistID == id && j.MissionID == dto.MissionID);

        if (existing == null){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Scientist already unassigned to mission");
        }
        _context.JointScientistMissions.Remove(existing);
        await _context.SaveChangesAsync();

        {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Scientist unassigned mission");
    }
}
