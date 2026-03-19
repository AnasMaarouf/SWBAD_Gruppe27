using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ManagersController : ControllerBase
{
    private readonly MainDBContext _context;

    public ManagersController(MainDBContext context) {
        _context = context;
    }

    // GET: api/Managers
    // Gets all Managers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetManagers(){
        return await _context.Managers
            .Include(m => m.Employee)
            .ToListAsync();
    }

    // GET: api/Managers/{id}
    // Gets manager from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<Manager>> GetManager(int id)
    {
        var manager = await _context.Managers
            .Include(m => m.Employee)
            .FirstOrDefaultAsync(m => m.ID == id);

        if (manager == null)
            return NotFound();

        return manager;
    }

    // POST: api/Managers
    // Creates an manager
    [HttpPost]
    public async Task<ActionResult<Manager>> CreateManager(Manager manager) {
        // If id is negative return bad request 
        if (manager.ID < 0)
            return BadRequest("Invalid value: Manager.ID: Must not be negative!");

        // if manager and emplyee id is not consistent, return bad request.
        if(!manager.ID.Equals(manager.Employee.ID))
            return BadRequest("ERROR!: Scientist.ID: Not consistent with Employee.ID");

        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetManager), new { id = manager.ID }, manager);
    }

    // PUT: api/Managers/{id}
    // Updates manager on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(int id, Manager manager) {
        if (id != manager.ID)
            return BadRequest("ERROR!: id and Manager.ID, not consistent!");

        _context.Entry(manager).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Managers.Any(m => m.ID == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Managers/{id}
    // Deletes manager by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
            return NotFound();

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}