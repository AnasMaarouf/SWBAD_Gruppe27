public class RocketResponseDTO
{
    public int Id { get; set; }
    public string ModelName { get; set; }
    public int? FuelCapacity { get; set; }
    public int? CrewCapacity { get; set; }
    public int? NumberOfStages { get; set; }
    public int? TotalWeight { get; set; }

    public string assignedMission { get; set; }
}


public class CreateRocketDTO
{
    public string ModelName { get; set; }
    public int? FuelCapacity { get; set; }
    public int? CrewCapacity { get; set; }
    public int? NumberOfStages { get; set; }
    public int? TotalWeight { get; set; }
}


public class UpdateRocketDTO
{
    public string ModelName { get; set; }
    public int? FuelCapacity { get; set; }
    public int? CrewCapacity { get; set; }
    public int? NumberOfStages { get; set; }
    public int? TotalWeight { get; set; }
}


public class AssignRocketToMissionDTO
{
    public int RocketId { get; set; }
    public int MissionId { get; set; }
}