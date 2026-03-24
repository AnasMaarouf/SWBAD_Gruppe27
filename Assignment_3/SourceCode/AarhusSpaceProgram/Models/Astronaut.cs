public class Astronaut
{
    public int ID { get; set; }  // Primary Key
    public Employee? employee { get; set; }  // Navigation to Employee
    public string Rank { get; set; }
    public int FlightHours { get; set; }
    public string Paygrade { get; set; }
    public ICollection<Joint_Astronaut_Crew>? joint_Astronaut_Crew { get; set; }  // Many-to-Many relationship with Crew (astronaut can be assigned many missions AS A PART OF A CREW)
}
