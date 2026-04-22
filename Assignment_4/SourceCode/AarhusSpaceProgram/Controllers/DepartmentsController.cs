using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<IEnumerable<DepartmentResponseDTO>>> GetDepartments()
    {
        var departments = await _context.Departments.Select(dto => new DepartmentResponseDTO{
            ID = dto.ID,
            DepartmentName = dto.name,
            Manager = dto.manager.Employee.FullName
        }).ToListAsync();

        if(departments == null)
            return NotFound("No departments found");

        return Ok(departments);
    }

    // GET: api/Departments/{id}
    // Gets department from id (primary key)
    [HttpGet("{id}")]
    [Authorize(Roles = "Astronaut, Manager, Admin")]
    public async Task<ActionResult<DetailedDepartmentResponseDTO>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.employees)
            .Select(dto => new DetailedDepartmentResponseDTO{
                ID = dto.ID,
                DepartmentName = dto.name,
                Manager = dto.manager.Employee.FullName,
                employees = dto.employees.Select(e => e.FullName).ToList()
            })
            .FirstOrDefaultAsync(d => d.ID == id);

        if (department == null)
            return NotFound("Department " + id + " does not exist");

        return department;
    }

    // POST: api/Departments
    // Creates an department
    [HttpPost]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<Department>> CreateDepartment(CreateDepartmentDTO dto)
    {
        var managerExist = await _context.Managers.FirstOrDefaultAsync(d => d.ID == dto.ManagerID);
        if(managerExist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Manager does not exist");
        }

        var department = new Department
        {
            name = dto.DepartmentName,
            FK_managerID = dto.ManagerID
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetDepartment), new { id = department.ID }, department);
    }

    // PUT: api/Departments/{id}
    // Updates department on id
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDTO dto) {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.ID == id);
        
        if (!_context.Departments.Any(d => d.ID == id)) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound();
        }

        var managerExist = await _context.Managers.FirstOrDefaultAsync(d => d.ID == dto.ManagerID);
        if(managerExist == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound("Manager does not exist");
        }

        department.FK_managerID = dto.ManagerID;
        department.name = dto.DepartmentName;
                
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Department updated");
    }

    // DELETE: api/Departments/{id}
    // Deletes department by id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }
}