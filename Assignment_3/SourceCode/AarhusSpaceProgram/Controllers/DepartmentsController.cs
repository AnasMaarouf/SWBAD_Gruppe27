using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        return await _context.Departments
            .ToListAsync();
    }

    // GET: api/Departments/{id}
    // Gets department from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.manager)
            .FirstOrDefaultAsync(d => d.ID == id);

        if (department == null)
            return NotFound();

        return department;
    }

    // POST: api/Departments
    // Creates an department
    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment(Department department)
    {
        if (department.ID < 0)
            return BadRequest("Invalid value: Department.ID: Must not be negative!");

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetDepartment), new { id = department.ID }, department);
    }

    // PUT: api/Departments/{id}
    // Updates department on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, Department department) {
        if (id != department.ID)
            return BadRequest();

        if (department.ID < 0)
            return BadRequest("Invalid value: Department.ID: Must not be negative!");

        _context.Entry(department).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Departments.Any(d => d.ID == id))
                return NotFound();
            throw;
        }

        //logging
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

        //logging
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}