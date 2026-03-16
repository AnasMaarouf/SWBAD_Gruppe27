public class Crew
{
    public int ID { get; set; }  // Primary Key (Crew specific ID)
    public ICollection<Astronaut> Astronauts { get; set; }  // Astronauts in the crew
    public ICollection<Mission> Missions { get; set; }  // Missions associated with this crew
}

