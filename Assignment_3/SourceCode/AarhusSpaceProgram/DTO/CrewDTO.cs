// Object to list all objects of the same table using http get
public class CrewResponseDto
{
    public int Id { get; set; }
    public List<string> Missions { get; set; }
    public List<string> Astronauts { get; set; }
}


public class CreateCrewDto {}

public class AddCrewToMissionDTO {
    public int CrewID { get; set; }
}

public class RemoveCrewFromMissionDTO
{
    public int CrewID { get; set; }
}