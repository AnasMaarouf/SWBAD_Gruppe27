public class Crew
{
    public int ID { get; set; }  // Primary Key (Crew specific ID)
    public ICollection<Mission>? Missions { get; set; }  // Missions associated with this crew
    public ICollection<Joint_Astronaut_Crew>? joint_Astronaut_Crew { get; set; }  // Astronauts in the crew
}

