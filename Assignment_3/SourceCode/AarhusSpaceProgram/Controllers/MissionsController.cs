using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    private readonly MainDBContext _context;

    public MissionsController(MainDBContext context) {
        _context = context;
    }

    // GET: api/missions
    // Gets all missions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetMissions() {
        return await _context.Missions.Select(m_dto => new {
            m_dto.ID,
            m_dto.Name,
            m_dto.CurrentStatus,
            m_dto.LaunchDate,
            m_dto.AssignedRocket.ModelName,

            manager_name = m_dto.manager.Employee.FullName,

            rocket_model = m_dto.AssignedRocket.ModelName,

            launchpad_Location = m_dto.launchpad.Location,

            target_celestial_body = m_dto.celestialBody.Name
        }).ToListAsync();
    }

    // GET: api/missions/{id}
    // Gets mission from id (primary key)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMission(int id)
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
            .Select(m => new
            {
                m.Name,

                Astronauts = m.crew.joint_Astronaut_Crew
                    .Select(j => j.astronaut.Employee.FullName)
                    .ToList(),

                Scientists = m.joint_scientist_missions
                    .Select(js => js.scientist.Employee.FullName)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (mission == null) return NotFound();

        return Ok(mission);
    }

    // POST: api/missions
    [HttpPost]
    public async Task<ActionResult<Mission>> CreateMission(Mission mission) {
        // If id is negative return bad request 
        if (mission.ID < 0)
            return BadRequest("Invalid value: Mission.ID: Must not be negative!");

        if (mission.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow)) {
            return BadRequest("Launch date cannot be in the past.");
        }

        if (mission.FK_launchpadID != null) {
            var LaunchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == mission.FK_launchpadID &&
                    m.LaunchDate == mission.LaunchDate
                );

            if (LaunchPadIsReserved)
                return BadRequest("Launchpad already has a mission scheduled for this date.");
        }

        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMission), new { id = mission.ID }, mission);
    }

    // PUT: api/missions/{id}
    // Updates mission on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMission(int id, Mission mission) {
        if (id != mission.ID)
            return BadRequest("ERROR!: Invalid value: \"id\" and \"Mission.ID\" are not the same!");
        
        if (mission.Duration < 0)
            return BadRequest("Invalid value: Mission.Duration: Must not be negative!");

        // Get old object, for the sake of validation.
        var oldMission = await _context.Missions.FindAsync(id);
        if(oldMission == null)
            return NotFound();

        // Status variable value Validation
        if((oldMission.CurrentStatus.Equals(Mission.Status.Created) || oldMission.CurrentStatus.Equals(Mission.Status.Completed)) && mission.CurrentStatus.Equals(Mission.Status.Active))
            return BadRequest("ERROR!: Status cannot move directly from \"Created\" to \"Active\", or from \"Completed\" back to \"Active\"!");
        
        if(!oldMission.CurrentStatus.Equals(Mission.Status.Active) &&  (mission.CurrentStatus.Equals(Mission.Status.Completed)  ||
                                                                        mission.CurrentStatus.Equals(Mission.Status.Failed)     ||
                                                                        mission.CurrentStatus.Equals(Mission.Status.Aborted)    ))
            return BadRequest("ERROR!: Only \"Active\" missions can become \"Completed\", \"Failed\", or \"Aborted\"!");
        
        if (mission.LaunchDate < DateOnly.FromDateTime(DateTime.UtcNow)) {
            return BadRequest("Launch date cannot be in the past.");
        }
        
        if (mission.FK_launchpadID != null) {
            var LaunchPadIsReserved = await _context.Missions
                .AnyAsync(m =>
                    m.FK_launchpadID == mission.FK_launchpadID &&
                    m.LaunchDate == mission.LaunchDate
                );

            if (LaunchPadIsReserved)
                return BadRequest("Launchpad already has a mission scheduled for this date.");
        }

        _context.Entry(mission).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (_context.Missions.Any(e => e.ID == id))
                return NotFound();
            throw;
        }

        return Ok();
    }

    // DELETE: api/missions/{id}
    // Deletes Mission by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMission(int id) {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null)
            return NotFound();

        _context.Missions.Remove(mission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}