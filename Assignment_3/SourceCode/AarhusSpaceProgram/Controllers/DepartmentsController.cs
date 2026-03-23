using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<DepartmentsController> _logger; 

    public DepartmentsController(MainDBContext context, ILogger<DepartmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Departments
    // Gets all Departments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        return await _context.Departments
            .Select(d => new DepartmentDto {
                ID = d.ID,
                Name = d.name,
                ManagerName = d.manager != null ? d.manager.Employee.FullName : null
            })
            .ToListAsync();
    }

    // GET: api/Departments/{id}
    // Gets department from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Where(d => d.ID == id)
            .Select(d => new DepartmentDto {
                ID = d.ID,
                Name = d.name,
                ManagerName = d.manager != null ? d.manager.Employee.FullName : null
            })
            .FirstOrDefaultAsync();

        if (department == null)
            return NotFound();

        return Ok(department);
    }

    // POST: api/Departments
    // Creates an department
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(DepartmentCreateDto dto)
    {
        var department = new Department {
            name = dto.Name,
            FK_managerID = dto.FK_ManagerID ?? 0
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetDepartment), new { id = department.ID }, new DepartmentDto {
            ID = department.ID,
            Name = department.name
        });
    }

    // PUT: api/Departments/{id}
    // Updates department on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        department.name = dto.Name;
        department.FK_managerID = dto.FK_ManagerID ?? 0;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/Departments/{id}
    // Deletes department by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}