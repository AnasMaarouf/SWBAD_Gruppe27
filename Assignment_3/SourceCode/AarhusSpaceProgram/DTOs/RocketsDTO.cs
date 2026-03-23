namespace AarhusSpaceProgram.DTOs;

public class RocketDto
{
    public int ID { get; set; }
    public string ModelName { get; set; } = "";
    public int? FuelCapacity { get; set; }
    public int? CrewCapacity { get; set; }
    public int? NumberOfStages { get; set; }
    public int? TotalWeight { get; set; }
    public string? MissionName { get; set; }
}

public class RocketCreateDto
{
    public string ModelName { get; set; } = "";
    public double Weight { get; set; }
}

public class RocketUpdateDto
{
    public int ID { get; set; }
    public string ModelName { get; set; } = "";
    public double Weight { get; set; }
}