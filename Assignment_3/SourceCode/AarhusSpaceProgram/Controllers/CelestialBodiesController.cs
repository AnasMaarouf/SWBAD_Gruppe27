using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<CelestialBodyResponseDTO>>> GetCelestialBodies() {
        var celestialbodies = await _context.CelestialBodies
            .Include(c => c.ParentPlanet)
            .Include(c => c.Moons)
            .Select(dto => new CelestialBodyResponseDTO {
                Id = dto.ID,
                Name = dto.Name,
                Distance = dto.Distance,
                BodyType = dto.BodyType,
                PlanetType = dto.PlanetType,
                ParentPlanetName = dto.ParentPlanet != null ? dto.ParentPlanet.Name : null,
                Moons = dto.Moons.Select(m => m.Name).ToList(),
                Missions = dto.Missions.Select(m => m.Name).ToList()
            })
            .ToListAsync();

        if(celestialbodies == null) {
            return NotFound("No celestialbodies exists");
        }

        return Ok(celestialbodies);
    }

    // GET: api/celestialBodies/{id}
    // Gets celestialBody from id (primary key)
    [HttpGet("{id}")]
    public async Task<ActionResult<CelestialBodyResponseDTO>> GetCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies
            .Include(c => c.ParentPlanet)
            .Include(c => c.Moons)
            .Select(dto => new CelestialBodyResponseDTO {
                Id = dto.ID,
                Name = dto.Name,
                Distance = dto.Distance,
                BodyType = dto.BodyType,
                PlanetType = dto.PlanetType,
                ParentPlanetName = dto.ParentPlanet != null ? dto.ParentPlanet.Name : null,
                Moons = dto.Moons.Select(m => m.Name).ToList(),
                Missions = dto.Missions.Select(m => m.Name).ToList()
            })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (celestialBody == null) {
            return NotFound("No celestialbody " + id + " was found");
        }

        

        return Ok(celestialBody);
    }

    // POST: api/CelestialBodies
    // Creates an celestialBody
    [HttpPost]
    public async Task<ActionResult<CreateCelestialBodyDTO>> CreateCelestialBody(CreateCelestialBodyDTO dto)
    {
        if(dto.Distance < 0) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Distance cannot be negative");
        }
        
        if(dto.BodyType != "Moon" && dto.BodyType != "Planet") {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("bodytype needs to be \"Planet\" or \"Moon\"");
        }


        var celestialbody = new CelestialBody {
            Name = dto.Name,
            Distance = dto.Distance,
            BodyType = dto.BodyType,
            PlanetType = dto.PlanetType,
            FK_ParentPlanetID = dto.BodyType != "Moon" ? dto.ParentPlanetId : null
        };

        _context.CelestialBodies.Add(celestialbody);
        await _context.SaveChangesAsync();
        
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "POST", Path = Request.Path, StatusCode = 201, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return CreatedAtAction(nameof(GetCelestialBody), new { id = celestialbody.ID }, celestialbody);
    }

    // PUT: api/CelestialBodies/{id}
    // Updates celestialBody on id
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCelestialBody(int id, UpdateCelestialBodyDTO dto) {
        
        if(dto.Distance < 0) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("Distance cannot be negative");
        }

        if(dto.BodyType != "Moon" && dto.BodyType != "Planet") {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 400, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return BadRequest("bodytype needs to be \"Planet\" or \"Moon\"");
        }

        var celestialbody = await _context.CelestialBodies.FindAsync(id);

        if(celestialbody == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("CelestialBody, does not exist");
        }

        celestialbody.Name = dto.Name;
        celestialbody.Distance = dto.Distance;
        celestialbody.BodyType = dto.BodyType;
        celestialbody.PlanetType = dto.PlanetType;
        celestialbody.FK_ParentPlanetID = dto.ParentPlanetId;

        await _context.SaveChangesAsync();

        {
        var timestamp = new DateTimeOffset(DateTime.UtcNow);
        var logInfo = new { Method = "PUT", Path = Request.Path, StatusCode = 200, Timestamp = timestamp };
        _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return Ok("Celestialbody updated");
    }

    // DELETE: api/CelestialBodies/{id}
    // Deletes celestialBody by id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCelestialBody(int id)
    {
        var celestialBody = await _context.CelestialBodies.FindAsync(id);
        if (celestialBody == null) {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 404, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
            return NotFound("Celestialbody doesn't exist");
        }

        _context.CelestialBodies.Remove(celestialBody);
        await _context.SaveChangesAsync();

        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Method = "DELETE", Path = Request.Path, StatusCode = 204, Timestamp = timestamp };
            _logger.LogInformation("Request called {@LogInfo}", logInfo);
        }

        return NoContent();
    }
}