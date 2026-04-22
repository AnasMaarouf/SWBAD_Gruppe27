using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExperimentsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<ExperimentsController> _logger;

    public ExperimentsController(MainDBContext context, ILogger<ExperimentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Experiments
    // Gets all Experiments
    [HttpGet]
    [Authorize(Roles = "Astronaut, Scientist, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<ExperimentResponseDTO>>> GetExperiments() {
        var experiments = await _context.Experiments
            .Select(e => new ExperimentResponseDTO
            {
                ID = e.ID,
                Name = e.Name,
                DayOfCreation = e.CreationDate,
                AssignedMissionName = e.mission.Name
            })
            .ToListAsync();

        if(experiments == null) {
            return NotFound("No experiments exist");
        }

        return Ok(experiments);
    }

    // GET: api/experiment/{id}
    // Gets experiment from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Scientist, Astronaut, Manager, Admin")]
    public async Task<ActionResult<DetailedExperimentResponseDTO>> GetExperiment(int id) {
        var experiment = await _context.Experiments
            .Select(e => new DetailedExperimentResponseDTO
            {
                ID = e.ID,
                Name = e.Name,
                Description = e.Description,
                DayOfCreation = e.CreationDate,
                AssignedMissionID = e.mission.ID,
                AssignedMissionName = e.mission.Name,
                AssignedScientists = e.joint_scientist_experiment.Select(s => s.scientist.Employee.FullName).ToList()
            })
            .FirstOrDefaultAsync(e => e.ID == id);

        if (experiment == null)
            return NotFound("No experiment with the id: " + id);

        return experiment;
    }

    // POST: api/experiments
    // Creates an experiment
    [HttpPost]
    [Authorize(Roles = "Scientist, Manager, Admin")]
    public async Task<ActionResult<Experiment>> CreateExperiment(CreateExperimentDTO dto) {
        var missionExists = await _context.Missions
            .Where(e => e.ID == dto.MissionID)
            .FirstOrDefaultAsync();

        if(missionExists == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Mission doesnt exist!");
        }

        var experiment = new Experiment {
            Name = dto.Name,
            Description = dto.Description,
            CreationDate = dto.CreationDate,
            FK_MissionID = dto.MissionID
        };

        _context.Experiments.Add(experiment);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetExperiment), new { id = experiment.ID }, experiment);
    }

    // PUT: api/Experiment/{id}
    // Updates experiment from id
    [HttpPut("{id}")]
    [Authorize(Roles = "Scientist, Manager, Admin")]
    public async Task<IActionResult> UpdateExperiment(int id, UpdateExperimentDTO dto) {
        var experiment = await _context.Experiments
            .Where(e => e.ID == id)
            .FirstOrDefaultAsync();

        if(experiment == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Experiment doesnt exist");
        }

        experiment.Name = dto.Name;
        experiment.Description = dto.Description;
        experiment.CreationDate = dto.CreationDate;
        experiment.FK_MissionID = dto.MissionID;

        _context.Entry(experiment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return Ok("Experiment updated");
    }

    // DELETE: api/Experiments/{id}
    // Deletes experiment by id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Scientist, Manager, Admin")]
    public async Task<IActionResult> DeleteExperiment(int id)
    {
        var experiment = await _context.Experiments.FindAsync(id);
        if (experiment == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Experiment didn't exist already");
        }

        _context.Experiments.Remove(experiment);
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "Delete", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }
}


