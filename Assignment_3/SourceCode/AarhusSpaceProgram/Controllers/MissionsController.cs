using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<MissionsController> _logger;

    public MissionsController(MainDBContext context, ILogger<MissionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/missions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MissionDto>>> GetMissions()
    {
        return await _context.Missions
            .Select(m => new MissionDto {
                ID = m.ID,
                Name = m.Name,
                CurrentStatus = (int)m.CurrentStatus,
                LaunchDate = m.LaunchDate,
                ManagerName = m.manager != null ? m.manager.Employee.FullName : null,
                RocketModel = m.AssignedRocket != null ? m.AssignedRocket.ModelName : null,
                LaunchpadLocation = m.launchpad != null ? m.launchpad.Location : null,
                TargetCelestialBody = m.celestialBody != null ? m.celestialBody.Name : null
            })
            .ToListAsync();
    }

    // GET: api/missions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MissionDto>> GetMission(int id)
    {
        var mission = await _context.Missions
            .Where(m => m.ID == id)
            .Include(m => m.crew)
                .ThenInclude(c => c.joint_Astronaut_Crew)
                    .ThenInclude(j => j.astronaut)
                        .ThenInclude(a => a.Employee)
            .Include(m => m.joint_scientist_missions)
                .ThenInclude(js => js.scientist)
                    .ThenInclude(s => s.Employee)
            .Select(m => new MissionDto {
                ID = m.ID,
                Name = m.Name,
                CurrentStatus = (int)m.CurrentStatus,
                LaunchDate = m.LaunchDate,
                Astronauts = m.crew != null ? m.crew.joint_Astronaut_Crew
                    .Select(j => j.astronaut.Employee.FullName)
                    .ToList() : null,
                Scientists = m.joint_scientist_missions
                    .Select(js => js.scientist.Employee.FullName)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (mission == null)
            return NotFound();

        return Ok(mission);
    }

    // POST: api/missions
    [HttpPost]
    public async Task<ActionResult<MissionDto>> CreateMission(MissionCreateDto dto)
    {
        if (dto.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest("Launch date cannot be in the past.");

        if (dto.FK_launchpadID != null)
        {
            var launchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.FK_launchpadID &&
                    m.LaunchDate == dto.LaunchDate);

            if (launchPadIsReserved)
                return BadRequest("Launchpad already has a mission scheduled for this date.");
        }

        var mission = new Mission {
            Name = dto.Name,
            Duration = dto.Duration,
            CurrentStatus = dto.CurrentStatus,
            LaunchDate = dto.LaunchDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            FK_RocketID = dto.FK_RocketID,
            FK_launchpadID = dto.FK_launchpadID,
            FK_CrewID = dto.FK_CrewID,
            FK_ManagerID = dto.FK_ManagerID,
            FK_CelestialID = dto.FK_CelestialID
        };

        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetMission), new { id = mission.ID }, new MissionDto {
            ID = mission.ID,
            Name = mission.Name
        });
    }

    // PUT: api/missions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMission(int id, MissionUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest("ERROR!: Invalid value: \"id\" and \"Mission.ID\" are not the same!");

        if (dto.Duration < 0)
            return BadRequest("Invalid value: Mission.Duration: Must not be negative!");

        var oldMission = await _context.Missions.FindAsync(id);
        if (oldMission == null)
            return NotFound();

        var newStatus = (Mission.Status)dto.CurrentStatus;

        if ((oldMission.CurrentStatus.Equals(Mission.Status.Created) || oldMission.CurrentStatus.Equals(Mission.Status.Completed)) && newStatus.Equals(Mission.Status.Active))
            return BadRequest("ERROR!: Status cannot move directly from \"Created\" to \"Active\", or from \"Completed\" back to \"Active\"!");

        if (!oldMission.CurrentStatus.Equals(Mission.Status.Active) && (newStatus.Equals(Mission.Status.Completed) ||
                                                                         newStatus.Equals(Mission.Status.Failed) ||
                                                                         newStatus.Equals(Mission.Status.Aborted)))
            return BadRequest("ERROR!: Only \"Active\" missions can become \"Completed\", \"Failed\", or \"Aborted\"!");

        if (dto.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest("Launch date cannot be in the past.");

        if (dto.FK_launchpadID != null)
        {
            var launchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == dto.FK_launchpadID &&
                    m.LaunchDate == dto.LaunchDate &&
                    m.ID != id);

            if (launchPadIsReserved)
                return BadRequest("Launchpad already has a mission scheduled for this date.");
        }

        oldMission.Name = dto.Name;
        oldMission.Duration = dto.Duration;
        oldMission.CurrentStatus = dto.CurrentStatus;
        oldMission.LaunchDate = dto.LaunchDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        oldMission.FK_RocketID = dto.FK_RocketID;
        oldMission.FK_launchpadID = dto.FK_launchpadID;
        oldMission.FK_CrewID = dto.FK_CrewID;
        oldMission.FK_ManagerID = dto.FK_ManagerID;
        oldMission.FK_CelestialID = dto.FK_CelestialID;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return Ok();
    }

    // DELETE: api/missions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMission(int id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null)
            return NotFound();

        _context.Missions.Remove(mission);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}