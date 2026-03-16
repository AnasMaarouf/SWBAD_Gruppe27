public class Astronaut
{
    public int ID { get; set; }  // Primary Key
    public Employee Employee { get; set; }  // Navigation to Employee
    public string Rank { get; set; }
    public int FlightHours { get; set; }
    public string Paygrade { get; set; }
    public ICollection<Crew> Crews { get; set; }  // Relationship with Crew
}
