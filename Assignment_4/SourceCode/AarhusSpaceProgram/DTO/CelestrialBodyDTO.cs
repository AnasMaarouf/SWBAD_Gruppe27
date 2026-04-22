// Object to list all objects of the same table using http get
public class CelestialBodyResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Distance { get; set; }
    public string BodyType { get; set; }
    public string PlanetType { get; set; }
    public string ParentPlanetName { get; set; }
    public List<string> Moons { get; set; }
    public List<string> Missions { get; set; }
}
// Object to create the table using http post
public class CreateCelestialBodyDTO
{
    public string Name { get; set; }
    public decimal Distance { get; set; }
    public string BodyType { get; set; }
    public string PlanetType { get; set; }
    public int? ParentPlanetId { get; set; }
}

// Object to update the table using http put
public class UpdateCelestialBodyDTO
{
    public string Name { get; set; }
    public decimal Distance { get; set; }
    public string BodyType { get; set; }
    public string PlanetType { get; set; }
    public int? ParentPlanetId { get; set; }
}

