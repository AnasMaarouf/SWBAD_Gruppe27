public class LaunchpadResponseDTO {
    public int Id { get; set; }
    public string Location { get; set; }
    public int CurrentStatus { get; set; }
}

public class DetailedLaunchpadResponseDTO {
    public int Id { get; set; }
    public string Location { get; set; }
    public int MaxSupportedWeight { get; set; }
    public int CurrentStatus { get; set; }
    public List<string> Missions { get; set; }
}

public class CreateLaunchpadDTO
{
    public string Location { get; set; }
    public int MaxSupportedWeight { get; set; }
    public int CurrentStatus { get; set; }
}

public class UpdateLaunchpadDTO
{
    public string Location { get; set; }
    public int MaxSupportedWeight { get; set; }
    public int CurrentStatus { get; set; }
}