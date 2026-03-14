

public class Mission
{
    public int ID { get; set; }  // Primary Key for Mission
    public string Name { get; set; }
    public int Duration { get; set; }
    public string CurrentStatus { get; set; }
    public string Type { get; set; }
    public DateOnly LaunchDate { get; set; }
    public Rocket Rocket { get; set; }  // Navigation to Rocket
    public Launchpad Launchpad { get; set; }  // Navigation to Launchpad
    public Crew Crew { get; set; }  // Navigation to Crew
    public Manager Manager { get; set; }  // Navigation to Manager
    public CelestialBody CelestialBody { get; set; }  // Navigation to CelestialBody (destination)
}
