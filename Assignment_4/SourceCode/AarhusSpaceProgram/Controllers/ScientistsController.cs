using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScientistsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<ScientistsController> _logger;

    public ScientistsController(MainDBContext context, ILogger<ScientistsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Gets all Scientists
    // GET: api/Scientists
    [HttpGet]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<ScientistResponseDTO>>> GetScientists() {
        var scientists = await _context.Scientists
            .Select(s => new ScientistResponseDTO
            {
                Id = s.ID,
                FullName = s.Employee.FullName,
                Title = s.Title,
                Specialty = s.Specialty
            })
            .ToListAsync();

        if(scientists == null) {
            return NotFound("No scientists exist");
        }

        return Ok(scientists);
    }

    // Gets scientist from id (primary key)
    // GET: api/celestialBodies/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<DetailedScientistResponseDTO>> GetScientist(int id) {
        var scientist = await _context.Scientists
            .Select(s => new DetailedScientistResponseDTO
            {
                Id = s.ID,
                FullName = s.Employee.FullName,
                Title = s.Title,
                Specialty = s.Specialty,
                Missions = s.joint_scientist_missions.Select(m => m.mission.Name).ToList(),
                Experiments = s.joint_scientist_experiments.Select(e => e.experiment.Name).ToList()
            })
            .FirstOrDefaultAsync(r => r.Id == id);

        if (scientist == null)
            return NotFound("No scientist with the id " + id);

        return scientist;
    }

    // Creates an scientist
    // POST: api/Scientists
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Scientist>> CreateScientist(CreateScientistDTO dto) {
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

    // Updates scientist on id
    // PUT: api/Scientists/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UpdateScientist(int id, UpdateScientistDTO dto) {
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

    // Deletes scientist by id
    // DELETE: api/Scientists/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Admin")]
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
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Scientist deleted");
    }

    // Assign scientist to mission
    // POST: api/Scientists/{id}/assign-to-mission
    [HttpPost("{id}/assign-to-mission")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> AssignScientistToMission(int id, AssignScientistToMissionDTO dto) {
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

    // Assign scientist to mission
    // DELETE: api/Scientists/{id}/unassign-to-mission
    [HttpDelete("{id}/unassign-to-mission")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UnassignScientistToMission(int id, UnassignScientistFromMissionDTO dto) {
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

    // Assign scientist to experiment
    [HttpPost("{id}/assign-to-experiment")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> AssignScientistToExperiment(int id, AssignScientistToExperimentDTO dto) {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Scientist not found");
        }

        var experiment = await _context.Experiments.FindAsync(dto.ExperimentID);
        if (experiment == null){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Mission not found");
        }
        var exists = await _context.JointScientistExperiments
            .AnyAsync(j => j.ScientistID == id && j.ExperimentID == dto.ExperimentID);

        if (exists) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Scientist already assigned to experiment");
        }

        var joint = new Joint_Scientist_Experiment
        {
            ScientistID = id,
            ExperimentID = dto.ExperimentID
        };

        _context.JointScientistExperiments.Add(joint);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return Ok("Scientist assigned experiment");
    }

    // Unassign scientist to experiment
    [HttpDelete("{id}/unassign-to-experiment")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UnassignScientistToExperiment(int id, UnassignScientistFromExperimentDTO dto) {
        var existing = await _context.JointScientistExperiments
            .FirstOrDefaultAsync(j => j.ScientistID == id && j.ExperimentID == dto.ExperimentID);

        if (existing == null){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Scientist already unassigned to experiment");
        }
        _context.JointScientistExperiments.Remove(existing);
        await _context.SaveChangesAsync();

        {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Scientist unassigned experiment!");
    }
}
