using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ScientistsController : ControllerBase
{
    private readonly MainDBContext _context;

    public ScientistsController(MainDBContext context)
    {
        _context = context;
    }

    // GET: api/Scientists
    // Gets all Scientists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetScientists() {
        var result = await _context.Scientists
            .Select(s => new
            {
                s.ID,
                s.Employee.FullName,
                Missions = s.joint_scientist_missions
                    .Select(j => j.mission.Name)
                    .ToList()
            }).ToListAsync();

        return Ok(result);
    }

    // GET: api/celestialBodies/{id}
    // Gets scientist from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetScientist(int id) {
        var scientist = await _context.Scientists
            .Select(s => new {
                s.ID,
                s.Employee.FullName,
                Missions = s.joint_scientist_missions
                    .Select(j => j.mission.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync(r => r.ID == id);

        if (scientist == null)
            return NotFound();

        return scientist;
    }

    // POST: api/Scientists
    // Creates an scientist
    [HttpPost]
    public async Task<ActionResult<Scientist>> CreateScientist(Scientist scientist) {
        if(!scientist.ID.Equals(scientist.Employee.ID))
            return BadRequest("ERROR!: Scientist.ID: Not consistent with Employee.ID");

        _context.Scientists.Add(scientist);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScientist), new { id = scientist.ID }, scientist);
    }

    // PUT: api/Scientists/{id}
    // Updates scientist on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScientist(int id, Scientist scientist) {
        if(id != scientist.ID)
            return BadRequest("ERROR!: id and Scientist.ID, not consistent!");

        if(!scientist.ID.Equals(scientist.Employee.ID))
            return BadRequest("ERROR!: Scientist.ID: Not consistent with Employee.ID!");

        _context.Entry(scientist).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (_context.Scientists.Any(r => r.ID == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Scientists/{id}
    // Deletes scientist by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScientist(int id)
    {
        var scientist = await _context.Scientists.FindAsync(id);
        if (scientist == null)
            return NotFound();

        _context.Scientists.Remove(scientist);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}