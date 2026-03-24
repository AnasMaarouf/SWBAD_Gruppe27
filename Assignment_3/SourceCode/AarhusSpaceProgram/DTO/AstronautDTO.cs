public class AstronautResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Rank { get; set; }
    public int FlightHours { get; set; }
    public string Paygrade { get; set; }
    public List<int> Crews { get; set; }
}

public class CreateAstronautDTO
{
    public int EmployeeId { get; set; }
    public string Rank { get; set; }
    public int FlightHours { get; set; }
    public string Paygrade { get; set; }
}

public class UpdateAstronautDTO
{
    public string Rank { get; set; }
    public int FlightHours { get; set; }
    public string Paygrade { get; set; }
}

public class AddAstronautToCrewDTO
{
    public int CrewId { get; set; }
}

public class RemoveAstronautToCrewDTO
{
    public int CrewId { get; set; }
}