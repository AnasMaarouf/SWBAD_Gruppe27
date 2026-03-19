using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CrewsController : ControllerBase
{
    private readonly MainDBContext _context;

    public CrewsController(MainDBContext context)
    {
        _context = context;
    }

    // GET: api/Crews
    // Gets all Crews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Crew>>> GetCrews()
    {
        return await _context.Crews
            .ToListAsync();
    }

    // GET: api/celestialBodies/{id}
    // Gets crew from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<Crew>> GetCrew(int id)
    {
        var crew = await _context.Crews
            .FirstOrDefaultAsync(c => c.ID == id);

        if (crew == null)
            return NotFound();

        return crew;
    }

    // POST: api/Crews
    // Creates an crew
    [HttpPost]
    public async Task<ActionResult<Crew>> CreateCrew(Crew crew)
    {
        if (crew.ID < 0)
            return BadRequest("Invalid value: <Crew>.ID: Must not be negative!");

        _context.Crews.Add(crew);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCrew), new { id = crew.ID }, crew);
    }

    // PUT: api/Crews/{id}
    // Updates crew on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCrew(int id, Crew crew) {
        if (id != crew.ID)
            return BadRequest();

        if (crew.ID < 0)
            return BadRequest("Invalid value: Crew.ID: Must not be negative!");

        _context.Entry(crew).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Crews.Any(c => c.ID == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Crews/{id}
    // Deletes crew by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCrew(int id)
    {
        var crew = await _context.Crews.FindAsync(id);
        if (crew == null)
            return NotFound();

        _context.Crews.Remove(crew);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}