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
            .Include(e => e.department)
            .Include(e => e.astronaut)
            .Include(e => e.scientist)
            .Include(e => e.manager)
            .Select(e => new EmployeeResponseDTO
            {
                Id = e.ID,
                FullName = e.FullName,
                DepartmentName = e.department != null ? e.department.name : null,
            })
            .ToListAsync();

        if(employees == null) {
            return NotFound("No employees exist");
        }

        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        
        var employee = await _context.Employees
            .Include(e => e.department)
            .Include(e => e.astronaut)
            .Include(e => e.scientist)
            .Include(e => e.manager)
            .Select(e => new DetailedEmployeeResponseDTO
            {
                Id = e.ID,
                FullName = e.FullName,
                HireDate = e.HireDate,
                DepartmentName = e.department != null ? e.department.name : null,

                Role = e.astronaut != null ? "Astronaut" :
                    e.scientist != null ? "Scientist" :
                    e.manager != null ? "Manager" :
                    "None",
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO dto)
    {
        
        // Basic validation
        if (string.IsNullOrWhiteSpace(dto.FullName)) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            
            return BadRequest("Full name is required.");
        }

        if (dto.HireDate > DateOnly.FromDateTime(DateTime.Now)) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Hire date cannot be in the future.");
        }

        var employee = new Employee
        {
            FullName = dto.FullName,
            HireDate = dto.HireDate,
            FK_DepartmentID = dto.DepartmentId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
    }

    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDTO dto)
    {
        var existingEmployee = await _context.Employees.FindAsync(id);

        if (existingEmployee == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Employee does not exist");
        }

        var existingDepartment = await _context.Departments.FindAsync(dto.DepartmentID);

        if (existingDepartment == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Department does not exist");
        }

        // Validation
        if (string.IsNullOrWhiteSpace(dto.FullName)){
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);

            return BadRequest("Full name is required.");
        }

        // Update fields
        existingEmployee.FullName = dto.FullName;
        existingEmployee.HireDate = dto.HireDate;
        existingEmployee.FK_DepartmentID = dto.DepartmentID;

        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null) {
            // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Employee does not exist");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        {   // Logging
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }
}