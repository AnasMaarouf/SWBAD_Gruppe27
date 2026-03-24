

public class Mission {
    public int ID { get; set; }  // Primary Key for Mission
    public string Name { get; set; }
    public int Duration { get; set; }
    public Status CurrentStatus { get; set; }
    public string Type { get; set; }
    public DateOnly? LaunchDate { get; set; }
    
    // Foreign Key for rocket.
    public int FK_RocketID { get; set; }
    public Rocket AssignedRocket { get; set; }  // Navigation to Rocket
    
    // Foreign Key for launchpad.
    public int? FK_launchpadID { get; set; }
    public Launchpad? launchpad { get; set; }  // Navigation to Launchpad
    
    // Foreign Key for crew.
    public int? FK_CrewID { get; set; }
    public Crew? crew { get; set; }  // Navigation to Crew
    
    // Foreign Key for manager.
    public int FK_ManagerID { get; set; }
    public Manager manager { get; set; }  // Navigation to Manager
    
    // Foreign Key for celestialBody.
    public int FK_CelestialID { get; set; }
    public CelestialBody celestialBody { get; set; }  // Navigation to CelestialBody (destination)

    // Navigation property for the joint between mission and scientist (Many-To-Many Relationship).
    public ICollection<Joint_Scientist_Mission>? joint_scientist_missions {get; set;}

    public enum Status : int {
        Created = 0,
        Budgeted,
        Approved,
        Planned,
        Active,
        Completed,
        Aborted,
        Failed
    }
}
