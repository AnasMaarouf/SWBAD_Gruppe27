namespace AarhusSpaceProgram.DTOs;

public class CelestialBodyDto
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public double Distance { get; set; }
    public string BodyType { get; set; } = "";
    public string PlanetType { get; set; } = "";
    public string? ParentPlanetName { get; set; }
}

public class CelestialBodyCreateDto
{
    public string Name { get; set; } = "";
    public double Distance { get; set; }
    public string BodyType { get; set; } = "";
    public string PlanetType { get; set; } = "";
    public int? FK_ParentPlanetID { get; set; }
}

public class CelestialBodyUpdateDto
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public double Distance { get; set; }
    public string BodyType { get; set; } = "";
    public string PlanetType { get; set; } = "";
    public int? FK_ParentPlanetID { get; set; }
}