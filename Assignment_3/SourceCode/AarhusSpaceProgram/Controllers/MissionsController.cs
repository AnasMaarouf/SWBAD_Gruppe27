using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<ManagersController> _logger;
    public MissionsController(MainDBContext context, ILogger<ManagersController> logger) {
        _context = context;
        _logger = logger;
    }

    // GET: api/missions
    // Gets all missions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MissionResponseDTO>>> GetMissions() {
        var missions = await _context.Missions
        .Include(m => m.AssignedRocket)
        .Include(m => m.manager)
        .Include(m => m.launchpad)
        .Include(m => m.celestialBody)
        .Select(dto => new MissionResponseDTO {
            Id = dto.ID,
            Name = dto.Name,
            ManagerName = dto.manager.Employee.FullName,
            CurrentStatus = dto.CurrentStatus,
            LaunchDate = dto.LaunchDate,
            RocketModelName = dto.AssignedRocket.ModelName,
            LaunchpadLocation = dto.launchpad.Location,
            Target_CelestialBodyName = dto.celestialBody.Name
        }).ToListAsync();
        
        if (missions == null)
            return NotFound("No missios found");

        return Ok(missions);
    }

    // Gets all missions with one specific celestialbody as target
    [HttpGet("{TargetCelestialBody}")]
    public async Task<ActionResult<IEnumerable<Mission>>> GetMissions(string TargetCelestialBody) {
        var missions = await _context.Missions
        .Include(m => m.celestialBody)
        .Select(dto => new MissionResponseDTO {
            Id = dto.ID,
            Name = dto.Name,
            ManagerName = dto.manager.Employee.FullName,
            CurrentStatus = dto.CurrentStatus,
            LaunchDate = dto.LaunchDate,
            RocketModelName = dto.AssignedRocket.ModelName,
            LaunchpadLocation = dto.launchpad.Location
        }).Where(m => m.Target_CelestialBodyName == TargetCelestialBody).ToListAsync();
        
        if (missions == null)
            return NotFound("No missions found");

        return Ok(missions);
    }

    // GET: api/missions/{id}
    // Gets mission from id (primary key)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMission(int id)
    {
        var mission = await _context.Missions
        .Include(m => m.AssignedRocket)
        .Include(m => m.manager)
        .Include(m => m.launchpad)
        .Include(m => m.celestialBody)
        .Include(m => m.crew)
        .Include(m => m.joint_scientist_missions.Select(a => a.scientist))
        .Select(dto => new MissionDetailedResponseDTO {
            Id = dto.ID,
            Name = dto.Name,
            ManagerName = dto.manager.Employee.FullName,
            CurrentStatus = dto.CurrentStatus,
            LaunchDate = dto.LaunchDate,
            RocketModelName = dto.AssignedRocket.ModelName,
            LaunchpadLocation = dto.launchpad.Location,
            Target_CelestialBodyName = dto.celestialBody.Name,
            Scientists = dto.joint_scientist_missions
                .Select(j => j.scientist.Employee.FullName)
                .ToList(),
            
            Astronauts = dto.crew.joint_Astronaut_Crew
                .Select(j => j.astronaut.employee.FullName)
                .ToList()
        }).FirstOrDefaultAsync(m => m.Id == id);

        if (mission == null)
            return NotFound("No mission found");

        return Ok(mission);
    }

    // POST: api/missions
    // Creates mission
    [HttpPost]
    public async Task<ActionResult<CreateMissionDTO>> CreateMission(CreateMissionDTO dto) {
        if (dto.LaunchpadId != null) {
            var LaunchPadExists = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.LaunchpadId
                );

            if(LaunchPadExists == null) {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
                _logger.LogInformation("Request called {@LogInfo}", logInfo);
                return NotFound("Launchpad does not exist");
            }

            var LaunchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.LaunchpadId &&
                    m.LaunchDate == dto.LaunchDate
                );

            if (LaunchPadIsReserved) {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
                _logger.LogInformation("Request called {@LogInfo}", logInfo);
               return BadRequest("Launchpad already has a mission scheduled for this date.");
            }
        }

        if (dto.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow)) {
            return BadRequest("Launch date cannot be in the past.");
        }

        var ManagerExists = await _context.Managers
                .AnyAsync(m => m.ID == dto.ManagerId);

        if (ManagerExists == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Manager does not exist");
        }

        

        var RocketExists = await _context.Rockets
                .AnyAsync(r => r.ID == dto.RocketId && r.Mission == null);

        if (!RocketExists) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Rocket does not exist or is reserved for a different mission");
        }

        var celestialBodyExists = await _context.CelestialBodies
                .AnyAsync(r => r.ID == dto.CelestialBodyId);

        if (celestialBodyExists == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("celestialBody does not exist");
        }

        var mission = new Mission
        {
            Name = dto.Name,
            Duration = dto.Duration,
            CurrentStatus = dto.Status,
            Type = dto.Type,
            LaunchDate = dto.LaunchDate,
            FK_RocketID = dto.RocketId,
            FK_launchpadID = dto.LaunchpadId,
            FK_CrewID = dto.CrewId,
            FK_ManagerID = dto.ManagerId,
            FK_CelestialID = dto.CelestialBodyId
        };

        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetMission), new { id = mission.ID }, mission);
    }

    // PUT: api/missions/{id}
    // Updates mission on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMission(int id, UpdateMissionDTO dto) {
        
        
        if (dto.LaunchpadId != null) {
            var LaunchPadExists = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.LaunchpadId
                );

            if(LaunchPadExists == null) {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
                _logger.LogInformation("Request called {@LogInfo}", logInfo);
                return NotFound("Launchpad does not exist");
            }

            var LaunchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.LaunchpadId &&
                    m.LaunchDate == dto.LaunchDate
                );

            if (LaunchPadIsReserved) {
                var timestamp = new DateTimeOffset(DateTime.UtcNow);
                var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
                _logger.LogInformation("Request called {@LogInfo}", logInfo);
               return BadRequest("Launchpad already has a mission scheduled for this date.");
            }
        }

        if (dto.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow)) {
            return BadRequest("Launch date cannot be in the past.");
        }

        var ManagerExists = await _context.Managers
                .AnyAsync(m => m.ID == dto.ManagerId);

        if (ManagerExists == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Manager does not exist");
        }

        

        var RocketExists = await _context.Rockets
                .AnyAsync(r => r.ID == dto.RocketId && r.Mission == null);

        if (!RocketExists) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Rocket does not exist or is reserved for a different mission");
        }

        var celestialBodyExists = await _context.CelestialBodies
                .AnyAsync(r => r.ID == dto.CelestialBodyId);

        if (celestialBodyExists == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("celestialBody does not exist");
        }


        var mission = await _context.Missions
            .FirstOrDefaultAsync(m => m.ID == id);

        if(mission == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Mission does not exist");
        }

        
        mission.Name = dto.Name;
        mission.Duration = dto.Duration;
        mission.CurrentStatus = dto.CurrentStatus;
        mission.Type = dto.Type;
        mission.LaunchDate = dto.LaunchDate;
        mission.FK_RocketID = dto.RocketId;
        mission.FK_launchpadID = dto.LaunchpadId;
        mission.FK_CrewID = dto.CrewId;
        mission.FK_ManagerID = dto.ManagerId;
        mission.FK_CelestialID = dto.CelestialBodyId;
        

        await _context.SaveChangesAsync();
        

        return Ok();
    }

    // DELETE: api/missions/{id}
    // Deletes Mission by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMission(int id) {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "Delete", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Mission does not exist");
        }
        _context.Missions.Remove(mission);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "Delete", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }

    // PUT: api/Missions/{id}/assign-crew-mission
    // Assign crew to mission
    [HttpPut("{id}/assign-crew-mission")]
    public IActionResult AssignCrewToMission(int id, AddCrewToMissionDTO dto)
    {
        var mission = _context.Missions.Find(id);
        var crew = _context.Crews.Find(id);

        if (mission == null || crew == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Mission or crew does not exist");
        }

        mission.FK_CrewID = dto.CrewID;
        mission.crew = crew;

        _context.SaveChanges();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Crew assigned to mission");
    }

    [HttpPost("{id}/Unassign-to-crew")]
    public async Task<IActionResult> UnassignCrewToMission(int id, RemoveCrewFromMissionDTO dto)
    {
        var mission = _context.Missions.Find(id);
        var crew = _context.Crews.Find(dto.CrewID);

        if (mission == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("mission doesn't exist");
        }
        if (mission.crew == crew || mission.FK_CrewID == dto.CrewID){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Mission is not assigned this crew");
        }

        mission.FK_CrewID = null;
        mission.crew = null;

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        _context.SaveChanges();
        return Ok("Crew unassigned to mission");
    }
}