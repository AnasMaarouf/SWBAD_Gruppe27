namespace AarhusSpaceProgram.DTOs;

public class MissionDto
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public int CurrentStatus { get; set; }
    public DateOnly? LaunchDate { get; set; }

    public string? ManagerName { get; set; }
    public string? RocketModel { get; set; }
    public string? LaunchpadLocation { get; set; }
    public string? TargetCelestialBody { get; set; }

    public List<string>? Astronauts { get; set; }
    public List<string>? Scientists { get; set; }
}

public class MissionCreateDto
{
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
    public int CurrentStatus { get; set; }
    public DateOnly? LaunchDate { get; set; }

    public int FK_RocketID { get; set; }
    public int? FK_launchpadID { get; set; }
    public int? FK_CrewID { get; set; }
    public int FK_ManagerID { get; set; }
    public int FK_CelestialID { get; set; }
}

public class MissionUpdateDto
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
    public int CurrentStatus { get; set; }
    public DateOnly? LaunchDate { get; set; }

    public int FK_RocketID { get; set; }
    public int? FK_launchpadID { get; set; }
    public int? FK_CrewID { get; set; }
    public int FK_ManagerID { get; set; }
    public int FK_CelestialID { get; set; }
}