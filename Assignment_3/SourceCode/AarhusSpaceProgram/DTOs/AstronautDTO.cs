namespace AarhusSpaceProgram.DTOs;
public class AstronautDto
{
    public int ID { get; set; }
    public string Rank { get; set; } = "";
    public int FlightHours { get; set; }
    public string Paygrade { get; set; } = "";
    public string? FullName { get; set; }
}

public class AstronautCreateDto
{
    public string Rank { get; set; } = "";
    public int FlightHours { get; set; }
    public string Paygrade { get; set; } = "";
    public int FK_EmployeeID { get; set; }
}

public class AstronautUpdateDto
{
    public int ID { get; set; }
    public string Rank { get; set; } = "";
    public int FlightHours { get; set; }
    public string Paygrade { get; set; } = "";
}