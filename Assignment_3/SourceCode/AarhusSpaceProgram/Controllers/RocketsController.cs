using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RocketsController : ControllerBase
{
    private readonly MainDBContext _context;

    public RocketsController(MainDBContext context)
    {
        _context = context;
    }

    // GET: api/Rockets
    // Gets all Rockets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetRockets() {
        return await _context.Rockets.Select(r_dto => new {
            r_dto.ID,
            r_dto.ModelName,
            r_dto.FuelCapacity,
            r_dto.CrewCapacity,
            r_dto.NumberOfStages,
            r_dto.TotalWeight,
            r_dto.Mission
        }).ToListAsync();
    }

    // GET: api/celestialBodies/{id}
    // Gets rocket from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRocket(int id)
    {
        var rocket = await _context.Rockets.Select(r_dto => new {
            r_dto.ID,
            r_dto.ModelName,
            r_dto.FuelCapacity,
            r_dto.CrewCapacity,
            r_dto.NumberOfStages,
            r_dto.TotalWeight,
            r_dto.Mission
        }).FirstOrDefaultAsync(r => r.ID == id);

        if (rocket == null)
            return NotFound();

        return rocket;
    }

    // POST: api/Rockets
    // Creates an rocket
    [HttpPost]
    public async Task<ActionResult<Rocket>> CreateRocket(Rocket rocket) {
        // If id is negative return bad request 
        if (rocket.ID < 0)
            return BadRequest("Invalid value: Rocket.ID: Must not be negative!");

        // Variable value Validation
        if(rocket.FuelCapacity < 0)
            return BadRequest("Invalid value: Rocket.FuelCapacity: Variable value cannot be negative!");
        
        // Variable value Validation
        if(rocket.CrewCapacity < 0)
            return BadRequest("Invalid value: Rocket.CrewCapacity: Variable value cannot be negative!");

        // Variable value Validation
        if(rocket.NumberOfStages < 0)
            return BadRequest("Invalid value: Rocket.NumberOfStages: Variable value cannot be negative!");
        
        // Variable value Validation
        if(rocket.TotalWeight < 0)
            return BadRequest("Invalid value: Rocket.TotalWeight: Variable value cannot be negative!");

        _context.Rockets.Add(rocket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRocket), new { id = rocket.ID }, rocket);
    }

    // PUT: api/Rockets/{id}
    // Updates rocket on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRocket(int id, Rocket rocket) {
        // ID 
        if (id != rocket.ID)
            return BadRequest("Invalid value: \"id\" and \"Rocket.ID\" are not the same!");

        // Variable value Validation
        if(rocket.FuelCapacity < 0)
            return BadRequest("Invalid value: Rocket.FuelCapacity: Variable value cannot be negative!");
        
        // Variable value Validation
        if(rocket.CrewCapacity < 0)
            return BadRequest("Invalid value: Rocket.CrewCapacity: Variable value cannot be negative!");

        // Variable value Validation
        if(rocket.NumberOfStages < 0)
            return BadRequest("Invalid value: Rocket.NumberOfStages: Variable value cannot be negative!");
        
        // Variable value Validation
        if(rocket.TotalWeight < 0)
            return BadRequest("Invalid value: Rocket.TotalWeight: Variable value cannot be negative!");
        
        _context.Entry(rocket).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (_context.Rockets.Any(r => r.ID == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Rockets/{id}
    // Deletes rocket by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRocket(int id)
    {
        var rocket = await _context.Rockets.FindAsync(id);
        if (rocket == null)
            return NotFound();

        _context.Rockets.Remove(rocket);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}