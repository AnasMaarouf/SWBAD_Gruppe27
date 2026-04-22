public class MissionResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Mission.Status CurrentStatus { get; set; }
    public string Type { get; set; }
    public DateOnly? LaunchDate { get; set; }
    public int Duration { get; set; }
    public string RocketModelName { get; set; }
    public string LaunchpadLocation { get; set; }
    public string ManagerName { get; set; }
    public string Target_CelestialBodyName { get; set; }
}

public class MissionDetailedResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Mission.Status CurrentStatus { get; set; }
    public string Type { get; set; }
    public DateOnly? LaunchDate { get; set; }
    public int Duration { get; set; }
    public string RocketModelName { get; set; }
    public string LaunchpadLocation { get; set; }
    public string ManagerName { get; set; }
    public string Target_CelestialBodyName { get; set; }

    public List<string> Scientists { get; set; }
    public List<string> Astronauts { get; set; }
}

public class CreateMissionDTO
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public Mission.Status Status { get; set; }
    public string Type { get; set; }
    public DateOnly? LaunchDate { get; set; }

    public int RocketId { get; set; }
    public int? LaunchpadId { get; set; }
    public int? CrewId { get; set; }
    public int ManagerId { get; set; }
    public int CelestialBodyId { get; set; }
}

public class UpdateMissionDTO
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public Mission.Status CurrentStatus { get; set; }
    public string Type { get; set; }
    public DateOnly? LaunchDate { get; set; }
    public int RocketId { get; set; }
    public int? LaunchpadId { get; set; }
    public int? CrewId { get; set; }
    public int ManagerId { get; set; }
    public int CelestialBodyId { get; set; }
}

public class MissionLogDTO {
    public string MissionID { get; set; }
    public string MissionName { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp {get; set; }
}