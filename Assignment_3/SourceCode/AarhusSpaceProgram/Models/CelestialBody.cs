public class CelestialBody
{
    public int ID { get; set; }  // Primary Key for CelestialBody
    public string Name { get; set; }
    public decimal Distance { get; set; }
    public string BodyType { get; set; }  // Planet or Moon
    public string? PlanetType { get; set; }  // Rocky, Gas Giant (only for planets)
    public CelestialBody? ParentPlanet { get; set; }  // Navigation to parent planet
    public ICollection<CelestialBody>? Moons { get; set; }  // Moons of the celestial body
}

