public class ScientistResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; }

    public string Title { get; set; }
    public string Specialty { get; set; }

    public List<string> Missions { get; set; }
}

public class CreateScientistDto
{
    public int EmployeeId { get; set; }

    public string Title { get; set; }
    public string Specialty { get; set; }
}

public class UpdateScientistDto
{
    public string Title { get; set; }
    public string Specialty { get; set; }
}

public class AssignScientistToMissionDto
{
    public int MissionID { get; set; }
}

public class UnassignScientistFromMissionDto
{
    public int MissionID{get; set;}
}