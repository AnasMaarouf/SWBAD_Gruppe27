using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<EmployeesController> _logger; 

    public EmployeesController(MainDBContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        return await _context.Employees
            .Select(e => new EmployeeDto {
                ID = e.ID,
                FullName = e.FullName,
                HireDate = e.HireDate,
                Department = e.Department != null ? e.Department.name : null,
                IsAstronaut = e.Astronaut != null,
                IsScientist = e.Scientist != null,
                IsManager = e.Manager != null
            })
            .ToListAsync();
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Where(e => e.ID == id)
            .Select(e => new EmployeeDto {
                ID = e.ID,
                FullName = e.FullName,
                HireDate = e.HireDate,
                Department = e.Department != null ? e.Department.name : null,
                IsAstronaut = e.Astronaut != null,
                IsScientist = e.Scientist != null,
                IsManager = e.Manager != null,
                Astronaut = e.Astronaut == null ? null : new AstronautRoleDto { ID = e.Astronaut.ID },
                Scientist = e.Scientist == null ? null : new ScientistRoleDto { ID = e.Scientist.ID },
                Manager = e.Manager == null ? null : new ManagerRoleDto { ID = e.Manager.ID }
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest("Full name is required.");

        if (dto.HireDate > DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Hire date cannot be in the future.");

        var employee = new Employee {
            FullName = dto.FullName,
            HireDate = dto.HireDate,
            FK_DepartmentID = dto.FK_DepartmentID
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, new EmployeeDto {
            ID = employee.ID,
            FullName = employee.FullName,
            HireDate = employee.HireDate
        });
    }

    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest("Full name is required.");

        if (dto.HireDate > DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Hire date cannot be in the future.");

        existingEmployee.FullName = dto.FullName;
        existingEmployee.HireDate = dto.HireDate;
        existingEmployee.FK_DepartmentID = dto.FK_DepartmentID;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}