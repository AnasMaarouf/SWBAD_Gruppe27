using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<CreateManagerDTO>>> GetManagers(){
        
        var managers = await _context.Managers
            .Include(e => e.Employee)
            .Include(e => e.Departments)
            .Include(e => e.Missions)
            .Select(dto => new ManagerResponseDTO {
                Id = dto.Employee.ID,
                FullName = dto.Employee.FullName,
                Departments = dto.Departments.Select(d => d.name).ToList(),
                Missions = dto.Missions.Select(m => m.Name).ToList()
            })
            .ToListAsync();
        
        if(managers == null)
            return NotFound("No existing managers");

        return Ok(managers);
    }

    // GET: api/Managers/{id}
    // Gets manager from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<Manager>> GetManager(int id)
    {
        var manager = await _context.Managers
            .Include(e => e.Employee)
            .Include(e => e.Departments)
            .Include(e => e.Missions)
            .Select(dto => new ManagerResponseDTO {
                Id = dto.Employee.ID,
                FullName = dto.Employee.FullName,
                Departments = dto.Departments.Select(d => d.name).ToList(),
                Missions = dto.Missions.Select(m => m.Name).ToList()
            })
            .ToListAsync();

        if (manager == null)
            return NotFound("Manager " + id + " does not exist");

        return Ok(manager);
    }

    // POST: api/Managers
    // Creates an manager
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Manager>> CreateManager(CreateManagerDTO dto) {
        var employee = await _context.Employees
            .Where(e => e.ID == dto.EmployeeId)
            .FirstOrDefaultAsync();

        if(employee == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("employee doesnt exist");
        }

        var manager = new Manager {
            ID = dto.EmployeeId
        };

        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return CreatedAtAction(nameof(GetManager), new { id = manager.ID }, manager);
    }


    // DELETE: api/Managers/{id}
    // Deletes manager by id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        var manager = await _context.Managers.FindAsync(id);
        if (manager == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound();
        }

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }
        return NoContent();
    }
}