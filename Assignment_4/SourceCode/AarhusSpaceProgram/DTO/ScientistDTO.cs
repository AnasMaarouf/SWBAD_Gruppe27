public class ScientistResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Title { get; set; }
    public string Specialty { get; set; }
}

public class DetailedScientistResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }

    public string Title { get; set; }
    public string Specialty { get; set; }

    public List<string> Missions { get; set; }
    public List<string> Experiments { get; set; }
}

public class CreateScientistDTO
{
    public int EmployeeId { get; set; }

    public string Title { get; set; }
    public string Specialty { get; set; }
}

public class UpdateScientistDTO
{
    public string Title { get; set; }
    public string Specialty { get; set; }
}

public class AssignScientistToMissionDTO
{
    public int MissionID { get; set; }
}

public class UnassignScientistFromMissionDTO
{
    public int MissionID{get; set;}
}

public class AssignScientistToExperimentDTO
{
    public int ExperimentID{get; set;}
}
public class UnassignScientistFromExperimentDTO
{
    public int ExperimentID{get; set;}
}