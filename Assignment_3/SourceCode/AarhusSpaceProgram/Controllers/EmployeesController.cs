using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .Select(e => new
            {
                e.ID,
                e.FullName,
                e.HireDate,

                Department = e.Department != null ? e.Department.name : null,

                // Roles (only one or none typically)
                IsAstronaut = e.Astronaut != null,
                IsScientist = e.Scientist != null,
                IsManager = e.Manager != null
            })
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        
        var employee = await _context.Employees
            .Where(e => e.ID == id)
            .Select(e => new
            {
                e.ID,
                e.FullName,
                e.HireDate,

                Department = e.Department != null ? e.Department.name : null,

                // Role details (expanded a bit more)
                Astronaut = e.Astronaut == null ? null : new
                {
                    e.Astronaut.ID
                },

                Scientist = e.Scientist == null ? null : new
                {
                    e.Scientist.ID
                },

                Manager = e.Manager == null ? null : new
                {
                    e.Manager.ID
                }
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        if (employee.ID < 0)
            return BadRequest("Invalid value: Employee.ID: Must not be negative!");

        // Basic validation
        if (string.IsNullOrWhiteSpace(employee.FullName))
            return BadRequest("Full name is required.");

        if (employee.HireDate > DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Hire date cannot be in the future.");

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
    }

    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
    {
        if (id != updatedEmployee.ID)
            return BadRequest();

        if (updatedEmployee.ID < 0)
            return BadRequest("Invalid value: Employee.ID: Must not be negative!");

        var existingEmployee = await _context.Employees.FindAsync(id);

        if (existingEmployee == null)
            return NotFound();

        // Validation
        if (string.IsNullOrWhiteSpace(updatedEmployee.FullName))
            return BadRequest("Full name is required.");

        if (updatedEmployee.HireDate > DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("Hire date cannot be in the future.");

        // Update fields
        existingEmployee.FullName = updatedEmployee.FullName;
        existingEmployee.HireDate = updatedEmployee.HireDate;
        existingEmployee.FK_DepartmentID = updatedEmployee.FK_DepartmentID;

        await _context.SaveChangesAsync();

        //logging
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

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}