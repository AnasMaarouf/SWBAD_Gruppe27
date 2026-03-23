namespace AarhusSpaceProgram.DTOs;

public class LaunchpadDto
{
    public int ID { get; set; }
    public string Location { get; set; } = "";
    public string CurrentStatus { get; set; } = "";
    public int MaxSupportedWeight { get; set; }
}

public class LaunchpadCreateDto
{
    public string Location { get; set; } = "";
    public int CurrentStatus { get; set; }
    public int MaxSupportedWeight { get; set; }
}

public class LaunchpadUpdateDto
{
    public int ID { get; set; }
    public string Location { get; set; } = "";
    public int CurrentStatus { get; set; }
    public int MaxSupportedWeight { get; set; }
}