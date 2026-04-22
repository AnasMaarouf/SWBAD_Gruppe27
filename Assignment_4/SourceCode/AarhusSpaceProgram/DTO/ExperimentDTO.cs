public class ExperimentResponseDTO {
    public int ID { get; set;}  // Primary Key
    public string Name {get; set;}
    public DateOnly DayOfCreation {get; set;}
    public string AssignedMissionName {get; set;}
}

public class DetailedExperimentResponseDTO {
    public int ID { get; set;}  // Primary Key
    public string Name {get; set;}
    public string Description {get; set;}
    public DateOnly DayOfCreation {get; set;}
    public int AssignedMissionID {get; set;}
    public string AssignedMissionName {get; set;}
    public List<string> AssignedScientists {get; set;}
}


public class CreateExperimentDTO {
    public string Name {get; set;}
    public string Description {get; set;}
    public DateOnly CreationDate {get; set;}
    public int MissionID {get; set;}
}

public class UpdateExperimentDTO {
    public string Name {get; set;}
    public string Description {get; set;}
    public DateOnly CreationDate {get; set;}
    public int MissionID {get; set;}
}