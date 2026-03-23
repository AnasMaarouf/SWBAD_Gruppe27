using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AarhusSpaceProgram.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CelestialBodiesController : ControllerBase
{
    private readonly MainDBContext _context;
    private readonly ILogger<CelestialBodiesController> _logger;

    public CelestialBodiesController(MainDBContext context, ILogger<CelestialBodiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/CelestialBodies
    // Gets all CelestialBodies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CelestialBodyDto>>> GetCelestialBodies()
    {
        return await _context.CelestialBodies
            .Select(c => new CelestialBodyDto {
                ID = c.ID,
                Name = c.Name,
                Distance = (double)c.Distance,
                BodyType = c.BodyType,
                PlanetType = c.PlanetType,
                ParentPlanetName = c.ParentPlanet != null ? c.ParentPlanet.Name : null
            })
            .ToListAsync();
    }

    // GET: api/celestialBodies/{id}
    // Gets celestialBody from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<CelestialBodyDto>> GetCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies
            .Where(c => c.ID == id)
            .Select(c => new CelestialBodyDto {
                ID = c.ID,
                Name = c.Name,
                Distance = (double)c.Distance,
                BodyType = c.BodyType,
                PlanetType = c.PlanetType,
                ParentPlanetName = c.ParentPlanet != null ? c.ParentPlanet.Name : null
            })
            .FirstOrDefaultAsync();

        if (celestialBody == null)
            return NotFound();

        return Ok(celestialBody);
    }

    // POST: api/CelestialBodies
    // Creates an celestialBody
    [HttpPost]
    public async Task<ActionResult<CelestialBodyDto>> CreateCelestialBody(CelestialBodyCreateDto dto)
    {
        var celestialBody = new CelestialBody {
            Name = dto.Name,
            Distance = (decimal)dto.Distance,
            BodyType = dto.BodyType,
            PlanetType = dto.PlanetType,
            FK_ParentPlanetID = dto.FK_ParentPlanetID
        };

        _context.CelestialBodies.Add(celestialBody);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return CreatedAtAction(nameof(GetCelestialBody), new { id = celestialBody.ID }, new CelestialBodyDto {
            ID = celestialBody.ID,
            Name = celestialBody.Name,
            Distance = (double)celestialBody.Distance,
            BodyType = celestialBody.BodyType,
            PlanetType = celestialBody.PlanetType
        });
    }

    // PUT: api/CelestialBodies/{id}
    // Updates celestialBody on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCelestialBody(int id, CelestialBodyUpdateDto dto)
    {
        if (id != dto.ID)
            return BadRequest();

        var celestialBody = await _context.CelestialBodies.FindAsync(id);
        if (celestialBody == null)
            return NotFound();

        celestialBody.Name = dto.Name;
        celestialBody.Distance = (decimal)dto.Distance;
        celestialBody.BodyType = dto.BodyType;
        celestialBody.PlanetType = dto.PlanetType;
        celestialBody.FK_ParentPlanetID = dto.FK_ParentPlanetID;

        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }

    // DELETE: api/CelestialBodies/{id}
    // Deletes celestialBody by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies.FindAsync(id);
        if (celestialBody == null)
            return NotFound();

        _context.CelestialBodies.Remove(celestialBody);
        await _context.SaveChangesAsync();

        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);

        return NoContent();
    }
}