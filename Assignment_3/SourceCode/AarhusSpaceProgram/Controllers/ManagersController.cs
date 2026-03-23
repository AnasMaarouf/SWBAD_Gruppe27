using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ManagersController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<ManagersController> _logger;

    public ManagersController(MainDBContext context, ILogger<ManagersController> logger) {
        _context = context;
        _logger = logger;
    }

    // GET: api/Managers
    // Gets all Managers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManagerDto>>> GetManagers()
    {
        return await _context.Managers
            .Select(m => new ManagerDto {
                ID = m.ID,
                FullName = m.Employee != null ? m.Employee.FullName : null
            })
            .ToListAsync();
    }
    // GET: api/Managers/{id}
    // Gets manager from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<ManagerDto>> GetManager(int id)
    {
        var manager = await _context.Managers
            .Where(m => m.ID == id)
            .Select(m => new ManagerDto {
                ID = m.ID,
                FullName = m.Employee != null ? m.Employee.FullName : null
            })
            .FirstOrDefaultAsync();

        if (manager == null)
            return NotFound();

        return Ok(manager);
    }

    // POST: api/Managers
    // Creates an manager
    [HttpPost]
    public async Task<ActionResult<ManagerDto>> CreateManager(ManagerCreateDto dto)
    {
        var manager = new Manager {
            ID = dto.EmployeeID
        };

        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetManager), new { id = manager.ID }, new ManagerDto {
            ID = manager.ID
        });
    }

    // PUT: api/Managers/{id}
    // Updates manager on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(int id, ManagerUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest("ERROR!: id and Manager.ID, not consistent!");

        var manager = await _context.Managers.FindAsync(id);
        if (manager == null)
            return NotFound();

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

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

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}