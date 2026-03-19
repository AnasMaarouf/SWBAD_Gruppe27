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
            m_dto.Duration,
            m_dto.CurrentStatus,
            m_dto.Type,
            m_dto.LaunchDate,
            m_dto.AssignedRocket.ModelName
        }).ToListAsync();
    }

    // GET: api/missions/{id}
    // Gets mission from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetMission(int id)
    {
        var mission = await _context.Missions.Select(m_dto => new {
            m_dto.ID,
            m_dto.Name,
            m_dto.Duration,
            m_dto.CurrentStatus,
            m_dto.Type,
            m_dto.LaunchDate,
            m_dto.AssignedRocket.ModelName
        }).FirstOrDefaultAsync(m => m.ID == id);

        if (mission == null)
            return NotFound();

        return mission;
    }

    // POST: api/missions
    [HttpPost]
    public async Task<ActionResult<Mission>> CreateMission(Mission mission) {
        // If id is negative return bad request 
        if (mission.ID < 0)
            return BadRequest("Invalid value: Rocket.ID: Must not be negative!");

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